using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

// Idea: unprintable characters (like \0) can be animated garbled text (like in Minecraft).

// TODO abstract this code more:
// - For example, scroll layout can be separate from screen contents
//   (as would be used by shell, less, text editor, title screen etc)
// - Perhaps scroll can be tucked into the shell's code, not the term
//   renderer?
// - The mesh code would be cool for UI elements so abstract that out too

public class TermRenderer : MonoBehaviour {
    public BitFont font;
    public int width = 40;
    public int height = 18;
    // Space between each character when drawn to screen (px)
    public int pad = 1;
    public int scroll = 0;
    // public bool flipFbX = false;
    // public bool flipFbY = false;
    private List<Cell> buffer = new List<Cell>();
    private bool needsRender = true;
    // For separating the edge case of having a full row then newlining, or newlining twice, being
    // different
    private bool hasNewlined = false;

    private RenderTexture tex;
    private CommandBuffer cmd;
    public Material mat;

    private struct Cell {
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

    // TODO anchor to top when buffer is not full
    // TODO smooth scroll
    // TODO fix scroll to actually stay when new lines are added
    private IEnumerable<(Vector2, Color32, byte)> Cells(float boundX, float boundY) {
        for (int i = 0; i < height; i++) {
            List<Cell> line = new List<Cell>();
            int numFullLines = buffer.Count / width;
            int numLines = (buffer.Count - 1) / width + 1;
            int startOfLine = (numFullLines - scroll - i) * width;
            if (startOfLine >= 0 && startOfLine < buffer.Count) {
                line = buffer.GetRange(startOfLine, Math.Min(width, buffer.Count - startOfLine));
            }
            for (int j = 0; j < line.Count; j++) {
                float x = ((font.charWidth + pad) * j + pad) / boundX;
                float y = ((font.charHeight + pad) * i + pad) / boundY;
                Cell c = line[j];
                if (c.chr == (byte)' ')
                    continue;
                yield return (new Vector2(x, y), c.col, c.chr);
            }
        }
    }

    private Mesh GenMesh(float boundX, float boundY, Mesh mesh = null) {
        return font.GenMesh(Cells(boundX, boundY),
                            new Vector2(font.charWidth / boundX, font.charHeight / boundY), mesh);
    }

    public void Print() {}

    // TODO handle \n
    public void Print(string s, Color32? col = null) {
        string[] lines = s.Split('\n');
        for (int i = 0; i < lines.Length; i++) {
            buffer.AddRange(
                System.Text.Encoding.ASCII.GetBytes(lines[i]).Select(b => new Cell(b, col)));
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
    void Start() {
        Renderer r = GetComponent<Renderer>();

        cmd = new CommandBuffer();
        tex = new RenderTexture((font.charWidth + pad) * width + pad,
                                (font.charHeight + pad) * height + pad, 0);

        Update();
        r.material.SetTexture("_MainTex", tex);
        r.material.SetTexture("_EmissionMap", tex);
    }

    // Update is called once per frame
    void Update() {
        if (needsRender) {
            Mesh m = GenMesh(tex.width / 2f, tex.height / 2f);
            cmd.Clear();
            cmd.SetRenderTarget(tex);
            cmd.ClearRenderTarget(true, true, Color.black);
            cmd.SetViewProjectionMatrices(Matrix4x4.Translate(new Vector3(-1f, -1f, 0f)),
                                          Matrix4x4.identity);
            cmd.DrawMesh(m, Matrix4x4.identity, mat, 0, 0);
            Graphics.ExecuteCommandBuffer(cmd);
            needsRender = false;
        }
    }
}
