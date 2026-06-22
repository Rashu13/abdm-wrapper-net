using System;
using Org.BouncyCastle.Math;

class test_curve {
    static void Main() {
        BigInteger _q = new BigInteger("7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFED", 16);
        BigInteger _a = new BigInteger("2AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA984914A144", 16);
        BigInteger _b = new BigInteger("7B425ED097B425ED097B425ED097B425ED097B425ED097B4260B5E9C7710C864", 16);
        BigInteger _Gx = new BigInteger("2AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD245A", 16);
        BigInteger _Gy = new BigInteger("20AE19A1B8A086B4E01EDD2C7748D14C923D4D7E6D7C61B229E9C5A27ECED3D9", 16);
        
        BigInteger left = _Gy.Multiply(_Gy).Mod(_q);
        BigInteger right = _Gx.Multiply(_Gx).Multiply(_Gx).Add(_a.Multiply(_Gx)).Add(_b).Mod(_q);
        
        Console.WriteLine($"Left:  {left.ToString(16)}");
        Console.WriteLine($"Right: {right.ToString(16)}");
        Console.WriteLine($"Match: {left.Equals(right)}");
    }
}
