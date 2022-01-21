// The state of each individual computer

using System;
using UnityEngine;
using System.Linq;

// TODO Remember to support communication between systems and fancy stuff

public class Sys : MonoBehaviour {
    public readonly Path[] sysPath = { "/bin" };

    [SerializeField]
    private FSLayerName[] layers = new FSLayerName[] { FSLayerName.Base };
    private FS fs;

    // TODO expose dimensions in a logical way
    [System.Obsolete]
    public readonly int width = 40;

    public readonly Path home = "/home/geff/";
    public Path workingDir = "/home/geff/";

    public Node GetNode(Path path) {
        Path abs = CanonPath(path);
        return fs.GetNode(abs);
    }

    public Node GetNode(Path path, bool recurseSymlinks) {
        Path abs = CanonPath(path);
        return fs.GetNode(abs, recurseSymlinks);
    }

    public T GetNode<T>(Path path)
        where T : class {
        Path abs = CanonPath(path);
        return fs.GetNode<T>(abs);
    }

    public T GetNode<T>(Path path, bool recurseSymlinks)
        where T : class {
        Path abs = CanonPath(path);
        return fs.GetNode<T>(abs, recurseSymlinks);
    }

    public Node ResolveSymlink(Node node) {
        return fs.ResolveSymlink(node);
    }

    public T ResolveSymlink<T>(Node node)
        where T : class {
        return fs.ResolveSymlink<T>(node);
    }

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
