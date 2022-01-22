using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;

using System.Text;

/// <summary>Contains font data as used in 3D text/terminal</summary>
[CreateAssetMenu(fileName = "NewBitFont", menuName = "BitFont", order = 1)]
public class BitFont : ScriptableObject {
    [SerializeField]
    private Texture2D atlas;
    [SerializeField]
    private bool flipY = false;
    private const int pad = 16 * 2;
    /// <summary>Width of character (in pixels)</summary>
    public int charWidth { get => (atlas.width - pad) >> 4; }
    /// <summary>Height of character (in pixels)</summary>
    public int charHeight { get => (atlas.height - pad) >> 4; }
    // Technically 1252 is correct but that is unavailable
    // Static because text data should be interoperable between fonts
    // It might be worth just storing Cells as chars if Unicode support is added
    /// <summary>Character encoding of font</summary>
    public static readonly Encoding codePage = GenerateEncoding();
    /// <summary>Texture atlas of font</summary>
    public Texture2D Atlas { get => atlas; }

    public BitFont(Texture2D fontAtlas, bool flipAtlasY = false) {
        atlas = fontAtlas;
        flipY = flipAtlasY;
        OnValidate();
    }

    private static Encoding GenerateEncoding() {
        Encoding e = (Encoding)System.Text.Encoding.ASCII.Clone();
        e.EncoderFallback = new EncoderReplacementFallback("\0");
        return e;
    }

    private void OnValidate() {
        if (((atlas.width - pad) & 0b1111) != 0 || ((atlas.height - pad) & 0b1111) != 0 ||
            atlas.width < 16 + pad || atlas.height < 16 + pad) {
            throw new ArgumentException("Font atlas must be a padded 16x16 grid of characters!");
        }
    }

    /// <summary>Gets pixel coordinates of specific character</summary>
    private ((int, int), (int, int)) GetCharBounds(byte c, bool includePad = false) {
        int w, h, x, y;
        if (includePad) {
            w = charWidth + 2;
            h = charHeight + 2;
            x = w * (c % 16);
            y = h * (flipY ? (15 - c / 16) : (c / 16));
        } else {
            w = charWidth;
            h = charHeight;
            x = 1 + (w + 2) * (c % 16);
            y = 1 + (h + 2) * (flipY ? (15 - c / 16) : (c / 16));
        }
        return ((x, y), (x + w, y + h));
    }

    private void DrawChar(Texture2D dst, byte c, int dstX, int dstY) {
        ((int srcX, int srcY), (_, _)) = GetCharBounds(c);
        Graphics.CopyTexture(atlas, 0, 0, srcX, srcY, charWidth, charHeight, dst, 0, 0, dstX, dstY);
    }

    /// <summary>Gets UV coordinates of specific character</summary>
    private ((float, float), (float, float)) GetCharUV(byte c, bool includePad) {
        float w = atlas.width;
        float h = atlas.height;
        ((float a, float b), (float x, float y)) = GetCharBounds(c, includePad);
        return ((a / w, b / h), (x / w, y / h));
    }

    /// <summary>Generates mesh of text from generator of cell positions</summary>
    public Mesh GenMesh(IEnumerable<(Vector2, Color32, byte)> chars, Vector2 size, Mesh m = null,
                        bool pad = false) {
        List<Vector3> verts = new List<Vector3>();
        List<Color32> cols = new List<Color32>();
        List<Vector2> uvs = new List<Vector2>();
        List<ushort> ind = new List<ushort>();

        Vector2 offset = Vector2.zero;
        if (pad) {
            size = (size * new Vector2(charWidth + 2f, charHeight + 2f)) /
                   new Vector2(charWidth, charHeight);
            offset = -size / new Vector2(charWidth + 2, charHeight + 2);
        }

        ushort n = 0;
        foreach ((Vector2 p, Color32 col, byte c) in chars) {
            Vector2 pos = p;
            if (pad)
                pos += offset;
            verts.Add(new Vector3(pos.x, pos.y, 0.0f));
            verts.Add(new Vector3(pos.x + size.x, pos.y, 0.0f));
            verts.Add(new Vector3(pos.x + size.x, pos.y + size.y, 0.0f));
            verts.Add(new Vector3(pos.x, pos.y + size.y, 0.0f));

            for (byte j = 0; j < 4; j++) cols.Add(col);

            ((float srcX, float srcY), (float outerX, float outerY)) = GetCharUV(c, pad);

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
