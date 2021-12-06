using UnityEngine;

public class CLITest : MonoBehaviour {
    // Start is called before the first frame update
    private Sys s = null;
    private Exe pac = null;

    private Exe Run(Sys s, string cmd) {
        s.Println("$ " + cmd, Color.green);
        string[] args = cmd.Split(' ');
        Exe prog = s.GetProgram(args[0]);
        try {
            prog.Start(args[1..]);
        } catch (System.Exception e) {
            s.Println(e.ToString(), Color.red);
        }
        return prog;
    }

    void Start() {
        s = GetComponent<Sys>();

        Run(s, "echo one two oatmeal");
        Run(s, "ls ./ ../../ /bin/");
        Run(s, "cd /home/.");
        Run(s, "cat geff/README.txt");
        Run(s, "ls ./ geff/ geff/README.txt");

        pac = Run(s, "pacman -Ss steam");
    }

    // Update is called once per frame
    void Update() {
        if (pac != null) pac.Update();
    }
}
