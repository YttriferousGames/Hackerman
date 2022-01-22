using System.Linq;

public class LS : Exe {
    public LS(NodeFlags flags = NodeFlags.None) : base("ls", flags) {}

    private void ListPath(ProgAPI d, Path p, bool name = false) {
        Node n = d.sys.GetNode(p);
        if (n is Dir dir) {
            if (name)
                d.Out.Println(p.ToString() + ":");
            d.Out.Println(String.Join(
                ' ',
                dir.Contents.Where(n => !n.flags.HasFlag(NodeFlags.Hidden)).Select(n => n.Name)));
        } else {
            d.Out.Println(n.Name);
        }
    }

    protected override IEnumerable<int?> Run(ProgAPI d) {
        if (d.args.Length == 0) {
            ListPath(d, d.sys.workingDir);
        } else if (d.args.Length == 1) {
            ListPath(d, d.sys.CanonPath(d.args[0]));
        } else {
            foreach (string s in d.args) {
                Path p = d.sys.CanonPath(s);
                ListPath(d, p, true);
                d.Out.Println();
            }
        }
        yield return 0;
    }
}
