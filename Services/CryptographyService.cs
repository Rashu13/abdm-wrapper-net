using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Crypto.EC;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

/// <summary>
/// EC curve25519 encryption service that works on Windows and Linux Docker.
/// Uses hardcoded Weierstrass parameters to avoid BouncyCastle name-lookup
/// failures on Linux (CustomNamedCurves.GetByName returns null there).
/// Generates SubjectPublicKeyInfo with id-ecPublicKey OID (1.2.840.10045.2.1)
/// which is required by the NHA Java HIU parser.
/// </summary>
public class CryptographyService : ICryptographyService
{
    private readonly ILogger<CryptographyService> _logger;

    // curve25519 Weierstrass-form parameters (from BouncyCastle source, platform-independent)
    private static readonly Org.BouncyCastle.Math.BigInteger _q =
        new("7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFED", 16);
    private static readonly Org.BouncyCastle.Math.BigInteger _a =
        new("2AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA984914A144", 16);
    private static readonly Org.BouncyCastle.Math.BigInteger _b =
        new("7B425ED097B425ED097B425ED097B425ED097B425ED097B425ED097B4260B5E", 16);
    private static readonly Org.BouncyCastle.Math.BigInteger _n =
        new("1000000000000000000000000000000014DEF9DEA2F79CD65812631A5CF5D3ED", 16);
    private static readonly Org.BouncyCastle.Math.BigInteger _h =
        Org.BouncyCastle.Math.BigInteger.ValueOf(8);
    private static readonly Org.BouncyCastle.Math.BigInteger _Gx =
        new("2AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD245A", 16);
    private static readonly Org.BouncyCastle.Math.BigInteger _Gy =
        new("20AE19A1B8A086B4E01EDD2C7748D14C923D4D7E6D7C61B229E9C5A27ECED3D9", 16);

    private static ECDomainParameters? _ecSpec;
    private static readonly object _specLock = new();

    public CryptographyService(ILogger<CryptographyService> logger)
    {
        _logger = logger;
    }

    // ─── Get curve25519 domain params (cached, hardcoded) ────────────────────

    private static ECDomainParameters GetEcSpec()
    {
        if (_ecSpec != null) return _ecSpec;
        lock (_specLock)
        {
            if (_ecSpec != null) return _ecSpec;

            // Try BouncyCastle name lookup first (works on Windows)
            X9ECParameters? named = CustomNamedCurves.GetByName("curve25519")
                                 ?? CustomNamedCurves.GetByName("Curve25519");

            if (named != null)
            {
                _ecSpec = new ECDomainParameters(named.Curve, named.G, named.N, named.H, named.GetSeed());
                return _ecSpec;
            }

            // Fallback: hardcode Weierstrass params (works on Linux Docker)
            var curve = new Org.BouncyCastle.Math.EC.FpCurve(_q, _a, _b, _n, _h);
            var G = curve.CreatePoint(_Gx, _Gy);
            _ecSpec = new ECDomainParameters(curve, G, _n, _h);
            return _ecSpec;
        }
    }

    // ─── Public API ───────────────────────────────────────────────────────────

    public EncryptionKeys GenerateKeys()
    {
        try
        {
            var ecSpec = GetEcSpec();
            var generator = new ECKeyPairGenerator();
            generator.Init(new ECKeyGenerationParameters(ecSpec, new SecureRandom()));

            AsymmetricCipherKeyPair keyPair = generator.GenerateKeyPair();
            var privateKey = (ECPrivateKeyParameters)keyPair.Private;
            var publicKey  = (ECPublicKeyParameters)keyPair.Public;

            byte[] nonceBytes = new byte[32];
            new SecureRandom().NextBytes(nonceBytes);

            return new EncryptionKeys
            {
                PrivateKey = Convert.ToBase64String(privateKey.D.ToByteArray()),
                PublicKey  = Convert.ToBase64String(publicKey.Q.GetEncoded(false)), // uncompressed
                Nonce      = Convert.ToBase64String(nonceBytes)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating ECDH keys");
            throw;
        }
    }

    /// <summary>
    /// Returns the SubjectPublicKeyInfo DER (base64) for the given raw public key.
    /// This is the format sent in keyMaterial.dhPublicKey.keyValue (HIU side).
    /// </summary>
    public string GetSubjectPublicKeyInfo(string rawPublicKeyBase64)
    {
        return GetEncodedEcPublicKey(rawPublicKeyBase64);
    }

    public EncryptionResponse Encrypt(HIPHealthInformationRequest request, List<HealthInformationBundle> bundles)
    {
        if (request?.HiRequest?.KeyMaterial?.DhPublicKey == null)
            throw new ArgumentException("Invalid health information request: receiver public key is missing");

        var senderKeys = GenerateKeys();

        string receiverPublicKeyB64 = request.HiRequest.KeyMaterial.DhPublicKey.KeyValue;
        string receiverNonce        = request.HiRequest.KeyMaterial.Nonce;

        byte[] xorOfRandom = XorOfRandom(senderKeys.Nonce, receiverNonce);
        var encryptedBundles = new List<HealthInformationBundle>();

        foreach (var bundle in bundles)
        {
            try
            {
                string enc = EncryptContent(xorOfRandom, senderKeys.PrivateKey, receiverPublicKeyB64, bundle.BundleContent);
                encryptedBundles.Add(new HealthInformationBundle
                {
                    CareContextReference = bundle.CareContextReference,
                    BundleContent = enc
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error encrypting care context: {bundle.CareContextReference}");
            }
        }

        // KeyToShare: SubjectPublicKeyInfo DER of HIP's ephemeral EC key (id-ecPublicKey OID)
        string keyToShare = GetEncodedEcPublicKey(senderKeys.PublicKey);

        return new EncryptionResponse
        {
            HealthInformationBundles = encryptedBundles,
            KeyToShare  = keyToShare,
            SenderNonce = senderKeys.Nonce
        };
    }

    public string Decrypt(string hipNonce, string hiuNonce, string hiuPrivateKey, string hipPublicKey, string encryptedData)
    {
        byte[] xorOfRandom = XorOfRandom(hipNonce, hiuNonce);
        return DecryptContent(xorOfRandom, hiuPrivateKey, hipPublicKey, encryptedData);
    }

    // ─── Private helpers ──────────────────────────────────────────────────────

    private static byte[] XorOfRandom(string senderNonce, string receiverNonce)
    {
        byte[] s = Convert.FromBase64String(senderNonce);
        byte[] r = Convert.FromBase64String(receiverNonce);
        byte[] result = new byte[s.Length];
        for (int i = 0; i < s.Length; i++)
            result[i] = (byte)(s[i] ^ r[i % r.Length]);
        return result;
    }

    private string EncryptContent(byte[] xorOfRandom, string senderPrivateB64, string receiverPublicB64, string plaintext)
    {
        // Java's loadPublicKey uses decodePoint(data) — raw EC point, NOT SubjectPublicKeyInfo
        byte[] sharedSecret = DoEcdh(senderPrivateB64, receiverPublicB64, useX509ForPublic: false);

        byte[] iv = new byte[12];
        Array.Copy(xorOfRandom, xorOfRandom.Length - 12, iv, 0, 12);

        byte[] aesKey = DeriveAesKey(xorOfRandom, sharedSecret);
        byte[] input  = Encoding.UTF8.GetBytes(plaintext);

        var cipher = new GcmBlockCipher(new AesEngine());
        cipher.Init(true, new AeadParameters(new KeyParameter(aesKey), 128, iv, null));
        byte[] output = new byte[cipher.GetOutputSize(input.Length)];
        int len = cipher.ProcessBytes(input, 0, input.Length, output, 0);
        len += cipher.DoFinal(output, len);
        byte[] result = new byte[len];
        Array.Copy(output, result, len);
        return Convert.ToBase64String(result);
    }

    private string DecryptContent(byte[] xorOfRandom, string receiverPrivateB64, string senderPublicB64, string ciphertext)
    {
        byte[] sharedSecret = DoEcdh(receiverPrivateB64, senderPublicB64, useX509ForPublic: true);

        byte[] iv = new byte[12];
        Array.Copy(xorOfRandom, xorOfRandom.Length - 12, iv, 0, 12);

        byte[] aesKey = DeriveAesKey(xorOfRandom, sharedSecret);
        byte[] enc    = Convert.FromBase64String(ciphertext);

        var cipher = new GcmBlockCipher(new AesEngine());
        cipher.Init(false, new AeadParameters(new KeyParameter(aesKey), 128, iv, null));
        byte[] output = new byte[cipher.GetOutputSize(enc.Length)];
        int len = cipher.ProcessBytes(enc, 0, enc.Length, output, 0);
        len += cipher.DoFinal(output, len);
        byte[] result = new byte[len];
        Array.Copy(output, result, len);

        string raw = Encoding.UTF8.GetString(result);
        try { using var doc = JsonDocument.Parse(raw); return JsonSerializer.Serialize(doc.RootElement); }
        catch { return raw; }
    }

    /// <summary>
    /// EC Diffie-Hellman on curve25519.
    /// privateKeyB64: BigInteger bytes (base64).
    /// publicKeyB64:  SubjectPublicKeyInfo DER bytes (base64) when useX509ForPublic=true,
    ///                or raw uncompressed EC point when false.
    /// </summary>
    private byte[] DoEcdh(string privateKeyB64, string publicKeyB64, bool useX509ForPublic)
    {
        var ecSpec = GetEcSpec();

        byte[] privBytes = Convert.FromBase64String(privateKeyB64);
        var privateKeyParams = new ECPrivateKeyParameters(
            new Org.BouncyCastle.Math.BigInteger(privBytes), ecSpec);

        byte[] pubBytes = Convert.FromBase64String(publicKeyB64);
        ECPublicKeyParameters publicKeyParams;
        if (useX509ForPublic)
        {
            publicKeyParams = (ECPublicKeyParameters)PublicKeyFactory.CreateKey(pubBytes);
        }
        else
        {
            var point = ecSpec.Curve.DecodePoint(pubBytes);
            publicKeyParams = new ECPublicKeyParameters(point, ecSpec);
        }

        var agreement = new ECDHBasicAgreement();
        agreement.Init(privateKeyParams);
        var secret = agreement.CalculateAgreement(publicKeyParams);

        byte[] secretBytes = secret.ToByteArrayUnsigned();
        int fieldSize = (ecSpec.Curve.FieldSize + 7) / 8;
        if (secretBytes.Length < fieldSize)
        {
            byte[] padded = new byte[fieldSize];
            Array.Copy(secretBytes, 0, padded, fieldSize - secretBytes.Length, secretBytes.Length);
            return padded;
        }
        return secretBytes;
    }

    private static byte[] DeriveAesKey(byte[] xorOfRandoms, byte[] sharedSecret)
    {
        byte[] salt = new byte[20];
        Array.Copy(xorOfRandoms, 0, salt, 0, Math.Min(20, xorOfRandoms.Length));
        var hkdf = new HkdfBytesGenerator(new Sha256Digest());
        hkdf.Init(new HkdfParameters(sharedSecret, salt, null));
        byte[] key = new byte[32];
        hkdf.GenerateBytes(key, 0, 32);
        return key;
    }

    /// <summary>
    /// Encodes a raw uncompressed EC public key (base64) as SubjectPublicKeyInfo DER (base64).
    /// Uses id-ecPublicKey OID (1.2.840.10045.2.1) — required by Java HIU parser.
    /// </summary>
    private string GetEncodedEcPublicKey(string rawPublicKeyBase64)
    {
        var ecSpec = GetEcSpec();
        byte[] publicKeyBytes = Convert.FromBase64String(rawPublicKeyBase64);
        var point = ecSpec.Curve.DecodePoint(publicKeyBytes);
        var publicKeyParams = new ECPublicKeyParameters(point, ecSpec);
        byte[] spki = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKeyParams).GetEncoded();
        return Convert.ToBase64String(spki);
    }
}
