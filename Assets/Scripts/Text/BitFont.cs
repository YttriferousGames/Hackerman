using System;
using UnityEngine;

public class BitFont {
    public readonly Texture2D atlas;
    public readonly bool flipY = false;
    public int width { get => atlas.width; }
    public int height { get => atlas.height; }
    public int charWidth { get => atlas.width >> 4; }
    public int charHeight { get => atlas.height >> 4; }

    public BitFont(Texture2D fontAtlas, bool flipAtlasY = false) {
        if ((fontAtlas.width & 0b1111) != 0 || (fontAtlas.height & 0b1111) != 0) {
            throw new ArgumentException("Font atlas must be a 16x16 grid of characters!");
        }
        atlas = fontAtlas;
        flipY = flipAtlasY;
    }

    // Gets pixel coordinates of specific character
    public ((int, int), (int, int)) GetCharBounds(byte c) {
        int w = charWidth;
        int h = charHeight;
        int x = w * (c % 16);
        int y = h * (flipY ? (15 - c / 16) : (c / 16));
        return ((x, y), (x + w, y + h));
    }

    public void DrawChar(Texture2D dst, byte c, int dstX, int dstY) {
        ((int srcX, int srcY), (_, _)) = GetCharBounds(c);
        Graphics.CopyTexture(atlas, 0, 0, srcX, srcY, charWidth, charHeight, dst, 0, 0, dstX, dstY);
    }

    // Gets UV coordinates of specific character
    public ((float, float), (float, float)) GetCharUV(byte c) {
        float w = width;
        float h = height;
        ((float a, float b), (float x, float y)) = GetCharBounds(c);
        return ((a / w, b / h), (x / w, y / h));
    }

    private static BitFont _proggyTiny = null;

    public static BitFont ProggyTiny {
        get {
            return _proggyTiny ??
                   (_proggyTiny = new BitFont(Resources.Load<Texture2D>("ProggyTinySZ"), true));
        }
    }
}
