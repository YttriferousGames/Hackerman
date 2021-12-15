// Contains the state of a program being executed

using System.Collections.Generic;

public class Prog {
    private ProgData d;
    private IEnumerator<int?> inner;

    public Prog(ProgData d, IEnumerable<int?> inner) {
        this.d = d;
        this.inner = inner.GetEnumerator();
    }

    public void Close() {
        d.close = true;
    }

    public void Input(string inp) {
        d.input += inp;
    }

    public int? Update() {
        if (inner.MoveNext()) {
            d.input = "";
            return null;
        } else {
            return inner.Current ?? 0;
        }
    }
}
