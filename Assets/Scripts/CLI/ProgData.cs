// Contains the state provided to a program being executed

using UnityEngine;
using System.Collections.Generic;

public delegate void OutputEvent(string str, Color32 col);
public delegate void ScreenEvent(Cell[,] screen);

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

public class ProgData {
    public readonly string[] args;
    public string input = "";
    // True if the program should be closed (CTRL+C)
    public bool close = false;
    private readonly List<(string, Color32)> output = new List<(string, Color32)>();
    public (string, Color32)[] Output { get => output.ToArray(); }
    private Cell[,] screen;
    public Cell[,] Screen { get => screen; }
    public readonly Sys sys;
    // Whether the text output or fullscreen output is shown
    private bool fullscreen = false;
    public bool Fullscreen { get => fullscreen; }

    public ProgData(Sys s, string[] args = null) {
        this.sys = s;
        this.args = args ?? new string[] {};
    }

    public event OutputEvent OutputHandler;
    public event ScreenEvent ScreenHandler;

    public void Print(string s, Color32? col = null) {
        Color32 color = col ?? Util.WHITE;
        if (output.Count > 0) {
            (string t, Color32 c) = output[output.Count - 1];
            if (c.Equals(color)) {
                t += s;
                output[output.Count - 1] = (t, c);
                OutputHandler?.Invoke(s, color);
                return;
            }
        }
        output.Add((s, color));
        OutputHandler?.Invoke(s, color);
    }

    public void Println() {
        if (output.Count > 0) {
            (string t, Color32 c) = output[output.Count - 1];
            t += "\n";
            output[output.Count - 1] = (t, c);
            OutputHandler?.Invoke("\n", c);
        } else {
            Color32 c = Util.WHITE;
            output.Add(("\n", c));
            OutputHandler?.Invoke("\n", c);
        }
    }

    public void Println(string s, Color32? col = null) {
        Print(s + "\n", col);
    }

    public void SetScreen(Cell[,] screen) {
        this.screen = screen;
        fullscreen = true;
        ScreenHandler?.Invoke(screen);
    }

    public void UnsetScreen() {
        fullscreen = false;
        ScreenHandler?.Invoke(null);
    }
}
