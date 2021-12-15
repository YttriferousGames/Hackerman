// Terminal rendering code

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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

[RequireComponent(typeof(MeshRenderer))]
public class TermRenderer : MonoBehaviour {
    public BitFont font;
    // Space between each character when drawn to screen (px)
    public int pad = 1;

    private RenderTexture tex = null;
    private CommandBuffer cmd;
    [SerializeField]
    private Material mat;
    private MeshRenderer rend;

    private IEnumerable<(Vector2, Color32, byte)> Cells(float boundX, float boundY,
                                                        Cell[,] layout) {
        for (int i = 0; i < layout.GetLength(1); i++) {
            for (int j = 0; j < layout.GetLength(0); j++) {
                float x = ((font.charWidth + pad) * j + pad) / boundX;
                float y = ((font.charHeight + pad) * i + pad) / boundY;
                Cell c = layout[j, i];
                if (c.chr == (byte)' ')
                    continue;
                yield return (new Vector2(x, y), c.col, c.chr);
            }
        }
    }

    private (int, int) TexRes(Cell[,] layout) {
        return ((font.charWidth + pad) * layout.GetLength(0) + pad,
                (font.charHeight + pad) * layout.GetLength(1) + pad);
    }

    private Mesh GenMesh(float boundX, float boundY, Cell[,] layout, Mesh mesh = null) {
        return font.GenMesh(Cells(boundX, boundY, layout),
                            new Vector2(font.charWidth / boundX, font.charHeight / boundY), mesh);
    }

    // Start is called before the first frame update
    private void Start() {
        rend = GetComponent<MeshRenderer>();

        cmd = new CommandBuffer();
    }

    public void Render(Cell[,] layout) {
        (int rX, int rY) = TexRes(layout);
        if (tex == null || tex.width != rX || tex.height != rY) {
            if (tex != null)
                tex.Release();
            tex = new RenderTexture(rX, rY, 0);
            rend.material.SetTexture("_MainTex", tex);
            rend.material.SetTexture("_EmissionMap", tex);
        }
        Mesh m = GenMesh(rX / 2f, rY / 2f, layout);
        cmd.Clear();
        cmd.SetRenderTarget(tex);
        cmd.ClearRenderTarget(true, true, Color.black);
        cmd.SetViewProjectionMatrices(Matrix4x4.Translate(new Vector3(-1f, -1f, 0f)),
                                      Matrix4x4.identity);
        cmd.DrawMesh(m, Matrix4x4.identity, mat, 0, 0);
        Graphics.ExecuteCommandBuffer(cmd);
    }
}
