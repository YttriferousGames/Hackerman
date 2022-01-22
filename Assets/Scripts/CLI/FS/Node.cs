// Filesystem node (any kind)

// Different properties a Node can have
[Flags]
public enum NodeFlags : byte { None = 0, ReadOnly = 1, Hidden = 2 }

public abstract class Node {
    // Name (on filesystem) of node
    public string Name;
    public NodeFlags flags = NodeFlags.None;

    public override string ToString() {
        return Name;
    }
}
