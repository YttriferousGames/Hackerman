using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Shell : Exe {
    private Prog current = null;
    private string buffer = "";
    private static Regex regex = new Regex(@"(?<L>[^\b])+(?<R-L>[\b])+(?(L)(?!))");

    private Prog RunExe(ProgData d, string cmd) {
        // s.Println("$ " + cmd, Color.green);
        string[] args = cmd.Split(' ');
        Exe prog = d.sys.GetProgram(args[0]);
        if (prog is Shell s) {
            Motd(d);
            return null;
        }
        Prog p = null;
        bool done = true;
        if (prog != null) {
            try {
                p = prog.Start(d.sys, args[1..]);
                done = p.Update() != null;
            } catch (System.Exception e) {
                d.sys.Println(e.Message, Color.red);
                UnityEngine.Debug.LogException(e);
            }
        } else {
            d.sys.Print(args[0] + " is not a command (try ", Color.red);
            d.sys.Print("ls /bin/", Color.magenta);
            d.sys.Println(")", Color.red);
        }
        if (done) {
            ProgFinish(d);
            return null;
        } else {
            return p;
        }
    }

    void Motd(ProgData d) {
        d.sys.Println("[Alibaba Intelligence OS v0.1]", Color.cyan);
        ProgFinish(d);
    }

    void ProgFinish(ProgData d) {
        d.sys.Print("$ ", Color.green);
    }

    static string FixBackspace(string s) {
        return regex.Replace(s, "").Replace("\b", "");
    }

    public void HandleInput(ProgData d) {
        if (current == null) {
            string[] inp = Input.inputString.Split('\n');
            if (inp.Length > 0) {
                if (inp[0].Length > 0) {
                    buffer += inp[0];
                    buffer = FixBackspace(buffer);
                    // JANK
                    d.sys.disp.SetLine("$ " + buffer, Color.green);
                }
                if (inp.Length > 1) {
                    d.sys.Println();
                    current = RunExe(d, buffer);
                    buffer = "";
                }
            }
        }
    }

    public Shell(NodeFlags flags = NodeFlags.None) : base("sh", flags) {}

    protected override IEnumerable<int?> Run(ProgData d) {
        Motd(d);
        while (true) {
            if (current != null) {
                current.Input(d.input);
                if (d.close)
                    current.Close();
                if (current.Update() != null) {
                    current = null;
                    ProgFinish(d);
                }
            } else if (d.input.Length > 0) {
                string[] inp = d.input.Split('\n');
                if (inp.Length > 0) {
                    if (inp[0].Length > 0) {
                        buffer += inp[0];
                        buffer = FixBackspace(buffer);
                        // JANK
                        d.sys.disp.SetLine("$ " + buffer, Color.green);
                    }
                    if (inp.Length > 1) {
                        d.sys.Println();
                        current = RunExe(d, buffer);
                        buffer = "";
                    }
                }
            }
            yield return null;
        }
    }
}
