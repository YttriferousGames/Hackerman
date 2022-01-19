// The shell of the computer (the commandline you run commands at)

using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// TODO dispense with the pile of hacks
// Get this done the RIGHT way
public class Shell : Exe {
    private class ShellGuts {
        public Prog current = null;
        public string buffer = "";

        public int width = 40;
        public int height = 18;
        public bool needsRender = true;
        public Cell[,] screenOverride = null;
        public TextBuffer tb;

        private void Print(string s, Color32? col = null) {
            tb.Append(s, col);
        }

        private void OnScreen(Cell[,] screen) {
            bool changed = screenOverride != screen;
            screenOverride = screen;
            needsRender = changed;
        }

        public void Println(string s = null, Color32? col = null) {
            Print(s != null ? s + '\n' : "\n", col);
        }

        public Prog RunExe(ProgData d, string cmd) {
            string[] args = cmd.Split(' ');
            Exe prog = d.sys.GetProgram(args[0]);
            if (prog is Shell s) {
                Motd();
                return null;
            }
            Prog p = null;
            bool done = true;
            if (prog != null) {
                try {
                    p = prog.Start(d.sys, args[1..]);
                    p.OnOutput += (string s, Color32 col) => Print(s, col);
                    p.OnScreen += OnScreen;
                    done = p.Update() != null;
                } catch (System.Exception e) {
                    Println(e.Message, Color.red);
                    UnityEngine.Debug.LogException(e);
                }
            } else {
                Print(args[0] + " is not a command (try ", Color.red);
                Print("ls /bin/", Color.magenta);
                Println(")", Color.red);
            }
            if (done) {
                ProgFinish();
                return null;
            } else {
                return p;
            }
        }

        public void Motd() {
            Println("[Alibaba Intelligence OS v0.1]", Color.cyan);
            ProgFinish();
        }

        public void ProgFinish() {
            Print("$ ", Color.green);
        }

        public ShellGuts() {
            tb = new TextBuffer(width, height);
        }
    }

    private static Regex regex = new Regex(@"(?<L>[^\b])+(?<R-L>[\b])+(?(L)(?!))");
    public Shell(NodeFlags flags = NodeFlags.None) : base("sh", flags) {}

    private static string FixBackspace(string s) {
        return regex.Replace(s, "").Replace("\b", "");
    }

    protected override IEnumerable<int?> Run(ProgData d) {
        ShellGuts guts = new ShellGuts();
        guts.Motd();
        while (true) {
            if (guts.current != null) {
                try {
                    guts.current.Input(d.input);
                    if (d.close)
                        guts.current.Close();
                    if (guts.current.Update() != null) {
                        guts.current = null;
                        guts.ProgFinish();
                    }
                } catch (System.Exception e) {
                    guts.Println(e.Message, Color.red);
                    UnityEngine.Debug.LogException(e);
                }
            } else if (d.input.Length > 0) {
                string[] inp = d.input.SplitNewlines();
                if (inp.Length > 0) {
                    if (inp[0].Length > 0) {
                        guts.buffer += inp[0];
                        guts.buffer = FixBackspace(guts.buffer);
                        // JANK
                        guts.tb.SetLine("$ " + guts.buffer, Color.green);
                    }
                    if (inp.Length > 1) {
                        guts.Println();
                        guts.current = guts.RunExe(d, guts.buffer);
                        guts.buffer = "";
                    }
                }
            }
            // TODO not quite right per se
            if (guts.tb.needsRedraw || guts.needsRender) {
                if (guts.screenOverride == null) {
                    d.SetScreen(guts.tb.Layout());
                } else {
                    d.SetScreen(guts.screenOverride);
                }
                guts.needsRender = false;
            }
            yield return null;
        }
    }
}
