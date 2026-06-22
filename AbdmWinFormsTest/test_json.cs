using System;
using System.Text.Json;

class Program
{
    static void Main()
    {
        var dataPushPayload = new
        {
            pageNumber = 1,
            pageCount = 1,
            transactionId = "1234-5678",
            entries = new[]
            {
                new {
                    content = "encrypted_content",
                    media = "application/fhir+json",
                    checksum = "MD5",
                    careContextReference = "CC-1234"
                }
            },
            keyMaterial = new
            {
                cryptoAlg = "ECDH",
                curve = "Curve25519",
                dhPublicKey = new
                {
                    expiry = "2026-06-20T14:31:22.000Z",
                    parameters = "Curve25519",
                    keyValue = "PUBLIC_KEY"
                },
                nonce = "NONCE"
            }
        };
        
        Console.WriteLine(JsonSerializer.Serialize(dataPushPayload));
    }
}
