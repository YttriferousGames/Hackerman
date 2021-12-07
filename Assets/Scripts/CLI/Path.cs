using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEditor;

public enum PathType {
    // Relative to /
    Absolute,
    // Relative to ./
    Relative,
    // Relative to ~/
    Home,
}

public class Path {
    public readonly string[] nodes;
    public readonly PathType pathType = PathType.Relative;

    public static implicit operator Path(string path) => new Path(path);

    public override string ToString() {
        string p;
        switch (pathType) {
            case PathType.Absolute:
                p = "/";
                break;
            case PathType.Home:
                p = "~";
                break;
            default:
                p = "";
                break;
        }

        p += String.Join('/', nodes);
        return p;
    }

    // TODO make it compliant to all edgecases and weird stuff
    public Path(string path) {
        char pre = path[0];
        string p = path;
        switch (pre) {
            case '/':
                pathType = PathType.Absolute;
                p = path[1..];
                break;
            case '~':
                pathType = PathType.Home;
                p = path[1..];
                break;
        }
        nodes = p.Split('/', StringSplitOptions.RemoveEmptyEntries);
        List<string> betterNodes = new List<string>();
        foreach (string n in nodes) {
            switch (n) {
                case ".":
                    break;
                case "..":
                    if (betterNodes.Any())
                        betterNodes.RemoveAt(betterNodes.Count - 1);
                    break;
                default:
                    betterNodes.Add(n);
                    break;
            }
        }
        nodes = betterNodes.ToArray();
    }

    private Path(string[] n, PathType pt) {
        nodes = n;
        pathType = pt;
    }

    public Path(Path p) {
        nodes = p.nodes;
        pathType = p.pathType;
    }

    // Add currentDirectory to the start of Path
    public Path WithBase(Path currentDir) {
        if (pathType != PathType.Absolute) {
            return new Path(currentDir.nodes.Concat(nodes).ToArray(), currentDir.pathType);
        } else {
            return new Path(this);
        }
    }
}
