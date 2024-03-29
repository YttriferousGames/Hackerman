using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>Horizontal alignment</summary>
public enum HAlign {
    LJustified,
    CJustified,
    RJustified,
}

/// <summary>3D text object</summary>
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BitText : MonoBehaviour {
    /// <summary>Font used</summary>
    public BitFont font;
    /// <summary>Color of text</summary>
    public Color col = Color.white;
    /// <summary>Text rendered</summary>
    [Multiline]
    public string text = "Hello, world!";
    public HAlign horizontalAlignment = HAlign.LJustified;
    /// <summary>Size multiplier of text</summary>
    public float size = 1f;
    /// <summary>Padding between each character</summary>
    public Vector2 pad = new Vector2(0.25f, 0.25f);
    private bool includePad = true;
#if UNITY_EDITOR
    private static string materialName = "Assets/Materials/Text/Text.mat";
#endif

    private MeshFilter mf = null;

    private IEnumerable<(Vector2, Color32, byte)> Cells() {
        // Character dimensions is (charWidth, size)
        float charWidth = (size * font.charWidth) / (float)font.charHeight;
        // Distance between start of two characters horizontally
        float hStride = charWidth + pad.x;
        // Distance between start of two characters vertically
        float vStride = -size - pad.y;

        string[] lines = text.SplitNewlines();
        for (int y = 0; y < lines.Length; y++) {
            byte[] line = BitFont.codePage.GetBytes(lines[y]);
            float hOffset = 0f;
            if (horizontalAlignment == HAlign.CJustified) {
                hOffset = -(line.Length * charWidth + (line.Length - 1) * pad.x) / 2f;
            } else if (horizontalAlignment == HAlign.RJustified) {
                hOffset = -(line.Length * charWidth + (line.Length - 1) * pad.x);
            }
            for (int x = 0; x < line.Length; x++) {
                byte c = line[x];
                if (c == (byte)' ')
                    continue;
                yield return (new Vector2(hStride * x + hOffset, vStride * y), col, c);
            }
        }
    }

    private Mesh GenMesh(Mesh m = null) {
        Vector2 s = new Vector2((size * font.charWidth) / (float)font.charHeight, size);

        return font.GenMesh(Cells(), s, m, includePad);
    }

    private void Awake() {
        mf = GetComponent<MeshFilter>();
    }

    private void Start() {
        UpdateMesh();
    }

    /// <summary>Call whenever modifying values</summary>
    public void UpdateMesh() {
        if (this != null && mf != null) {
            mf.sharedMesh = GenMesh();
        }
    }

#if UNITY_EDITOR
    private void OnValidate() {
        EditorApplication.delayCall += UpdateMesh;
    }

    // TODO saner defaults
    // Maybe have meshfilter/meshrenderer be made at startup by bittext?
    [MenuItem("GameObject/3D Object/BitText")]
    private static void CreateCustomGameObject(MenuCommand menuCommand) {
        // Create a custom game object
        GameObject go = new GameObject("BitText");
        // Mesh filter, mesh renderer, bitText, correct material
        MeshFilter mf = go.AddComponent<MeshFilter>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.material = AssetDatabase.LoadAssetAtPath<Material>(materialName);
        BitText b = go.AddComponent<BitText>();
        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
#endif
}
