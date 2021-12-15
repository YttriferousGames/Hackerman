using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// TODO abstract this code more:
// - For example, scroll layout can be separate from screen contents
//   (as would be used by shell, less, text editor, title screen etc)
// - Perhaps scroll can be tucked into the shell's code, not the term
//   renderer?
// - The mesh code would be cool for UI elements so abstract that out too

public struct Cell {
    public byte chr;
    public Color32 col;

    public Cell(byte character, Color32? color = null) {
        chr = character;
        if (color is Color32 c) {
            col = c;
        } else {
            col = new Color32(255, 255, 255, 255);
        }
    }
}

[RequireComponent(typeof(TermRenderer))]
public class TermLayout : MonoBehaviour {
    public int width = 40;
    public int height = 18;
    public int scroll = 0;
    // public bool flipFbX = false;
    // public bool flipFbY = false;
    private List<Cell> buffer = new List<Cell>();
    private bool needsRender = true;
    public bool overrideRender = false;
    // For separating the edge case of having a full row then newlining, or newlining twice, being
    // different
    private bool hasNewlined = false;
    [SerializeField]
    private TermRenderer rend;

    // TODO anchor to top when buffer is not full
    // TODO smooth scroll
    // TODO fix scroll to actually stay when new lines are added
    private Cell[,] LayoutText() {
        Cell[,] o = new Cell[width, height];
        for (int i = 0; i < height; i++) {
            List<Cell> line = new List<Cell>();
            int numFullLines = buffer.Count / width;
            int numLines = (buffer.Count - 1) / width + 1;
            int startOfLine = (numFullLines - scroll - i) * width;
            if (startOfLine >= 0 && startOfLine < buffer.Count) {
                line = buffer.GetRange(startOfLine, Math.Min(width, buffer.Count - startOfLine));
            }
            for (int j = 0; j < width; j++) {
                if (j < line.Count) {
                    o[j, i] = line[j];
                } else {
                    o[j, i] = new Cell((byte)' ', new Color32(255, 255, 255, 255));
                }
            }
        }
        return o;
    }

    public void Print() {}

    // TODO handle \n
    public void Print(string s, Color32? col = null) {
        string[] lines = s.Split('\n');
        for (int i = 0; i < lines.Length; i++) {
            buffer.AddRange(BitFont.codePage.GetBytes(lines[i]).Select(b => new Cell(b, col)));
            if (i < lines.Length - 1) { Println(); } hasNewlined = false; needsRender = true;
        }
    }

    public void Println() {
        int n = width - buffer.Count % width;
        if (n != width || hasNewlined) {
            buffer.AddRange(Enumerable.Repeat(new Cell((byte)' ', null), n));
        } else {
            hasNewlined = true;
        }
        needsRender = true;
    }

    public void Println(string s, Color32? col = null) {
        Print(s, col);
        Println();
        needsRender = true;
    }

    // TODO this falls apart beyond a single line (obviously)
    public void SetLine(string s, Color32? col = null) {
        buffer.RemoveRange((buffer.Count / width) * width,
                           buffer.Count - (buffer.Count / width) * width);
        Print(s, col);
    }

    // Start is called before the first frame update
    private void Start() {
        rend = GetComponent<TermRenderer>();

        Update();
    }

    public void Render(Cell[,] screen) {
        rend.Render(screen);
    }

    // Update is called once per frame
    private void Update() {
        if (!overrideRender && needsRender) {
            rend.Render(LayoutText());
            needsRender = false;
        }
    }
}
