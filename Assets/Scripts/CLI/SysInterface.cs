using UnityEngine;

[RequireComponent(typeof(Sys))]
public class SysInterface : MonoBehaviour {
    // Start is called before the first frame update
    private Sys s;
    private Prog sh;

    public void HandleInput() {
        string inp = Input.inputString;
        inp = inp.Replace("\r\n", "\n");
        inp = inp.Replace("\r", "\n");
        if (inp.Length > 0) {
            sh.Input(inp);
        }
    }

    void Start() {
        s = GetComponent<Sys>();
        Shell shell = (Shell)s.GetProgram("sh");
        sh = shell.Start(s);
        Update();
    }

    // Update is called once per frame
    void Update() {
        sh.Update();
    }
}
