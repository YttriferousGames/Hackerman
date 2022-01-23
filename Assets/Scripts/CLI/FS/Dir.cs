using System.Collections.Generic;
using System.Linq;

/// <summary>Directory node (contains nodes)</summary>
public class Dir : Node {
    /// <summary>Child nodes</summary>
    public List<Node> Contents;

    private static void PropagateFlags(Node n, NodeFlags f) {
        n.flags |= f;
        if (n is Dir d) {
            // Assume already propagated
            if ((d.flags & f) == f)
                return;
            foreach (Node x in d.Contents) {
                PropagateFlags(x, f);
            }
        }
    }

    public Dir(string name, IEnumerable<Node> contents, NodeFlags flags = NodeFlags.None) {
        Name = name;
        Contents = contents.ToList();
        Contents.Sort((x, y) => x.Name.CompareTo(y.Name));
        this.flags = flags; foreach (Node x in Contents) { PropagateFlags(x, flags); }
    }
}
