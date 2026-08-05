// Type declarations for jsencrypt
declare module 'jsencrypt' {
  export default class JSEncrypt {
    constructor(options?: { default_key_size?: number; default_public_exponent?: string });
    setPublicKey(publicKey: string): void;
    setPrivateKey(privateKey: string): void;
    encrypt(str: string): string | false;
    decrypt(str: string): string | false;
    getPublicKey(): string;
    getPrivateKey(): string;
  }
}
