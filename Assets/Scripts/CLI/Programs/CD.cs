using System.Collections.Generic;

public class CD : Exe {
    public CD(NodeFlags flags = NodeFlags.None) : base("cd", flags) {}

    protected override IEnumerable<int?> Run(ProgData d) {
        if (d.args.Length == 0) {
            yield return 0;
        } else if (d.args.Length > 1) {
            d.sys.Println("cd: too many arguments");
            yield return 1;
        }
        Path p = d.sys.CanonPath(d.args[0]);
        Node n = d.sys.GetNode(p);
        if (n is Dir dir) {
            d.sys.workingDir = p;
        } else if (n != null) {
            d.sys.Println("cd: not a directory: " + p.ToString());
            yield return 1;
        } else {
            d.sys.Println("cd: no such file or directory: " + p.ToString());
            yield return 1;
        }
        yield return 0;
    }
}
