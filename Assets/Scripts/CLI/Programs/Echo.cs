using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;

/// <summary>Echoes arguments to the screen</summary>
public class Echo : Exe {
    public Echo(NodeFlags flags = NodeFlags.None) : base("echo", flags) {}

    protected override IEnumerable<int?> Run(ProgAPI d) {
        d.Out.Println(String.Join(' ', d.args));
        yield return 0;
    }
}
