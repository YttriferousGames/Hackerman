using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;

/// <summary>Print working directory</summary>
public class PWD : Exe {
    public PWD(NodeFlags flags = NodeFlags.None) : base("pwd", flags) {}

    protected override IEnumerable<int?> Run(ProgAPI d) {
        if (d.args.Length > 0) {
            d.Out.Println("pwd: too many arguments");
            yield return 1;
        }
        d.Out.Println(d.sys.workingDir.ToString());
        yield return 0;
    }
}
