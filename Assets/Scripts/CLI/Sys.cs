using System;
using UnityEngine;

// TODO layered FS where files are added/removed/modified as game progresses, per PC.
// Not sure how to implement, I could use [Flags] or an incrementing integer.
// Trouble is that Exes depend on system state,
// probably worth changing so I can share them between computers.
// That doesn't fix the problem of mutability, perhaps I can make things read only
// Remember to support communication between systems and fancy stuff

// TODO contains state of a system
public class Sys : MonoBehaviour {
    public Path[] sysPath = { "/bin" };

    public Node[] root;

    public TermRenderer disp;

    public Node GetNode(Path path) {
        Path abs = CanonPath(path);
        Node current = new Dir("PogChamp", root);
        foreach (string n in abs.nodes) {
            if (current is Dir d) {
                current = d.Contents.Find(x => x.Name == n);
            } else {
                return null;
            }
        }
        return current;
    }

    public Exe GetProgram(string proc) {
        foreach (Path p in sysPath) {
            if (GetNode(p) is Dir d) {
                Node n = Unlink(d.Contents.Find(x => x.Name == proc));
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
            case PathType.Home:
                return path.WithBase(home);
            default:
                return new Path(path);
        }
    }

    // TODO symlink to symlink, avoid loop
    public Node Unlink(Node node) {
        if (node is Symlink link) {
            return GetNode(link.Target);
        } else {
            return node;
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

    public void Start() {
        root = new Node[] {
            new Dir("bin",
                    new Node[] { new Echo(this), new External(this, "pacman"),
                                 new Symlink("yay", "/bin/pacman"), new CD(this), new PWD(this),
                                 new LS(this), new Cat(this), new SelfDestruct(this) }),
            new Dir("home", new Node[] { new Dir(
                                "geff", new Node[] { new File("README.txt", "Hello, world!") }) })
        };
    }
}
