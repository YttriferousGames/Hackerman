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

public class TermRenderer : MonoBehaviour {
    public BitFont font;
    public int width = 80;
    public int height = 36;
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
    public bool demo = false;

    private RenderTexture tex;
    private CommandBuffer cmd;
    public Material mat;

    // TODO anchor to top when buffer is not full
    // TODO smooth scroll
    // TODO fix scroll to actually stay when new lines are added
    private IEnumerable<(Cell, int, int)> Cells() {
        for (int i = 0; i < height; i++) {
            List<Cell> line = new List<Cell>();
            int numFullLines = buffer.Count / width;
            int numLines = (buffer.Count - 1) / width + 1;
            int startOfLine = (numFullLines - scroll - i) * width;
            if (startOfLine >= 0 && startOfLine < buffer.Count) {
                line = buffer.GetRange(startOfLine, Math.Min(width, buffer.Count - startOfLine));
            }
            for (int j = 0; j < line.Count; j++) {
                yield return (line[j], j, i);
            }
        }
    }

    private Mesh GenMesh(float boundX, float boundY, Mesh mesh = null) {
        if (mesh == null)
            mesh = new Mesh();

        List<Vector3> verts = new List<Vector3>();
        List<Color32> cols = new List<Color32>();
        List<Vector2> uvs = new List<Vector2>();
        List<ushort> ind = new List<ushort>();

        ushort n = 0;
        foreach ((Cell c, int x, int y) in Cells()) {
            if (c.chr == (byte)' ')
                continue;
            float dstX = ((font.charWidth + pad) * x + pad) / boundX;
            float dstY = ((font.charHeight + pad) * y + pad) / boundY;
            float offsetX = font.charWidth / boundX;
            float offsetY = font.charHeight / boundY;

            verts.Add(new Vector3(dstX, dstY, 0.0f));
            verts.Add(new Vector3(dstX + offsetX, dstY, 0.0f));
            verts.Add(new Vector3(dstX + offsetX, dstY + offsetY, 0.0f));
            verts.Add(new Vector3(dstX, dstY + offsetY, 0.0f));

            // TODO consider texture instead?
            for (byte j = 0; j < 4; j++) cols.Add(c.col);

            ((float srcX, float srcY), (float outerX, float outerY)) = font.GetCharUV(c.chr);

            uvs.Add(new Vector2(srcX, srcY));
            uvs.Add(new Vector2(outerX, srcY));
            uvs.Add(new Vector2(outerX, outerY));
            uvs.Add(new Vector2(srcX, outerY));

            ind.Add(n);
            ind.Add((ushort)(n + 3));
            ind.Add((ushort)(n + 2));
            ind.Add(n);
            ind.Add((ushort)(n + 2));
            ind.Add((ushort)(n + 1));
            n += 4;
        }
        mesh.vertices = verts.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.SetColors(cols);
        mesh.SetTriangles(ind, 0, true, 0);

        return mesh;
    }

    public void Print() {}

    // TODO handle \n
    public void Print(string s, Nullable<Color32> col = null) {
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

    public void Println(string s, Nullable<Color32> col = null) {
        Print(s, col);
        Println();
        needsRender = true;
    }

    // TODO this falls apart beyond a single line (obviously)
    public void SetLine(string s, Nullable<Color32> col = null) {
        buffer.RemoveRange((buffer.Count / width) * width,
                           buffer.Count - (buffer.Count / width) * width);
        Print(s, col);
    }

    // Start is called before the first frame update
    void Start() {
        font = BitFont.ProggyTiny;
        Renderer r = GetComponent<Renderer>();

        cmd = new CommandBuffer();
        tex = new RenderTexture((font.charWidth + pad) * width + pad,
                                (font.charHeight + pad) * height + pad, 0);

        if (demo) {
            Println("Hello, world!");
            Println("Fortnite gaming");
        }
        Update();
        r.material.SetTexture("_MainTex", tex);
    }

    // Update is called once per frame
    void Update() {
        const int step = 4;
        if (demo && Time.frameCount % step == 0) {
            int frame = Time.frameCount / step;
            float n = 19f * (Mathf.Sin(frame / 5f) + 1f);
            Println();
            for (float j = 0f; j < n; j++) {
                Print("#", new Color(n / 10, j / n, 0.8f));
            }
        }
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
