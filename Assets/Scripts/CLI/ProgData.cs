// Contains the state provided to a program being executed

using UnityEngine;
using System.Collections.Generic;

// Things to redesign:
// - Input handling
//   - Consider a program that uses arrow keys, mouse input etc
//   - Remember mouse input should also be usable to highlight screen contents
//   - And all that custom input should only be done when it's explicitly allowed to (and when PC is
//   focused)
//   - Helper methods to get mouse in screen coords etc
//   - There should still be an easy way to just "readline" stuff
// - Ending the program
//   - E.g CTRL+C, minimising a running program, yielding an integer, raising exception etc
//   - Make sure state is all cleaned up at end (e.g, screen is set to null after exception)

public class ProgData : Prog, ProgAPI {
    public delegate void OutputEvent(string str, Color32 col);
    public delegate void ScreenEvent(Cell[,] screen);
    public string[] args { get => _args; }
    private readonly string[] _args;
    public string input { get => _input; }
    private string _input = "";
    // True if the program should be closed (CTRL+C)
    public bool close { get => _close; set => _close = value; }
    private bool _close = false;
    private readonly List<(string, Color32)> output = new List<(string, Color32)>();
    // public (string, Color32)[] Output { get => output.ToArray(); }
    private Cell[,] screen;
    public Cell[,] Screen {
        get => screen;
        set {
            screen = value;
            fullscreen = screen != null;
            OnScreen?.Invoke(screen);
        }
    }
    public Sys sys { get => _sys; }
    private readonly Sys _sys;
    // Whether the text output or fullscreen output is shown
    private bool fullscreen = false;
    // public bool Fullscreen { get => fullscreen; }

    public ProgData(Sys s, Exe.RunFunc p, string[] args = null) {
        this._sys = s;
        this._args = args ?? new string[] {};
        this.inner = p(this).GetEnumerator();
    }

    public event OutputEvent OnOutput;
    public event ScreenEvent OnScreen;

    public void Print(string s, Color32? col = null) {
        Color32 color = col ?? Util.WHITE;
        if (output.Count > 0) {
            (string t, Color32 c) = output[output.Count - 1];
            if (c.Equals(color)) {
                t += s;
                output[output.Count - 1] = (t, c);
                OnOutput?.Invoke(s, color);
                return;
            }
        }
        output.Add((s, color));
        OnOutput?.Invoke(s, color);
    }

    private void Blankln() {
        if (output.Count > 0) {
            (string t, Color32 c) = output[output.Count - 1];
            t += "\n";
            output[output.Count - 1] = (t, c);
            OnOutput?.Invoke("\n", c);
        } else {
            Color32 c = Util.WHITE;
            output.Add(("\n", c));
            OnOutput?.Invoke("\n", c);
        }
    }

    public void Println(string s = null, Color32? col = null) {
        if (s != null)
            Print(s + "\n", col);
        else
            Blankln();
    }

    public void Close() {
        _close = true;
    }

    // This is pretty inadequate
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

public interface ProgAPI {
    string[] args { get; }
    string input { get; }
    // True if the program should be closed (CTRL+C)
    bool close { get; set; }
    Sys sys { get; }
    Cell[,] Screen { set; }

    void Print(string s, Color32? col = null);

    void Println(string s = null, Color32? col = null);
}

public interface Prog {
    event ProgData.OutputEvent OnOutput;
    event ProgData.ScreenEvent OnScreen;

    void Close();

    // This is pretty inadequate
    void Input(string inp);

    int? Update();
}