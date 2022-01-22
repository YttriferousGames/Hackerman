using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;

// TODO Things to redesign:
// - Input handling
//   - Consider a program that uses arrow keys, mouse input etc
//   - Remember mouse input should also be usable to highlight screen contents
//   - And all that custom input should only be done when it's explicitly allowed to (and when PC is
//   focused)
//   - Helper methods to get mouse in screen coords etc
//   - There should still be an easy way to just "readline" stuff

/// <summary>Contains the state of a program being executed</summary>
public class ProgData : Prog, ProgAPI {
    public string[] args { get => _args; }
    private readonly string[] _args;
    public string input { get => _input; }
    private string _input = "";
    // True if the program should be closed (CTRL+C)
    public bool close {
        get => _close;
        set => _close = value;
    }
    private bool _close = false;
    private TextOut _o = null;
    public TextOut Out { get => _o; }
    public Sys sys { get => _sys; }
    private readonly Sys _sys;

    public ProgData(Sys s, TextOut o, Exe.RunFunc p, string[] args = null) {
        this._sys = s;
        this._o = o;
        this._args = args ?? new string[] {};
        this.inner = p(this).GetEnumerator();
    }

    public void Close() {
        _close = true;
    }

    public void Input(string inp) {
        _input += inp;
    }

    private IEnumerator<int?> inner = null;

    public int? Update() {
        if (inner.MoveNext()) {
            _input = "";
            return null;
        } else {
            return inner.Current ?? 0;
        }
    }
}

/// <summary>API provided to programs as they execute</summary>
public interface ProgAPI {
    /// <summary>Arguments provided to a program</summary>
    string[] args { get; }
    /// <summary>Input provided to program during frame</summary>
    string input { get; }
    /// <summary>True if the program should be closed (CTRL+C)</summary>
    bool close { get; set; }
    /// <summary>The system the program is run on</summary>
    Sys sys { get; }
    /// <summary>Terminal output</summary>
    TextOut Out { get; }
}

/// <summary>API for interacting with a program</summary>
public interface Prog {
    /// <summary>Request the program to close</summary>
    void Close();

    // TODO This is pretty inadequate
    /// <summary>Provide input to the program on this frame</summary>
    void Input(string inp);

    /// <summary>Run the program for a frame</summary>
    int? Update();
}