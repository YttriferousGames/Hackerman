using System.Collections.Generic;

public class Cat : Exe {
    public Cat(NodeFlags flags = NodeFlags.None) : base("cat", flags) {}

    protected override IEnumerable<int?> Run(ProgData d) {
        foreach (string s in d.args) {
            File f = d.sys.GetNode<File>(s, true);
            // TODO helper method to get symlinks too
            // Handle nested symlinks to depth etc
            if (f != null) {
                d.sys.Println(f.Contents);
            } else {
                d.sys.Println("cat: " + s + ": is not a file");
                yield return 1;
                yield break;
            }
            yield return null;
        }
        yield return 0;
    }
}
