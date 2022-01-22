/// <summary>Different properties a Node can have</summary>
[Flags]
public enum NodeFlags : byte { None = 0, ReadOnly = 1, Hidden = 2 }

/// <summary>Filesystem node (any kind)</summary>
public abstract class Node {
    /// <summary>Name (on filesystem) of node</summary>
    public string Name;
    public NodeFlags flags = NodeFlags.None;

    public override string ToString() {
        return Name;
    }
}
