using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;

using System.Linq;

/// <summary>Directory node (contains nodes)</summary>
public class Dir : Node {
    /// <summary>Child nodes</summary>
    public List<Node> Contents;

    // TODO propagate ReadOnly
    public Dir(string name, IEnumerable<Node> contents, NodeFlags flags = NodeFlags.None) {
        Name = name;
        Contents = contents.ToList();
        Contents.Sort((x, y) => x.Name.CompareTo(y.Name));
        this.flags = flags;
    }
}
