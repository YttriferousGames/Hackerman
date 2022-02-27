using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>The glue between input and the Shell</summary>
[RequireComponent(typeof(Sys), typeof(TermRenderer))]
public class SysInterface : MonoBehaviour {
    private Sys s;
    private Prog sh;
    [SerializeField]
    private int width = TextOut.DEFAULT_WIDTH;
    [SerializeField]
    private int height = TextOut.DEFAULT_HEIGHT;
    private TextBuffer tb = new TextBuffer(TextOut.DEFAULT_WIDTH, TextOut.DEFAULT_HEIGHT);
    private TermRenderer rend;
    [SerializeField]
    private AudioClip status;
    private AudioSource player = null;
    private bool handle = false;

    private void Zoom(int step) {
        bool shouldMul = step < 0;
        int v = 1 << (shouldMul ? -step : step);
        if (!shouldMul) {
            if (tb.width % v == 0 && tb.height % v == 0) {
                tb.width /= v;
                tb.height /= v;
            }
        } else {
            tb.width *= v;
            tb.height *= v;
        }
        width = tb.width;
        height = tb.height;
    }

    /// <summary>Should be called to handle system input</summary>
    public void HandleInput(bool handle) {
        this.handle = handle;
    }

    private void Awake() {
        s = GetComponent<Sys>();
        rend = GetComponent<TermRenderer>();
        player = GetComponent<AudioSource>();
    }

    private void Start() {
        Shell shell = s.GetProgram<Shell>("sh");
        sh = shell.Start(s, tb);
        Update();
    }

    private void PlayAudio(AudioClip c) {
        if (player != null && c != null) {
            player.PlayOneShot(c);
        }
    }

    private void Update() {
        sh.canInput =
            handle && !Keyboard.current.ctrlKey.isPressed && !Keyboard.current.altKey.isPressed;
        // TODO this code should be moved into respective programs
        if (handle) {
            if (Keyboard.current.ctrlKey.isPressed) {
                if (Keyboard.current.cKey.wasPressedThisFrame) {
                    sh.Close();
                    PlayAudio(status);
                    return;
                } else if (Keyboard.current.equalsKey.wasPressedThisFrame) {
                    Zoom(1);
                    return;
                } else if (Keyboard.current.minusKey.wasPressedThisFrame) {
                    Zoom(-1);
                    return;
                }
            }
        }
        tb.width = width;
        tb.height = height;
        sh.Update();
        if (tb.needsRedraw) {
            rend.Render(tb.Layout());
        }
        handle = false;
    }
}
