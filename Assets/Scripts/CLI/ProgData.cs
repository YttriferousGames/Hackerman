// Contains the state provided to a program being executed

// Things to redesign:
// - Input handling
//   - Consider a program that uses arrow keys, mouse input etc
//   - Remember mouse input should also be usable to highlight screen contents
//   - And all that custom input should only be done when it's explicitly allowed to (and when PC is
//   focused)
//   - Helper methods to get mouse in screen coords etc
//   - There should still be an easy way to just "readline" stuff

public class ProgData : Prog, ProgAPI {
    public string[] args { get => _args; }
    private readonly string[] _args;
    public string input { get => _input; }
    private string _input = "";
    // True if the program should be closed (CTRL+C)
    public bool close {
        get => _close;
        set => _close = value;
    }
    private bool _close = false;
    private TextOut _o = null;
    public TextOut Out { get => _o; }
    public Sys sys { get => _sys; }
    private readonly Sys _sys;

    public ProgData(Sys s, TextOut o, Exe.RunFunc p, string[] args = null) {
        this._sys = s;
        this._o = o;
        this._args = args ?? new string[] {};
        this.inner = p(this).GetEnumerator();
    }

    public void Close() {
        _close = true;
    }

    // This is pretty inadequate
    public void Input(string inp) {
        _input += inp;
    }

    private IEnumerator<int?> inner = null;

    public int? Update() {
        if (inner.MoveNext()) {
            _input = "";
            return null;
        } else {
            return inner.Current ?? 0;
        }
    }
}

public interface ProgAPI {
    string[] args { get; }
    string input { get; }
    // True if the program should be closed (CTRL+C)
    bool close { get; set; }
    Sys sys { get; }
    TextOut Out { get; }
}

public interface Prog {
    void Close();

    // This is pretty inadequate
    void Input(string inp);

    int? Update();
}