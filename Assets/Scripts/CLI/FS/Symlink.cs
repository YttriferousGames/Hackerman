/// <summary>Symlink node (points to another node)</summary>

public class Symlink : Node {
    /// <summary>The <see cref="Path"/> the symlink points to</summary>
    public Path Target;

    public Symlink(string name, Path target, NodeFlags flags = NodeFlags.None) {
        Name = name;
        Target = target;
        this.flags = flags;
    }
}
