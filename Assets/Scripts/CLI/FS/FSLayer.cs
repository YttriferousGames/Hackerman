using System.Linq;

/// <summary>Contains an individual layer of a filesystem (can be combined with StackedFS)</summary>
public class FSLayer : FS {
    private List<Node> root;

    /// <summary>Returns node at path</summary>
    public override Node GetNode(Path path) {
        Node current = new Dir("", root);
        foreach (string n in path.nodes) {
            if (current is Dir d) {
                current = d.Contents.Find(x => x.Name == n);
            } else {
                return null;
            }
        }
        return current;
    }

    public FSLayer(IEnumerable<Node> root) {
        this.root = root.ToList();
    }
}
