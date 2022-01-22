// Directory node (contains nodes)

using System.Linq;

public class Dir : Node {
    public List<Node> Contents;

    // TODO propagate ReadOnly
    public Dir(string name, IEnumerable<Node> contents, NodeFlags flags = NodeFlags.None) {
        Name = name;
        Contents = contents.ToList();
        Contents.Sort((x, y) => x.Name.CompareTo(y.Name));
        this.flags = flags;
    }
}
