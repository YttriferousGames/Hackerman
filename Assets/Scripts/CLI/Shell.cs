using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System;

[RequireComponent(typeof(Sys))]
public class Shell : MonoBehaviour {
    // Start is called before the first frame update
    private Sys s = null;
    private Exe current = null;
    private string buffer = "";
    private static Regex regex = new Regex(@"(?<L>[^\b])+(?<R-L>[\b])+(?(L)(?!))");

    private Exe Run(string cmd) {
        // s.Println("$ " + cmd, Color.green);
        string[] args = cmd.Split(' ');
        Exe prog = s.GetProgram(args[0]);
        bool done = true;
        try {
            done = prog.Start(args[1..]);
        } catch (System.Exception e) {
            s.Println(e.ToString(), Color.red);
        }
        if (done) {
            ProgFinish();
            return null;
        } else {
            return prog;
        }
    }

    void ProgFinish() {
        s.Print("$ ", Color.green);
    }

    static string FixBackspace(string s) {
        return regex.Replace(s, "").Replace("\b", "");
    }

    public void HandleInput() {
        if (current == null) {
            string[] inp = Input.inputString.Split(new string[] { "\r\n", "\n", "\r" },
                                                   StringSplitOptions.None);
            if (inp.Length > 0) {
                if (inp[0].Length > 0) {
                    buffer += inp[0];
                    buffer = FixBackspace(buffer);
                    // JANK
                    s.disp.SetLine("$ " + buffer, Color.green);
                }
                if (inp.Length > 1) {
                    s.Println();
                    current = Run(buffer);
                    buffer = "";
                }
            }
        }
    }

    void Start() {
        s = GetComponent<Sys>();
        s.Println("[Alibaba Intelligence OS v0.1]", Color.cyan);
        ProgFinish();
    }

    // Update is called once per frame
    void Update() {
        if (current != null) {
            if (current.Update()) {
                current = null;
                ProgFinish();
                return;
            }
        }
    }
}
