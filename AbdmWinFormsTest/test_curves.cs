using System;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.EC;

class Program
{
    static void MainTestCurves()
    {
        var names = new string[] { "curve25519", "Curve25519", "X25519" };
        foreach (var name in names) {
            var c1 = CustomNamedCurves.GetByName(name);
            var c2 = ECNamedCurveTable.GetByName(name);
            Console.WriteLine($"{name} - CustomNamedCurves: {(c1 != null ? "Yes" : "No")}, ECNamedCurveTable: {(c2 != null ? "Yes" : "No")}");
        }
    }
}
