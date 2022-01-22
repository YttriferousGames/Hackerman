using UnityEngine.Rendering;

/// <summary>Data of a single character/grid cell rendered</summary>
public struct Cell {
    /// <summary>The encoded value of the character</summary>
    public byte chr;
    /// <summary>Color the character is rendered as</summary>
    public Color32 col;

    private Cell(byte character, Color32? color = null) {
        chr = character;
        col = color ?? Util.WHITE;
    }

    public Cell(char? c = null, Color32? color = null) {
        if (c is char ch) {
            byte[] data = BitFont.codePage.GetBytes(new char[] { ch });
            Assert.AreEqual(data.Length, 1);
            chr = data[0];
        } else {
            chr = (byte)' ';
        }
        col = color ?? Util.WHITE;
    }

    /// <summary>Creates an array of <see cref="Cell"/>s from a string</summary>
    public static Cell[] CreateArray(string text, Color32? color = null) {
        byte[] bytes = BitFont.codePage.GetBytes(text);
        Cell[] data = new Cell[bytes.Length];
        for (int i = 0; i < bytes.Length; i++) {
            data[i] = new Cell(bytes[i], color);
        }
        return data;
    }
}

/// <summary>Terminal rendering code</summary>
[RequireComponent(typeof(MeshRenderer))]
public class TermRenderer : MonoBehaviour {
    /// <summary>Font used for drawing to the screen</summary>
    public BitFont font;
    /// <summary>Space between each character when drawn to screen (px)</summary>
    public int pad = 1;

    private RenderTexture tex = null;
    private CommandBuffer cmd;
    [SerializeField]
    private Material mat;
    private MeshRenderer rend;
    private Mesh m = null;

    // TODO hacky fix to upside down text
    private IEnumerable<(Vector2, Color32, byte)> Cells(float boundX, float boundY,
                                                        Cell[,] layout) {
        int iMax = layout.GetLength(1);
        for (int i = 0; i < iMax; i++) {
            for (int j = 0; j < layout.GetLength(0); j++) {
                float x = ((font.charWidth + pad) * j + pad) / boundX;
                float y = ((font.charHeight + pad) * i + pad) / boundY;
                // Hack here
                Cell c = layout[j, iMax - i - 1];
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

    private void Awake() {
        rend = GetComponent<MeshRenderer>();

        cmd = new CommandBuffer();
    }

    /// <summary>Draws terminal contents to <see cref="RenderTexture"/></summary>
    public void Render(Cell[,] layout) {
        (int rX, int rY) = TexRes(layout);
        if (tex == null || tex.width != rX || tex.height != rY) {
            if (tex != null)
                tex.Release();
            tex = new RenderTexture(rX, rY, 0);
            rend.material.SetTexture("_MainTex", tex);
            rend.material.SetTexture("_EmissionMap", tex);
        }
        m?.Clear();
        m = GenMesh(rX / 2f, rY / 2f, layout, m);
        cmd.Clear();
        cmd.SetRenderTarget(tex);
        cmd.ClearRenderTarget(true, true, Color.black);
        cmd.SetViewProjectionMatrices(Matrix4x4.Translate(new Vector3(-1f, -1f, 0f)),
                                      Matrix4x4.identity);
        cmd.DrawMesh(m, Matrix4x4.identity, mat, 0, 0);
        Graphics.ExecuteCommandBuffer(cmd);
    }
}
