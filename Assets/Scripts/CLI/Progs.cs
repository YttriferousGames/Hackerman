using System;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class Echo : Exe {
    public Echo(Sys s) : base(s, "echo") {}

    public override bool Start(string[] args) {
        sys.Println(String.Join(' ', args));
        return true;
    }
}

// Security risk? yes.
// Fun? ye
public class External : Exe {
    Process proc = null;

    public External(Sys s, string proc) : base(s, proc) {}

    public override bool Start(string[] args) {
        if (proc != null)
            proc.Close();
        proc = new Process();
        proc.StartInfo.FileName = Name;
        proc.StartInfo.Arguments = String.Join(' ', args);
        proc.StartInfo.CreateNoWindow = true;
        proc.StartInfo.RedirectStandardError = true;
        proc.StartInfo.RedirectStandardInput = true;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.StartInfo.StandardErrorEncoding = System.Text.Encoding.ASCII;
        proc.StartInfo.StandardInputEncoding = System.Text.Encoding.ASCII;
        proc.StartInfo.StandardOutputEncoding = System.Text.Encoding.ASCII;
        proc.StartInfo.UseShellExecute = false;
        proc.OutputDataReceived += new DataReceivedEventHandler((sender, e) => {
            if (e.Data != null)
                sys.Println(e.Data);
        });
        proc.ErrorDataReceived += new DataReceivedEventHandler((sender, e) => {
            if (e.Data != null)
                sys.Println(e.Data);
        });
        proc.Start();
        proc.BeginOutputReadLine();
        proc.BeginErrorReadLine();
        return false;
    }

    public override bool Update() {
        if (proc == null || proc.HasExited) {
            return Close();
        }
        return false;
    }

    public override bool Close() {
        if (proc == null)
            return true;
        proc.Close();
        proc = null;
        return true;
    }
}

public class CD : Exe {
    public CD(Sys s) : base(s, "cd") {}

    public override bool Start(string[] args) {
        if (args.Length == 0) {
            return true;
        } else if (args.Length > 1) {
            sys.Println("cd: too many arguments");
            return true;
        }
        Path p = sys.CanonPath(args[0]);
        Node n = sys.GetNode(p);
        if (n is Dir d) {
            sys.workingDir = p;
        } else if (n != null) {
            sys.Println("cd: not a directory: " + p.ToString());
        } else {
            sys.Println("cd: no such file or directory: " + p.ToString());
        }
        return true;
    }
}

public class PWD : Exe {
    public PWD(Sys s) : base(s, "pwd") {}

    public override bool Start(string[] args) {
        if (args.Length > 0) {
            sys.Println("pwd: too many arguments");
            return true;
        }
        sys.Println(sys.workingDir.ToString());
        return true;
    }
}

public class LS : Exe {
    public LS(Sys s) : base(s, "ls") {}

    private void ListPath(Path p, bool name = false) {
        Node n = sys.GetNode(p);
        if (n is Dir d) {
            if (name)
                sys.Println(p.ToString() + ":");
            sys.Println(String.Join(' ', d.Contents.Select(n => n.Name)));
        } else {
            sys.Println(n.Name);
        }
    }

    public override bool Start(string[] args) {
        if (args.Length == 0) {
            ListPath(sys.workingDir);
        } else if (args.Length == 1) {
            ListPath(sys.CanonPath(args[0]));
        } else {
            foreach (string s in args) {
                Path p = sys.CanonPath(s);
                ListPath(p, true);
                sys.Println("");
            }
        }
        return true;
    }
}

public class Cat : Exe {
    public Cat(Sys s) : base(s, "cat") {}

    public override bool Start(string[] args) {
        foreach (string s in args) {
            Node n = sys.GetNode(s);
            if (n is File f) {
                sys.Println(f.Contents);
            } else {
                sys.Println("cat: " + s + ": is not a file");
            }
        }
        return true;
    }
}

public class Demo : Exe {
    private int step;
    public Demo(Sys s, int step = 4) : base(s, "demo") {
        this.step = step;
    }

    public override bool Start(string[] args) {
        return false;
    }

    public override bool Update() {
        if (Time.frameCount % step == 0) {
            int frame = Time.frameCount / step;
            float n = ((float)sys.disp.width - 1f) * (Mathf.Sin(frame / 5f) / 2f + 0.5f);
            sys.Println();
            for (float j = 0f; j < n; j++) {
                sys.Print("#", new Color(n / 10, j / n, 0.8f));
            }
        }
        return false;
    }
}

// public class SelfDestruct : Exe {
//     public SelfDestruct(Sys s) : base(s, "selfdestruct") {}
//     public override bool Start(string[] args) {
//         ParticleSystem p = sys.GetComponent<ParticleSystem>();
//         p.Play();
//         return true;
//     }
// }
