using System.Collections.Generic;

public class PWD : Exe {
    public PWD(NodeFlags flags = NodeFlags.None) : base("pwd", flags) {}

    protected override IEnumerable<int?> Run(ProgData d) {
        if (d.args.Length > 0) {
            d.Println("pwd: too many arguments");
            yield return 1;
        }
        d.Println(d.sys.workingDir.ToString());
        yield return 0;
    }
}
