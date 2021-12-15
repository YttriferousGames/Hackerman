using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnvVars {
    private Dictionary<string, string> vars = new Dictionary<string, string>();
}

// TODO layered FS where files are added/removed/modified as game progresses, per PC.
// Not sure how to implement, I could use [Flags] or an incrementing integer.
// Trouble is that Exes depend on system state,
// probably worth changing so I can share them between computers.
// That doesn't fix the problem of mutability, perhaps I can make things read only
// Remember to support communication between systems and fancy stuff

// TODO contains state of a system
public class Sys : MonoBehaviour {
    public Path[] sysPath = { "/bin" };

    public FSLayerName[] layers = new FSLayerName[] { FSLayerName.Base };
    public FS fs;

    public TermLayout disp;

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
        foreach (Path p in sysPath) {
            if (GetNode(p) is Dir d) {
                Node n = ResolveSymlink<Exe>(d.Contents.Find(x => x.Name == proc));
                if (n is Exe exe) return exe;
            }
        }
        return null;
    }

    public Path home = "/home/geff/";
    public Path workingDir = "/home/geff/";

    public Path CanonPath(Path path) {
        switch (path.pathType) {
            case PathType.Relative:
                return path.WithBase(workingDir);
            default:
                return new Path(path);
        }
    }

    public void Print() {
        disp.Print();
    }

    public void Print(string s, Nullable<Color32> col = null) {
        disp.Print(s, col);
    }

    public void Println() {
        disp.Println();
    }

    public void Println(string s, Nullable<Color32> col = null) {
        disp.Println(s, col);
    }

    public void SetScreen(Cell[,] screen) {
        disp.Render(screen);
    }

    public void Start() {
        fs = new StackedFS(layers.Select(l => l.Create()));
    }
}
