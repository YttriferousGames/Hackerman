#if UNITY_STANDALONE
using System;
using System.Collections.Generic;
using System.Diagnostics;

// Security risk? yes.
// Fun? ye
/// <summary>Wraps an actual system program so it can be used in the game</summary>
public class External : Exe {
    public External(string proc, NodeFlags flags = NodeFlags.None) : base(proc, flags) {}

    protected override IEnumerable<int?> Run(ProgAPI d) {
        using (Process proc = new Process()) {
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
                    d.Out.Println(e.Data);
            });
            proc.ErrorDataReceived += new DataReceivedEventHandler((sender, e) => {
                if (e.Data != null)
                    d.Out.Println(e.Data);
            });
            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();
            // TODO handle keyboard interrupt
            bool killed = false;
            while (!proc.HasExited) {
                if (d.close && !killed) {
                    proc.CloseMainWindow();
                    killed = true;
                }
                yield return null;
            }
            yield return proc.ExitCode;
        }
    }
}
#endif
