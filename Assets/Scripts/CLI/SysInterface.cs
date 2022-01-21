// The glue between input and the Shell

using UnityEngine;

[RequireComponent(typeof(Sys), typeof(TermRenderer))]
public class SysInterface : MonoBehaviour {
    // Start is called before the first frame update
    private Sys s;
    private Prog sh;
    private TermRenderer rend;

    public void HandleInput(bool handle) {
        if (handle) {
            string inp = Input.inputString.FixNewlines();
            if (inp.Length > 0) {
                sh.Input(inp);
            }
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.C)) {
                sh.Close();
            }
        }
    }

    void OnScreen(Cell[,] screen) {
        if (screen != null) {
            rend.Render(screen);
        }
    }

    private void Awake() {
        s = GetComponent<Sys>();
        rend = GetComponent<TermRenderer>();
    }

    private void Start() {
        Shell shell = s.GetProgram<Shell>("sh");
        sh = shell.Start(s);
        sh.OnScreen += OnScreen;
        Update();
    }

    // Update is called once per frame
    void Update() {
        sh.Update();
    }
}
