using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Flags]
public enum NodeFlags : byte { None = 0, ReadOnly = 1, Hidden = 2 }

// Filesystem node (any kind)
public abstract class Node {
    // Name (on filesystem) of node
    public string Name;
    public NodeFlags flags = NodeFlags.None;
}

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

// Symlink node (points to another node)
public class Symlink : Node {
    public Path Target;

    public Symlink(string name, Path target, NodeFlags flags = NodeFlags.None) {
        Name = name;
        Target = target;
        this.flags = flags;
    }
}

// Directory node (contains nodes)
public class Dir : Node {
    public List<Node> Contents;

    // TODO sort alphabetically, propogate ReadOnly
    public Dir(string name, IEnumerable<Node> contents, NodeFlags flags = NodeFlags.None) {
        Name = name;
        Contents = contents.ToList();
        this.flags = flags;
    }
}

public class ProgData {
    public readonly string[] args;
    public string input = "";
    public bool close = false;
    public string output = "";
    public readonly Sys sys;

    public ProgData(Sys s, string[] args = null) {
        this.sys = s;
        this.args = args ?? new string[] {};
    }

    // TODO refactor printing/whole screen control
    // Make it easier to tell if programs can read mouse/keyboard
    // Have a helper method to get mouse coordinates
    // public void Print() {}
}

public class Prog {
    private ProgData d;
    private IEnumerator<int?> inner;

    public Prog(ProgData d, IEnumerable<int?> inner) {
        this.d = d;
        this.inner = inner.GetEnumerator();
    }

    public void Close() {
        d.close = true;
    }

    public void Input(string inp) {
        d.input += inp;
    }

    public int? Update() {
        if (inner.MoveNext()) {
            d.input = "";
            return null;
        } else {
            return inner.Current ?? 0;
        }
    }
}

// Exe node (subclassed by programs)
public abstract class Exe : File {
    public override string Contents {
        get => "# Source code of " + Name + "\n:(){ :|: & };:";
        set => throw new InvalidOperationException("That would be an ACE. Nice try.");
    }
    public Exe(string name, NodeFlags flags = NodeFlags.None)
        : base(name, "", flags | NodeFlags.ReadOnly) {}

    // Coroutine for a program (should run every frame until an int is returned, ending it)
    protected abstract IEnumerable<int?> Run(ProgData d);

    public Prog Start(Sys s, string[] args = null) {
        ProgData d = new ProgData(s, args);
        return new Prog(d, Run(d));
    }
}

public static class SysStart {
    public static Prog Start(this Sys s, Exe e, string[] args = null) {
        return e.Start(s, args);
    }
}

// TODO this should be a ScriptableObject
// So they can be edited in the UI, and then layered
public abstract class FS {
    private const int SYMLINK_DEPTH = 4;
    // Gets a Node at an absolute path
    public abstract Node GetNode(Path path);

    // TODO what about directory symlinks in the middle of a path
    private Node GetNode(Path path, int recurseSymlinks) {
        if (recurseSymlinks <= 0) {
            return GetNode(path);
        } else {
            Node n = GetNode(path);
            if (n is Symlink l) {
                return GetNode(l.Target, recurseSymlinks - 1);
            } else {
                return n;
            }
        }
    }

    // Gets a Node at an absolute path, recursing through symlinks
    public Node GetNode(Path path, bool recurseSymlinks) {
        if (!recurseSymlinks)
            return GetNode(path);
        else
            return GetNode(path, SYMLINK_DEPTH);
    }

    // Gets a Node of type T at an absolute path
    public T GetNode<T>(Path path)
        where T : class {
        if (GetNode(path) is T t) {
            return t;
        } else {
            return null;
        }
    }

    // Gets a Node of type T at an absolute path, recursing through symlinks
    public T GetNode<T>(Path path, bool recurseSymlinks)
        where T : class {
        if (GetNode(path, recurseSymlinks) is T t) {
            return t;
        } else {
            return null;
        }
    }

    // Resolves a Node to a version without symlinks
    public Node ResolveSymlink(Node node) {
        if (node is Symlink l) {
            return GetNode(l.Target, SYMLINK_DEPTH - 1);
        } else {
            return node;
        }
    }

    // Resolves a Node of type T to a version without symlinks
    public T ResolveSymlink<T>(Node node)
        where T : class {
        if (ResolveSymlink(node) is T t) {
            return t;
        } else {
            return null;
        }
    }
}

public class FSLayer : FS {
    private List<Node> root;

    public override Node GetNode(Path path) {
        Node current = new Dir("", root);
        foreach (string n in path.nodes) {
            if (current is Dir d) {
                current = d.Contents.Find(x => x.Name == n);
            } else {
                return null;
            }
        }
        return current;
    }

    public FSLayer(IEnumerable<Node> root) {
        this.root = root.ToList();
    }
}

// Should I keep FSLayer immutable?
// Should I merge the layers in the creation of this?
// idk
public class StackedFS : FS {
    private List<FSLayer> layers;

    public override Node GetNode(Path path) {
        foreach (FSLayer layer in layers) {
            Node r = layer.GetNode(path);
            if (r != null)
                return r;
        }
        return null;
    }

    public StackedFS(IEnumerable<FSLayer> layers) {
        this.layers = layers.ToList();
    }
}
