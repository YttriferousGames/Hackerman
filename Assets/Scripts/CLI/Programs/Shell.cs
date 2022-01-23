using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>The shell of the computer (the CLI you use to run commands)</summary>
public class Shell : Exe {
    private class ShellGuts {
        private List<string> history = new List<string>();
        private Prog current = null;
        private string buffer = "";

        public bool UpdateProg(ProgAPI d) {
            if (current != null) {
                try {
                    current.Input(d.input);
                    if (d.close)
                        current.Close();
                    if (current.Update() != null) {
                        current = null;
                        ProgFinish(d);
                    } else {
                        return true;
                    }
                } catch (System.Exception e) {
                    PrintError(d, e);
                }
            }
            return false;
        }

        private Prog RunExe(ProgAPI d, string cmd) {
            history.Add(cmd);
            string[] args = cmd.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            Exe prog = null;
            if (args.Length > 0) {
                prog = d.sys.GetProgram(args[0]);
                if (prog is Shell s) {
                    Motd(d);
                    return null;
                }
            }
            if (prog != null) {
                Prog p = null;
                try {
                    p = prog.Start(d.sys, d.Out, args[1..]);
                    if (p.Update() != null)
                        p = null;
                } catch (System.Exception e) {
                    PrintError(d, e);
                }
                if (p != null)
                    return p;
            } else if (args.Length > 0) {
                d.Out.Print(args[0] + " is not a command (try ", Color.red);
                d.Out.Print("ls /bin/", Color.magenta);
                d.Out.Println(")", Color.red);
            }
            ProgFinish(d);
            return null;
        }

        public void HandleInput(ProgAPI d) {
            string[] inp = d.input.SplitNewlines();
            if (inp.Length > 0) {
                if (inp[0].Length > 0) {
                    buffer += inp[0];
                    buffer = FixBackspace(buffer);
                    // JANK
                    d.Out.SetLine("$ " + buffer, Color.green);
                }
                if (inp.Length > 1) {
                    d.Out.Println();
                    current = RunExe(d, buffer);
                    buffer = "";
                }
            }
        }

        public static void Motd(ProgAPI d) {
            d.Out.Println("[Alibaba Intelligence OS v0.1]", Color.cyan);
            ProgFinish(d);
        }

        private static void ProgFinish(ProgAPI d) {
            d.Out.ScreenOverride = null;
            d.Out.Print("$ ", Color.green);
        }

        private static void PrintError(ProgAPI d, System.Exception e) {
            UnityEngine.Debug.LogException(e);
            d.Out.Println(e.GetType().Name + ": " + e.Message, Color.red);
        }
    }

    private static Regex regex = new Regex(@"(?<L>[^\b])+(?<R-L>[\b])+(?(L)(?!))");
    public Shell(NodeFlags flags = NodeFlags.None) : base("sh", flags) {}

    private static string FixBackspace(string s) {
        return regex.Replace(s, "").Replace("\b", "");
    }

    protected override IEnumerable<int?> Run(ProgAPI d) {
        ShellGuts guts = new ShellGuts();
        ShellGuts.Motd(d);
        while (true) {
            bool progRunning = guts.UpdateProg(d);
            if (!progRunning && d.input.Length > 0) {
                guts.HandleInput(d);
            }
            d.close = false;
            yield return null;
        }
    }
}
