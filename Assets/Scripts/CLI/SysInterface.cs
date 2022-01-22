/// <summary>The glue between input and the Shell</summary>
[RequireComponent(typeof(Sys), typeof(TermRenderer))]
public class SysInterface : MonoBehaviour {
    private Sys s;
    private Prog sh;
    [SerializeField]
    private int width = TextOut.DEFAULT_WIDTH;
    [SerializeField]
    private int height = TextOut.DEFAULT_HEIGHT;
    private TextBuffer tb = new TextBuffer(TextOut.DEFAULT_WIDTH, TextOut.DEFAULT_HEIGHT);
    private TermRenderer rend;
    [SerializeField]
    private AudioClip status;
    private AudioSource player = null;

    /// <summary>Should be called to handle system input</summary>
    public void HandleInput(bool handle) {
        if (handle) {
            string inp = Input.inputString.FixNewlines();
            if (inp.Length > 0) {
                sh.Input(inp);
            }
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) &&
                Input.GetKeyDown(KeyCode.C)) {
                sh.Close();
                PlayAudio(status);
            }
        }
    }

    private void Awake() {
        s = GetComponent<Sys>();
        rend = GetComponent<TermRenderer>();
        player = GetComponent<AudioSource>();
    }

    private void Start() {
        Shell shell = s.GetProgram<Shell>("sh");
        sh = shell.Start(s, tb);
        Update();
    }

    private void PlayAudio(AudioClip c) {
        if (player != null && c != null) {
            player.PlayOneShot(c);
        }
    }

    // Update is called once per frame
    private void Update() {
        tb.width = width;
        tb.height = height;
        sh.Update();
        if (tb.needsRedraw) {
            rend.Render(tb.Layout());
        }
    }
}
