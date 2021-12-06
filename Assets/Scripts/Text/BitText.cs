using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HAlign {
    LJustified,
    CJustified,
    RJustified,
}

public enum VAlign {
    Top,
    Center,
    Bottom,
}

// TODO getters/setters for everything so mesh is up to date
public class BitText : MonoBehaviour {
    public BitFont font = BitFont.ProggyTiny;
    public Color col = Color.white;
    public string text = "Hello, world!";
    public HAlign horizontalAlignment = HAlign.CJustified;
    public VAlign verticalAlignment = VAlign.Top;
    public float size = 1f;
    public Vector2 padding = new Vector2(0.25f, 0.25f);
    private static readonly string[] newlines = { "\r\n", "\r", "\n" };

    private Mesh mesh {
        get => GetComponent<MeshFilter>().mesh;
        set => GetComponent<MeshFilter>().mesh = value;
    }

    private Mesh GenMesh() {
        string[] lines = text.Split(newlines, StringSplitOptions.None);
        foreach (string s in lines) {
            byte[] line = System.Text.Encoding.ASCII.GetBytes(s);
        }
        // TODO
        return null;
    }

    // Start is called before the first frame update
    void Start() {}

    // Update is called once per frame
    void Update() {}
}
