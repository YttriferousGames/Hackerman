using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBitFont", menuName = "BitFont", order = 1)]
public class BitFont : ScriptableObject {
    public Texture2D atlas;
    public bool flipY = false;
    private const int pad = 16 * 2;
    public int charWidth { get => (atlas.width - pad) >> 4; }
    public int charHeight { get => (atlas.height - pad) >> 4; }

    public BitFont(Texture2D fontAtlas, bool flipAtlasY = false) {
        atlas = fontAtlas;
        flipY = flipAtlasY;
        OnValidate();
    }

    private void OnValidate() {
        if (((atlas.width - pad) & 0b1111) != 0 || ((atlas.height - pad) & 0b1111) != 0 ||
            atlas.width < 16 + pad || atlas.height < 16 + pad) {
            throw new ArgumentException("Font atlas must be a padded 16x16 grid of characters!");
        }
    }

    // Gets pixel coordinates of specific character
    private ((int, int), (int, int)) GetCharBounds(byte c) {
        int w = charWidth;
        int h = charHeight;
        int x = 1 + (w + 2) * (c % 16);
        int y = 1 + (h + 2) * (flipY ? (15 - c / 16) : (c / 16));
        return ((x, y), (x + w, y + h));
    }

    private void DrawChar(Texture2D dst, byte c, int dstX, int dstY) {
        ((int srcX, int srcY), (_, _)) = GetCharBounds(c);
        Graphics.CopyTexture(atlas, 0, 0, srcX, srcY, charWidth, charHeight, dst, 0, 0, dstX, dstY);
    }

    // Gets UV coordinates of specific character
    public ((float, float), (float, float)) GetCharUV(byte c) {
        float w = atlas.width;
        float h = atlas.height;
        ((float a, float b), (float x, float y)) = GetCharBounds(c);
        return ((a / w, b / h), (x / w, y / h));
    }

    public Mesh GenMesh(IEnumerable<(Vector2, Color32, byte)> chars, Vector2 size, Mesh m = null) {
        List<Vector3> verts = new List<Vector3>();
        List<Color32> cols = new List<Color32>();
        List<Vector2> uvs = new List<Vector2>();
        List<ushort> ind = new List<ushort>();

        ushort n = 0;
        foreach ((Vector2 pos, Color32 col, byte c) in chars) {
            verts.Add(new Vector3(pos.x, pos.y, 0.0f));
            verts.Add(new Vector3(pos.x + size.x, pos.y, 0.0f));
            verts.Add(new Vector3(pos.x + size.x, pos.y + size.y, 0.0f));
            verts.Add(new Vector3(pos.x, pos.y + size.y, 0.0f));

            for (byte j = 0; j < 4; j++) cols.Add(col);

            ((float srcX, float srcY), (float outerX, float outerY)) = GetCharUV(c);

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

        if (m == null)
            m = new Mesh();
        m.vertices = verts.ToArray();
        m.uv = uvs.ToArray();
        m.SetColors(cols);
        m.SetTriangles(ind, 0, true, 0);
        return m;
    }
}
