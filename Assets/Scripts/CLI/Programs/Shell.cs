// The shell of the computer (the commandline you run commands at)

using System.Text.RegularExpressions;

// TODO dispense with the pile of hacks
// Get this done the RIGHT way
public class Shell : Exe {
    private class ShellGuts {
        public Prog current = null;
        public string buffer = "";
        public ProgAPI d = null;
        public Prog RunExe(string cmd) {
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
                    p = prog.Start(d.sys, d.Out, args[1..]);
                    done = p.Update() != null;
                } catch (System.Exception e) {
                    d.Out.Println(e.Message, Color.red);
                    UnityEngine.Debug.LogException(e);
                }
            } else {
                d.Out.Print(args[0] + " is not a command (try ", Color.red);
                d.Out.Print("ls /bin/", Color.magenta);
                d.Out.Println(")", Color.red);
            }
            if (done) {
                ProgFinish();
                return null;
            } else {
                return p;
            }
        }

        public void Motd() {
            d.Out.Println("[Alibaba Intelligence OS v0.1]", Color.cyan);
            ProgFinish();
        }

        public void ProgFinish() {
            d.Out.ScreenOverride = null;
            d.Out.Print("$ ", Color.green);
        }

        public ShellGuts(ProgAPI d) {
            this.d = d;
        }
    }

    private static Regex regex = new Regex(@"(?<L>[^\b])+(?<R-L>[\b])+(?(L)(?!))");
    public Shell(NodeFlags flags = NodeFlags.None) : base("sh", flags) {}

    private static string FixBackspace(string s) {
        return regex.Replace(s, "").Replace("\b", "");
    }

    protected override IEnumerable<int?> Run(ProgAPI d) {
        ShellGuts guts = new ShellGuts(d);
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
                    d.Out.Println(e.Message, Color.red);
                    UnityEngine.Debug.LogException(e);
                }
            } else if (d.input.Length > 0) {
                string[] inp = d.input.SplitNewlines();
                if (inp.Length > 0) {
                    if (inp[0].Length > 0) {
                        guts.buffer += inp[0];
                        guts.buffer = FixBackspace(guts.buffer);
                        // JANK
                        d.Out.SetLine("$ " + guts.buffer, Color.green);
                    }
                    if (inp.Length > 1) {
                        d.Out.Println();
                        guts.current = guts.RunExe(guts.buffer);
                        guts.buffer = "";
                    }
                }
            }
            d.close = false;
            yield return null;
        }
    }
}
