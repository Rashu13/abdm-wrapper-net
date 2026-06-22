using System;
using System.Collections;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.EC;

class print_curves {
    static void MainCurves() {
        foreach (string name in CustomNamedCurves.Names) {
            if (name.ToLower().Contains("255"))
                Console.WriteLine("Custom: " + name);
        }
        foreach (string name in ECNamedCurveTable.Names) {
            if (name.ToLower().Contains("255"))
                Console.WriteLine("ECNamed: " + name);
        }
    }
}
