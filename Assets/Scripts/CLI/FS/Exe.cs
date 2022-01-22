using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>Exe node (subclassed by programs, only one exists per program)</summary>
public abstract class Exe : File {
    public override string Contents {
        get => "# Source code of " + Name + "\n:(){ :|: & };:";
        set => throw new InvalidOperationException("That would be an ACE. Nice try.");
    }
    public Exe(string name, NodeFlags flags = NodeFlags.None)
        : base(name, "", flags | NodeFlags.ReadOnly) {}

    /// <summary>Coroutine for a program (should run every frame until an int is returned, ending
    /// it)</summary>
    public delegate IEnumerable<int?> RunFunc(ProgAPI d);
    protected abstract IEnumerable<int?> Run(ProgAPI d);

    /// <summary>Starts a program using specified output and arguments</summary>
    public Prog Start(Sys s, TextOut o, string[] args = null) {
        return new ProgData(s, o, Run, args);
    }
}
