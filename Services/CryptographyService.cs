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
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

public class CryptographyService : ICryptographyService
{
    private readonly ILogger<CryptographyService> _logger;
    private const string CurveName = "curve25519";

    public CryptographyService(ILogger<CryptographyService> logger)
    {
        _logger = logger;
    }

    public EncryptionKeys GenerateKeys()
    {
        try
        {
            X9ECParameters ecParameters = ECNamedCurveTable.GetByName(CurveName);
            ECDomainParameters domainParams = new ECDomainParameters(
                ecParameters.Curve,
                ecParameters.G,
                ecParameters.N,
                ecParameters.H,
                ecParameters.GetSeed()
            );

            ECKeyGenerationParameters keyGenParams = new ECKeyGenerationParameters(domainParams, new SecureRandom());
            ECKeyPairGenerator generator = new ECKeyPairGenerator();
            generator.Init(keyGenParams);

            AsymmetricCipherKeyPair keyPair = generator.GenerateKeyPair();
            
            ECPrivateKeyParameters privateKey = (ECPrivateKeyParameters)keyPair.Private;
            ECPublicKeyParameters publicKey = (ECPublicKeyParameters)keyPair.Public;

            byte[] privateKeyBytes = privateKey.D.ToByteArrayUnsigned();
            byte[] publicKeyBytes = publicKey.Q.GetEncoded(false); // Uncompressed point encoding

            byte[] nonceBytes = new byte[32];
            new SecureRandom().NextBytes(nonceBytes);

            return new EncryptionKeys
            {
                PrivateKey = Convert.ToBase64String(privateKeyBytes),
                PublicKey = Convert.ToBase64String(publicKeyBytes),
                Nonce = Convert.ToBase64String(nonceBytes)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating ECDH keys");
            throw;
        }
    }

    public EncryptionResponse Encrypt(HIPHealthInformationRequest request, List<HealthInformationBundle> bundles)
    {
        if (request?.HiRequest?.KeyMaterial?.DhPublicKey == null)
        {
            throw new ArgumentException("Invalid health information request: receiver public key is missing");
        }

        var senderKeys = GenerateKeys();
        
        string receiverPublicKey = request.HiRequest.KeyMaterial.DhPublicKey.KeyValue;
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
                    receiverPublicKey,
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

        // Shared key is the DER-encoded SubjectPublicKeyInfo of HIP's public key
        string keyToShare = GetEncodedHipPublicKey(senderKeys.PublicKey);

        return new EncryptionResponse
        {
            HealthInformationBundles = encryptedBundles,
            KeyToShare = keyToShare,
            SenderNonce = senderKeys.Nonce
        };
    }

    public string Decrypt(string hipNonce, string hiuNonce, string hiuPrivateKey, string hipPublicKey, string encryptedData)
    {
        byte[] xorOfRandom = XorOfRandom(hipNonce, hiuNonce);
        return DecryptContent(xorOfRandom, hiuPrivateKey, hipPublicKey, encryptedData);
    }

    private byte[] XorOfRandom(string senderNonce, string receiverNonce)
    {
        byte[] randomSender = Convert.FromBase64String(senderNonce);
        byte[] randomReceiver = Convert.FromBase64String(receiverNonce);

        byte[] combinedRandom = new byte[randomSender.Length];
        for (int i = 0; i < randomSender.Length; i++)
        {
            combinedRandom[i] = (byte)(randomSender[i] ^ randomReceiver[i % randomReceiver.Length]);
        }
        return combinedRandom;
    }

    private string EncryptContent(byte[] xorOfRandom, string senderPrivateKey, string receiverPublicKey, string stringToEncrypt)
    {
        byte[] privateKeyBytes = Convert.FromBase64String(senderPrivateKey);
        byte[] publicKeyBytes = Convert.FromBase64String(receiverPublicKey);

        byte[] sharedSecret = DoEcdh(privateKeyBytes, publicKeyBytes, useX509ForPublic: false);

        byte[] iv = new byte[12];
        Array.Copy(xorOfRandom, xorOfRandom.Length - 12, iv, 0, 12);

        byte[] aesKey = GenerateAesKey(xorOfRandom, sharedSecret);

        byte[] inputBytes = Encoding.UTF8.GetBytes(stringToEncrypt);

        GcmBlockCipher cipher = new GcmBlockCipher(new AesEngine());
        AeadParameters parameters = new AeadParameters(new KeyParameter(aesKey), 128, iv, null);
        cipher.Init(true, parameters);

        byte[] outBytes = new byte[cipher.GetOutputSize(inputBytes.Length)];
        int len = cipher.ProcessBytes(inputBytes, 0, inputBytes.Length, outBytes, 0);
        int finalLen = len + cipher.DoFinal(outBytes, len);

        byte[] actualEncrypted = new byte[finalLen];
        Array.Copy(outBytes, 0, actualEncrypted, 0, finalLen);

        return Convert.ToBase64String(actualEncrypted);
    }

    private string DecryptContent(byte[] xorOfRandom, string receiverPrivateKey, string senderPublicKey, string stringToDecrypt)
    {
        byte[] privateKeyBytes = Convert.FromBase64String(receiverPrivateKey);
        byte[] publicKeyBytes = Convert.FromBase64String(senderPublicKey);

        byte[] sharedSecret = DoEcdh(privateKeyBytes, publicKeyBytes, useX509ForPublic: true);

        byte[] iv = new byte[12];
        Array.Copy(xorOfRandom, xorOfRandom.Length - 12, iv, 0, 12);

        byte[] aesKey = GenerateAesKey(xorOfRandom, sharedSecret);

        byte[] encryptedBytes = Convert.FromBase64String(stringToDecrypt);

        GcmBlockCipher cipher = new GcmBlockCipher(new AesEngine());
        AeadParameters parameters = new AeadParameters(new KeyParameter(aesKey), 128, iv, null);
        cipher.Init(false, parameters);

        byte[] outBytes = new byte[cipher.GetOutputSize(encryptedBytes.Length)];
        int len = cipher.ProcessBytes(encryptedBytes, 0, encryptedBytes.Length, outBytes, 0);
        int finalLen = len + cipher.DoFinal(outBytes, len);

        byte[] decryptedBytes = new byte[finalLen];
        Array.Copy(outBytes, 0, decryptedBytes, 0, finalLen);

        string rawDecrypted = Encoding.UTF8.GetString(decryptedBytes);

        // Normalize format by parsing and formatting as JSON to clean up any formatting/whitespaces, matching Java HIU's JsonParser behavior
        try
        {
            using var doc = JsonDocument.Parse(rawDecrypted);
            return JsonSerializer.Serialize(doc.RootElement);
        }
        catch
        {
            return rawDecrypted;
        }
    }

    private byte[] DoEcdh(byte[] privateKeyBytes, byte[] publicKeyBytes, bool useX509ForPublic)
    {
        ECPrivateKeyParameters privateKeyParams = LoadPrivateKey(privateKeyBytes);
        ECPublicKeyParameters publicKeyParams = useX509ForPublic 
            ? LoadPublicKeyFromX509(publicKeyBytes) 
            : LoadPublicKeyFromUncompressedPoint(publicKeyBytes);

        ECDHBasicAgreement agreement = new ECDHBasicAgreement();
        agreement.Init(privateKeyParams);
        
        var secretBigInt = agreement.CalculateAgreement(publicKeyParams);
        byte[] secret = secretBigInt.ToByteArrayUnsigned();

        if (secret.Length < 32)
        {
            byte[] paddedSecret = new byte[32];
            Array.Copy(secret, 0, paddedSecret, 32 - secret.Length, secret.Length);
            return paddedSecret;
        }

        return secret;
    }

    private ECPrivateKeyParameters LoadPrivateKey(byte[] data)
    {
        X9ECParameters ecP = ECNamedCurveTable.GetByName(CurveName);
        ECDomainParameters domainParams = new ECDomainParameters(ecP.Curve, ecP.G, ecP.N, ecP.H, ecP.GetSeed());
        var d = new Org.BouncyCastle.Math.BigInteger(1, data);
        return new ECPrivateKeyParameters(d, domainParams);
    }

    private ECPublicKeyParameters LoadPublicKeyFromUncompressedPoint(byte[] data)
    {
        X9ECParameters ecP = ECNamedCurveTable.GetByName(CurveName);
        ECDomainParameters domainParams = new ECDomainParameters(ecP.Curve, ecP.G, ecP.N, ecP.H, ecP.GetSeed());
        ECPoint q = ecP.Curve.DecodePoint(data);
        return new ECPublicKeyParameters(q, domainParams);
    }

    private ECPublicKeyParameters LoadPublicKeyFromX509(byte[] data)
    {
        AsymmetricKeyParameter pubKey = PublicKeyFactory.CreateKey(data);
        return (ECPublicKeyParameters)pubKey;
    }

    private byte[] GenerateAesKey(byte[] xorOfRandoms, byte[] sharedSecret)
    {
        byte[] salt = new byte[20];
        Array.Copy(xorOfRandoms, 0, salt, 0, 20);

        HkdfBytesGenerator hkdfBytesGenerator = new HkdfBytesGenerator(new Sha256Digest());
        hkdfBytesGenerator.Init(new HkdfParameters(sharedSecret, salt, null));

        byte[] aesKey = new byte[32];
        hkdfBytesGenerator.GenerateBytes(aesKey, 0, 32);

        return aesKey;
    }

    private string GetEncodedHipPublicKey(string base64PublicKey)
    {
        byte[] publicKeyBytes = Convert.FromBase64String(base64PublicKey);
        ECPublicKeyParameters publicKeyParams = LoadPublicKeyFromUncompressedPoint(publicKeyBytes);
        
        byte[] subjectPublicKeyInfoBytes = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKeyParams).GetEncoded();
        return Convert.ToBase64String(subjectPublicKeyInfoBytes);
    }
}
