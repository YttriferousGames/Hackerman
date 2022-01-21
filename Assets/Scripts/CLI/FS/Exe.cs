// Exe node (subclassed by programs, only one exists per program)

using System;
using System.Collections.Generic;

public abstract class Exe : File {
    public override string Contents {
        get => "# Source code of " + Name + "\n:(){ :|: & };:";
        set => throw new InvalidOperationException("That would be an ACE. Nice try.");
    }
    public Exe(string name, NodeFlags flags = NodeFlags.None)
        : base(name, "", flags | NodeFlags.ReadOnly) {}

    // Coroutine for a program (should run every frame until an int is returned, ending it)
    public delegate IEnumerable<int?> RunFunc(ProgAPI d);
    protected abstract IEnumerable<int?> Run(ProgAPI d);

    public Prog Start(Sys s, string[] args = null) {
        return new ProgData(s, Run, args);
    }
}
