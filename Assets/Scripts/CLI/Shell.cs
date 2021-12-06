using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : Exe {
    public Shell(Sys s) : base(s, "sh") {}

    public override bool Start(string[] args) {
        return true;
    }
}
