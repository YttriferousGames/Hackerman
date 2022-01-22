using System.Linq;

// TODO Remember to support communication between systems and fancy stuff

/// <summary>The state of each individual computer</summary>
public class Sys : MonoBehaviour {
    /// <summary>Array of directories to search for programs</summary>
    public readonly Path[] sysPath = { "/bin" };

    [SerializeField]
    private FSLayerName[] layers = new FSLayerName[] { FSLayerName.Base };
    private FS fs;

    /// <summary>Home directory</summary>
    public readonly Path home = "/home/geff/";
    /// <summary>Current working directory</summary>
    public Path workingDir = "/home/geff/";

    /// <summary>Gets a Node at a path</summary>
    public Node GetNode(Path path) {
        Path abs = CanonPath(path);
        return fs.GetNode(abs);
    }

    /// <summary>Gets a Node at a path, recursing through symlinks</summary>
    public Node GetNode(Path path, bool recurseSymlinks) {
        Path abs = CanonPath(path);
        return fs.GetNode(abs, recurseSymlinks);
    }

    /// <summary>Gets a Node of type T at a path</summary>
    public T GetNode<T>(Path path)
        where T : class {
        Path abs = CanonPath(path);
        return fs.GetNode<T>(abs);
    }

    /// <summary>Gets a Node of type T at a path, recursing through symlinks</summary>
    public T GetNode<T>(Path path, bool recurseSymlinks)
        where T : class {
        Path abs = CanonPath(path);
        return fs.GetNode<T>(abs, recurseSymlinks);
    }

    /// <summary>Resolves a Node to a version without symlinks</summary>
    public Node ResolveSymlink(Node node) {
        return fs.ResolveSymlink(node);
    }

    /// <summary>Resolves a Node of type T to a version without symlinks</summary>
    public T ResolveSymlink<T>(Node node)
        where T : class {
        return fs.ResolveSymlink<T>(node);
    }

    /// <summary>Searches path for a program</summary>
    public Exe GetProgram(string proc) {
        Path pr = new Path(proc);
        if (pr.nodes.Length == 1) {
            foreach (Path p in sysPath) {
                Path ex = pr.WithBase(p);
                Exe exe = GetNode<Exe>(ex, true);
                if (exe != null)
                    return exe;
            }
            return null;
        } else {
            return GetNode<Exe>(pr, true);
        }
    }

    /// <summary>Searches path for a specific program T</summary>
    public T GetProgram<T>(string proc)
        where T : Exe {
        Path pr = new Path(proc);
        if (pr.nodes.Length == 1) {
            foreach (Path p in sysPath) {
                Path ex = pr.WithBase(p);
                T exe = GetNode<T>(ex, true);
                if (exe != null)
                    return exe;
            }
            return null;
        } else {
            return GetNode<T>(pr, true);
        }
    }

    /// <summary>Returns the canonical version of a path</summary>
    public Path CanonPath(Path path) {
        switch (path.pathType) {
            case PathType.Relative:
                return path.WithBase(workingDir);
            default:
                return new Path(path);
        }
    }

    private void Start() {
        fs = new StackedFS(layers.Select(l => l.Create()));
    }
}
