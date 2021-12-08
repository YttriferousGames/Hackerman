using System;
using System.Collections.Generic;
using System.Linq;

// Filesystem node (any kind)
public abstract class Node {
    // Name (on filesystem) of node
    public string Name;
    // bool readOnly = false;
    // bool hidden = false;
}

// File node (stores text contents)
public class File : Node {
    private string _contents;
    public virtual string Contents {
        get => _contents;
        set => _contents = value;
    }

    public File(string name, string contents) {
        Name = name;
        _contents = contents;
    }
}

// Symlink node (points to another node)
public class Symlink : Node {
    public Path Target;

    public Symlink(string name, Path target) {
        Name = name;
        Target = target;
    }
}

// Directory node (contains nodes)
public class Dir : Node {
    public List<Node> Contents;

    public Dir(string name, IEnumerable<Node> contents) {
        Name = name;
        Contents = contents.ToList();
    }
}

// Exe node (subclassed by programs)
// TODO should I subclass File so it works with cat and the like easier
public abstract class Exe : File {
    public override string Contents {
        get => "# Source code of " + Name + "\n:(){ :|: & };:";
        set => throw new InvalidOperationException("That would be an ACE. Nice try.");
    }
    public Sys sys;
    public Exe(Sys sys, string name) : base(name, null) {
        this.sys = sys;
    }

    // Starts a program (and returns true if it's ended)
    public abstract bool Start(string[] args);

    // Updates a program every frame (and returns true if it's ended)
    public virtual bool Update(/*byte[] stdin*/) {
        return true;
    }

    // Tells a program to close (and returns true if it does)
    // Not guaranteed to be called (yet)
    public virtual bool Close() {
        return true;
    }
}

// TODO this should be a ScriptableObject
// So they can be edited in the UI, and then layered
public class FS {
    // TODO
}
