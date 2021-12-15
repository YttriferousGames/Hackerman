using System;
using System.Diagnostics;
using System.Collections.Generic;

#if UNITY_STANDALONE
// Security risk? yes.
// Fun? ye
public class External : Exe {
    public External(string proc, NodeFlags flags = NodeFlags.None) : base(proc, flags) {}

    protected override IEnumerable<int?> Run(ProgData d) {
        Process proc = new Process();
        proc.StartInfo.FileName = Name;
        proc.StartInfo.Arguments = String.Join(' ', d.args);
        proc.StartInfo.CreateNoWindow = true;
        proc.StartInfo.RedirectStandardError = true;
        proc.StartInfo.RedirectStandardInput = true;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.StartInfo.StandardErrorEncoding = BitFont.codePage;
        proc.StartInfo.StandardInputEncoding = BitFont.codePage;
        proc.StartInfo.StandardOutputEncoding = BitFont.codePage;
        proc.StartInfo.UseShellExecute = false;
        proc.OutputDataReceived += new DataReceivedEventHandler((sender, e) => {
            if (e.Data != null)
                d.sys.Println(e.Data);
        });
        proc.ErrorDataReceived += new DataReceivedEventHandler((sender, e) => {
            if (e.Data != null)
                d.sys.Println(e.Data);
        });
        proc.Start();
        proc.BeginOutputReadLine();
        proc.BeginErrorReadLine();
        while (!proc.HasExited) {
            yield return null;
        }
        proc.Close();
        yield return proc.ExitCode;
    }
}
#endif
