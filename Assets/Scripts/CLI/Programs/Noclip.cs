using System.Collections.Generic;

/// <summary>Changes directory</summary>
public class Noclip : Exe {
    public Noclip(NodeFlags flags = NodeFlags.None) : base("noclip", flags) {}

    protected override IEnumerable<int?> Run(ProgAPI d) {
        PlayerMovement.instance.noclip ^= true;
        d.Out.Println(
            "Welcome to Team Fortress 2. After 9 years in development, hopefully it will have been worth the weight.");
        yield return 0;
    }
}
