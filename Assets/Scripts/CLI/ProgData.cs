using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

// TODO Things to redesign:
// - Input handling
//   - Consider a program that uses arrow keys, mouse input etc
//   - Remember mouse input should also be usable to highlight screen contents
//   - And all that custom input should only be done when it's explicitly allowed to (and when PC is
//   focused)
//   - Helper methods to get mouse in screen coords etc
//   - There should be an easy way to just "readline" stuff - that is, a "readline" function that
//   does all the work for you

/// <summary>Contains the state of a program being executed</summary>
public class ProgData : Prog, ProgAPI {
    public string[] args { get => _args; }
    private readonly string[] _args;
    public bool close {
        get => _close;
        set => _close = value;
    }
    private bool _close = false;
    private TextOut _o = null;
    public TextOut Out { get => _o; }
    public Sys sys { get => _sys; }
    private readonly Sys _sys;

    private bool _canInput;
    public bool canInput {
        get => _canInput;
        set => _canInput = value;
    }

    public ProgData(Sys s, TextOut o, Exe.RunFunc p, string[] args = null) {
        this._sys = s;
        this._o = o;
        this._args = args ?? new string[] {};
        this.inner = p(this).GetEnumerator();
    }

    public void Close() {
        _close = true;
    }

    private IEnumerator<int?> inner = null;

    public int? Update() {
        if (inner.MoveNext()) {
            return null;
        } else {
            return inner.Current ?? 0;
        }
    }
}

// TODO handle cursor movement, screen display etc
// History? arrow keys to swap? probably out of scope for this function
// TODO generalize so it doesn't have to end at newline, and you can work directly with Cell
// lists
public class Readline {
    private ProgAPI d;

    private string prompt;
    private Color32 promptCol = Util.WHITE;
    private string i;
    private Color32 iCol = Util.WHITE;

    private bool exit = false;

    public string Line { get => i; }

    public bool Complete {
        get => exit;
        set {
            if (value && !exit)
                Stop();
        }
    }

    private void UpdateScreen() {
        d.Out.SetLine(prompt, promptCol);
        d.Out.Print(i, iCol);
    }

    private void Stop() {
        d.Out.Println();
        exit = true;
        Keyboard.current.onTextInput -= OnTextInput;
    }

    private void OnTextInput(char c) {
        if (!exit && d.canInput) {
            if (c == '\n' || c == '\r') {
                Stop();
                return;
            } else if (c == '\b') {
                if (i.Length > 0) {
                    i = i.Remove(i.Length - 1, 1);
                } else {
                    return;
                }
            } else if (c == '\x1B' || c == '\a' || c == '\f' || c == '\t' || c == '\v') {
            } else {
                i += c;
            }
            UpdateScreen();
        }
    }

    public Readline(ProgAPI d, string prompt, Color32? promptCol, string i = "",
                    Color32? iCol = null) {
        this.d = d;
        this.prompt = prompt;
        if (promptCol is Color32 pc)
            this.promptCol = pc;
        this.i = i;
        if (iCol is Color32 ic)
            this.iCol = ic;
        UpdateScreen();
        Keyboard.current.onTextInput += OnTextInput;
    }
}

/// <summary>API provided to programs as they execute</summary>
public interface ProgAPI {
    /// <summary>Arguments provided to a program</summary>
    string[] args { get; }
    /// <summary>True if the program should be closed (CTRL+C)</summary>
    bool close { get; set; }
    /// <summary>The system the program is run on</summary>
    Sys sys { get; }
    /// <summary>Terminal output</summary>
    TextOut Out { get; }
    /// <summary>Whether or not a program is allowed to read input from the user</summary>
    bool canInput { get; }
}

/// <summary>API for interacting with a program</summary>
public interface Prog {
    bool canInput { get; set; }

    /// <summary>Request the program to close</summary>
    void Close();

    /// <summary>Run the program for a frame</summary>
    int? Update();
}