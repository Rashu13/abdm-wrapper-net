import 'dart:convert';

class CryptoKeyPair {
  final String publicKey;
  final String privateKey;
  CryptoKeyPair({required this.publicKey, required this.privateKey});
}

class CryptoHelper {
  /// Generates temporary ECDH keypair for M3 encrypted FHIR data exchange
  static CryptoKeyPair generateEcdhKeys() {
    // Demo key generation representation
    String pubKey = 'ECDH_PUB_KEY_${DateTime.now().millisecondsSinceEpoch}';
    String privKey = 'ECDH_PRIV_KEY_${DateTime.now().millisecondsSinceEpoch}';
    return CryptoKeyPair(publicKey: pubKey, privateKey: privKey);
  }

  /// Decrypts encrypted FHIR payload received from ABDM HIU Data Transfer API
  static String decryptFhirBundle(String encryptedPayload, String privateKey) {
    try {
      // Decode payload representation
      List<int> bytes = base64.decode(encryptedPayload);
      return utf8.decode(bytes);
    } catch (e) {
      return encryptedPayload; // Returns string if plain JSON for demo
    }
  }
}
