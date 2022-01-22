// File node (stores text contents)

public class File : Node {
    private string _contents;
    public virtual string Contents {
        get => _contents;
        set {
            if (!flags.HasFlag(NodeFlags.ReadOnly))
                _contents = value;
        }
    }

    public File(string name, string contents, NodeFlags flags = NodeFlags.None) {
        Name = name;
        _contents = contents;
        this.flags = flags;
    }

    public File(string name, TextAsset contents, NodeFlags flags = NodeFlags.None) {
        Name = name;
        _contents = contents.text;
        this.flags = flags;
    }
}
