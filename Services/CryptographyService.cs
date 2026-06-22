using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

/// <summary>
/// Cross-platform ECDH encryption service using BouncyCastle X25519.
/// Uses X25519KeyPairGenerator which works on both Windows and Linux Docker.
/// Matches the NHA Java wrapper EncryptionService.java behavior.
/// </summary>
public class CryptographyService : ICryptographyService
{
    private readonly ILogger<CryptographyService> _logger;

    public CryptographyService(ILogger<CryptographyService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Generates an X25519 key pair and a random nonce.
    /// </summary>
    public EncryptionKeys GenerateKeys()
    {
        try
        {
            var random = new SecureRandom();

            // X25519KeyPairGenerator works on all platforms (Windows + Linux Docker)
            var generator = new X25519KeyPairGenerator();
            generator.Init(new X25519KeyGenerationParameters(random));
            var keyPair = generator.GenerateKeyPair();

            var privateKey = (X25519PrivateKeyParameters)keyPair.Private;
            var publicKey = (X25519PublicKeyParameters)keyPair.Public;

            byte[] nonceBytes = new byte[32];
            random.NextBytes(nonceBytes);

            return new EncryptionKeys
            {
                PrivateKey = Convert.ToBase64String(privateKey.GetEncoded()),
                PublicKey = Convert.ToBase64String(publicKey.GetEncoded()),
                Nonce = Convert.ToBase64String(nonceBytes)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating ECDH keys");
            throw;
        }
    }

    /// <summary>
    /// Encrypts FHIR bundles using the HIU's public key material from the request.
    /// </summary>
    public EncryptionResponse Encrypt(HIPHealthInformationRequest request, List<HealthInformationBundle> bundles)
    {
        if (request?.HiRequest?.KeyMaterial?.DhPublicKey == null)
        {
            throw new ArgumentException("Invalid health information request: receiver public key is missing");
        }

        var senderKeys = GenerateKeys();

        string receiverPublicKeyB64 = request.HiRequest.KeyMaterial.DhPublicKey.KeyValue;
        string receiverNonce = request.HiRequest.KeyMaterial.Nonce;

        byte[] xorOfRandom = XorOfRandom(senderKeys.Nonce, receiverNonce);
        List<HealthInformationBundle> encryptedBundles = new();

        foreach (var bundle in bundles)
        {
            try
            {
                string encryptedContent = EncryptContent(
                    xorOfRandom,
                    senderKeys.PrivateKey,
                    receiverPublicKeyB64,
                    bundle.BundleContent
                );

                encryptedBundles.Add(new HealthInformationBundle
                {
                    CareContextReference = bundle.CareContextReference,
                    BundleContent = encryptedContent
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error encrypting care context: {bundle.CareContextReference}");
            }
        }

        // Encode HIP's public key as SubjectPublicKeyInfo DER for the HIU to use
        string keyToShare = GetEncodedX25519PublicKey(senderKeys.PublicKey);

        return new EncryptionResponse
        {
            HealthInformationBundles = encryptedBundles,
            KeyToShare = keyToShare,
            SenderNonce = senderKeys.Nonce
        };
    }

    /// <summary>
    /// Decrypts received encrypted health data using HIU's private key.
    /// </summary>
    public string Decrypt(string hipNonce, string hiuNonce, string hiuPrivateKey, string hipPublicKey, string encryptedData)
    {
        byte[] xorOfRandom = XorOfRandom(hipNonce, hiuNonce);
        return DecryptContent(xorOfRandom, hiuPrivateKey, hipPublicKey, encryptedData);
    }

    // ─── Private Helpers ──────────────────────────────────────────────────────

    private byte[] XorOfRandom(string senderNonce, string receiverNonce)
    {
        byte[] randomSender   = Convert.FromBase64String(senderNonce);
        byte[] randomReceiver = Convert.FromBase64String(receiverNonce);

        byte[] combined = new byte[randomSender.Length];
        for (int i = 0; i < randomSender.Length; i++)
        {
            combined[i] = (byte)(randomSender[i] ^ randomReceiver[i % randomReceiver.Length]);
        }
        return combined;
    }

    private string EncryptContent(byte[] xorOfRandom, string senderPrivateKeyB64, string receiverPublicKeyB64, string plaintext)
    {
        byte[] sharedSecret = DoX25519(senderPrivateKeyB64, receiverPublicKeyB64);

        byte[] iv = new byte[12];
        Array.Copy(xorOfRandom, xorOfRandom.Length - 12, iv, 0, 12);

        byte[] aesKey = DeriveAesKey(xorOfRandom, sharedSecret);
        byte[] inputBytes = Encoding.UTF8.GetBytes(plaintext);

        GcmBlockCipher cipher = new GcmBlockCipher(new AesEngine());
        cipher.Init(true, new AeadParameters(new KeyParameter(aesKey), 128, iv, null));

        byte[] outBytes = new byte[cipher.GetOutputSize(inputBytes.Length)];
        int len = cipher.ProcessBytes(inputBytes, 0, inputBytes.Length, outBytes, 0);
        int finalLen = len + cipher.DoFinal(outBytes, len);

        byte[] encrypted = new byte[finalLen];
        Array.Copy(outBytes, 0, encrypted, 0, finalLen);
        return Convert.ToBase64String(encrypted);
    }

    private string DecryptContent(byte[] xorOfRandom, string receiverPrivateKeyB64, string senderPublicKeyB64, string ciphertext)
    {
        byte[] sharedSecret = DoX25519(receiverPrivateKeyB64, senderPublicKeyB64);

        byte[] iv = new byte[12];
        Array.Copy(xorOfRandom, xorOfRandom.Length - 12, iv, 0, 12);

        byte[] aesKey        = DeriveAesKey(xorOfRandom, sharedSecret);
        byte[] encryptedBytes = Convert.FromBase64String(ciphertext);

        GcmBlockCipher cipher = new GcmBlockCipher(new AesEngine());
        cipher.Init(false, new AeadParameters(new KeyParameter(aesKey), 128, iv, null));

        byte[] outBytes = new byte[cipher.GetOutputSize(encryptedBytes.Length)];
        int len = cipher.ProcessBytes(encryptedBytes, 0, encryptedBytes.Length, outBytes, 0);
        int finalLen = len + cipher.DoFinal(outBytes, len);

        byte[] decrypted = new byte[finalLen];
        Array.Copy(outBytes, 0, decrypted, 0, finalLen);

        string raw = Encoding.UTF8.GetString(decrypted);
        try
        {
            using var doc = JsonDocument.Parse(raw);
            return JsonSerializer.Serialize(doc.RootElement);
        }
        catch
        {
            return raw;
        }
    }

    /// <summary>
    /// Performs X25519 ECDH agreement. Both keys are raw 32-byte X25519 keys (base64 encoded).
    /// If publicKey is a SubjectPublicKeyInfo DER (>32 bytes), extracts the raw key from it.
    /// </summary>
    private byte[] DoX25519(string privateKeyB64, string publicKeyB64)
    {
        byte[] privateKeyBytes = Convert.FromBase64String(privateKeyB64);
        byte[] publicKeyBytes  = Convert.FromBase64String(publicKeyB64);

        // Strip SubjectPublicKeyInfo header if present (DER encoded, >32 bytes)
        // X25519 raw key is always exactly 32 bytes
        if (publicKeyBytes.Length > 32)
        {
            // Last 32 bytes of SubjectPublicKeyInfo are the raw key
            byte[] raw = new byte[32];
            Array.Copy(publicKeyBytes, publicKeyBytes.Length - 32, raw, 0, 32);
            publicKeyBytes = raw;
        }

        // Strip leading sign byte from private key if BigInteger-encoded (33 bytes)
        if (privateKeyBytes.Length == 33 && privateKeyBytes[0] == 0x00)
        {
            byte[] raw = new byte[32];
            Array.Copy(privateKeyBytes, 1, raw, 0, 32);
            privateKeyBytes = raw;
        }

        var privKey = new X25519PrivateKeyParameters(privateKeyBytes, 0);
        var pubKey  = new X25519PublicKeyParameters(publicKeyBytes, 0);

        var agreement = new Org.BouncyCastle.Crypto.Agreement.X25519Agreement();
        agreement.Init(privKey);

        byte[] secret = new byte[agreement.AgreementSize];
        agreement.CalculateAgreement(pubKey, secret, 0);
        return secret;
    }

    /// <summary>
    /// Derives 32-byte AES key via HKDF-SHA256 (matches Java wrapper).
    /// </summary>
    private byte[] DeriveAesKey(byte[] xorOfRandoms, byte[] sharedSecret)
    {
        byte[] salt = new byte[20];
        Array.Copy(xorOfRandoms, 0, salt, 0, Math.Min(20, xorOfRandoms.Length));

        var hkdf = new HkdfBytesGenerator(new Sha256Digest());
        hkdf.Init(new HkdfParameters(sharedSecret, salt, null));

        byte[] aesKey = new byte[32];
        hkdf.GenerateBytes(aesKey, 0, 32);
        return aesKey;
    }

    /// <summary>
    /// Encodes the 32-byte X25519 public key as a SubjectPublicKeyInfo DER, then base64.
    /// This is the format the Java HIU parser expects for "keyToShare".
    /// </summary>
    private string GetEncodedX25519PublicKey(string base64RawPublicKey)
    {
        byte[] rawKey = Convert.FromBase64String(base64RawPublicKey);

        // X25519 SubjectPublicKeyInfo DER header (OID 1.3.101.110)
        // 30 2a 30 05 06 03 2b 65 6e 03 21 00 <32 bytes key>
        byte[] header = new byte[]
        {
            0x30, 0x2a,             // SEQUENCE (42 bytes)
            0x30, 0x05,             // SEQUENCE (5 bytes)
            0x06, 0x03, 0x2b, 0x65, 0x6e, // OID 1.3.101.110 (X25519)
            0x03, 0x21,             // BIT STRING (33 bytes)
            0x00                    // no unused bits
        };

        byte[] spki = new byte[header.Length + rawKey.Length];
        Array.Copy(header, 0, spki, 0, header.Length);
        Array.Copy(rawKey, 0, spki, header.Length, rawKey.Length);

        return Convert.ToBase64String(spki);
    }
}
