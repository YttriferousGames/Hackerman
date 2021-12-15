// Symlink node (points to another node)

public class Symlink : Node {
    public Path Target;

    public Symlink(string name, Path target, NodeFlags flags = NodeFlags.None) {
        Name = name;
        Target = target;
        this.flags = flags;
    }
}
