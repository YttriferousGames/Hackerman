using System;
using System.Collections.Generic;

public class Echo : Exe {
    public Echo(NodeFlags flags = NodeFlags.None) : base("echo", flags) {}

    protected override IEnumerable<int?> Run(ProgData d) {
        d.Println(String.Join(' ', d.args));
        yield return 0;
    }
}
