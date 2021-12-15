// Contains the state provided to a program being executed

public class ProgData {
    public readonly string[] args;
    public string input = "";
    public bool close = false;
    public string output = "";
    public readonly Sys sys;

    public ProgData(Sys s, string[] args = null) {
        this.sys = s;
        this.args = args ?? new string[] {};
    }

    // TODO refactor printing/whole screen control
    // Make it easier to tell if programs can read mouse/keyboard
    // Have a helper method to get mouse coordinates
    // public void Print() {}
}
