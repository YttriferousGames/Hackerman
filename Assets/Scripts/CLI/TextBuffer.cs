using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System;

public interface TextOut {
    int width { get; set; }
    int height { get; set; }
    void Print(string text, Color32? col = null);
    void Println(string text = null, Color32? col = null);
    // Very hacky, just to help with refactoring for now
    [System.Obsolete]
    void SetLine(string str, Color32? col = null);
}

// TODO might be nice if there was a "global" one per system so all programs can do cool stuff
// It would have to be exposed in ProgData, which could be messy
public class TextBuffer : TextOut {
    public int width {
        get => _width;
        set {
            _width = value;
            screenLines = recalcScreenLines();
            int lines = Lines(currentLine);
            if (lines <= screenScroll) {
                screenScroll = lines - 1;
            }
            _needsRedraw = true;
        }
    }
    public int height {
        get => _height;
        set {
            _height = value;
            _needsRedraw = true;
        }
    }
    public bool needsRedraw { get => _needsRedraw; }
    private bool _needsRedraw = true;
    private int _width;
    private int _height;
    // Scroll is relative to top, that is, 0 = first line
    // This line will be displayed at bottom of screen (assuming screen is full)

    // Number of text lines scrolled down
    private int textScroll = 0;
    // Number of screen lines offset from textScroll
    private int screenScroll = 0;
    // Total number of text lines
    private int textLines { get => buf.Count; }
    // Total number of screen lines
    private int screenLines = 1;
    private Cell[] currentLine {
        get => textScroll < buf.Count ? buf[textScroll] : null;
        set => buf[textScroll] = value;
    }

    private List<Cell[]> buf;

    private int Lines(Cell[] c) {
        return (c == null ? 0 : c.Length / width) + 1;
    }

    private int recalcScreenLines() {
        int l = 0;
        for (int i = 0; i < buf.Count; i++) {
            l += Lines(buf[i]);
        }
        if (l == 0)
            l = 1;
        return l;
    }

    private void SanityCheck() {
        // Assert.AreEqual(screenLines, recalcScreenLines());
    }

    public TextBuffer(int width, int height) {
        this._width = width;
        this._height = height;
        buf = new List<Cell[]>();
    }

    public bool IsBottom() {
        return (textScroll + 1 >= textLines || textLines == 0) &&
               screenScroll + 1 >= Lines(currentLine);
    }

    public void GotoBottom() {
        textScroll = textLines == 0 ? 0 : textLines - 1;
        screenScroll = Lines(currentLine) - 1;
        Assert.IsTrue(IsBottom());
    }

    public void Print(string str, Color32? col = null) {
        if (str.Length == 0) {
            return;
        }
        _needsRedraw = true;
        bool bottom = IsBottom();
        string[] lines = str.SplitNewlines();
        for (uint i = 0; i < lines.Length; i++) {
            Cell[] line = Cell.CreateArray(lines[i], col);

            if (buf.Count != 0 && i == 0) {
                int idx = buf.Count - 1;
                Cell[] other = buf[idx].Concat(line);
                int next = Lines(other) - Lines(buf[idx]);
                buf[idx] = other;
                screenLines += next;
            } else {
                if (buf.Count == 0) {
                    screenLines = Lines(line);
                } else {
                    screenLines += Lines(line);
                }

                buf.Add(line);
            }
            SanityCheck();
        }
        if (bottom) {
            GotoBottom();
        }
    }

    public void Println(string text = null, Color32? col = null) {
        Print(text != null ? text + '\n' : "\n", col);
    }

    // Very hacky, just to help with refactoring for now
    [System.Obsolete]
    public void SetLine(string str, Color32? col = null) {
        if (buf.Count > 0) {
            screenLines -= Lines(buf[buf.Count - 1]);
            buf.RemoveAt(buf.Count - 1);
            if (IsBottom())
                GotoBottom();
            SanityCheck();
            Print('\n' + str, col);
        } else {
            Print(str, col);
        }
    }

    // Simplify the crud
    public Cell[,] Layout(bool drawCursor = true) {
        Cell[,] o = new Cell[width, height];
        if (screenLines <= height) {
            // Ignore scroll, draw from top
            int l = 0;

            for (int n = 0; n < buf.Count; n++) {
                Cell[] line = buf[n];
                for (int x1 = 0; x1 < line.Length; x1 += width) {
                    for (int i = 0; i < width; i++) {
                        o[i, l] = i + x1 < line.Length ? line[i + x1] : new Cell();
                    }
                    l++;
                }
            }

            while (l < height) {
                for (int i = 0; i < width; i++) {
                    o[i, l] = new Cell();
                }
                l++;
            }
        } else {
            int l = height - 1;
            Cell[] g = currentLine;
            int sc = screenScroll;
            for (int n = textScroll - 1; l >= 0 && n >= 0; n--) {
                while (sc >= 0 && l >= 0) {
                    Cell[] f = g[(width * sc)..Math.Min(g.Length, width * (sc + 1))];
                    for (int i = 0; i < width; i++) {
                        o[i, l] = i < f.Length ? f[i] : new Cell();
                    }
                    sc--;
                    l--;
                }

                g = n >= 0 ? buf[n] : null;
                sc = n >= 0 ? Lines(g) - 1 : -1;
            }
        }
        _needsRedraw = false;
        return o;
    }
}