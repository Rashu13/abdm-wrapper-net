using System.Collections.Generic;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

public interface ICryptographyService
{
    EncryptionKeys GenerateKeys();
    EncryptionResponse Encrypt(HIPHealthInformationRequest request, List<HealthInformationBundle> bundles);
    string Decrypt(string hipNonce, string hiuNonce, string hiuPrivateKey, string hipPublicKey, string encryptedData);
}
