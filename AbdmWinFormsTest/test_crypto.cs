using System;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using AbdmWrapperNet.Services;

class Program
{
    static void Main()
    {
        var svc = new CryptographyService(NullLogger<CryptographyService>.Instance);
        var keys = svc.GenerateKeys();
        
        Console.WriteLine("Private Key Base64 length: " + keys.PrivateKey.Length);
        Console.WriteLine("Public Key Base64 length: " + keys.PublicKey.Length);
        Console.WriteLine("Nonce Base64 length: " + keys.Nonce.Length);
        
        // Let's decode the shared key it would generate
        // The senderKeys.PublicKey is the uncompressed point.
        // We need to see what GetEncodedHipPublicKey generates.
        // Let's use reflection to call GetEncodedHipPublicKey
        var method = typeof(CryptographyService).GetMethod("GetEncodedHipPublicKey", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var encodedKey = (string)method.Invoke(svc, new object[] { keys.PublicKey });
        
        Console.WriteLine("KeyToShare (SubjectPublicKeyInfo) Base64: " + encodedKey);
    }
}
