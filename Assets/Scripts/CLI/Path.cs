using System;
using System.Collections.Generic;
using System.Linq;

public enum PathType {
    // Relative to /
    Absolute,
    // Relative to ./
    Relative,
}

// TODO only support absolute paths and disambiguify in the constructor (based on context)
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
            default:
                p = "";
                break;
        }

        p += String.Join('/', nodes);
        return p;
    }

    private string[] SimplifyNodes(IEnumerable<string> nodes) {
        List<string> betterNodes = new List<string>();
        foreach (string n in nodes) {
            switch (n) {
                case ".":
                    break;
                case "..":
                    if (betterNodes.Any()) {
                        betterNodes.RemoveAt(betterNodes.Count - 1);
                    } else {
                        betterNodes.Add(n);
                    }
                    break;
                default:
                    betterNodes.Add(n);
                    break;
            }
        }
        return betterNodes.ToArray();
    }

    // TODO make it compliant to all edgecases and weird stuff
    public Path(string path) {
        char pre = path[0];
        string p = path;
        if (pre == '/') {
            pathType = PathType.Absolute;
            p = path[1..];
        }
        nodes = SimplifyNodes(p.Split('/', StringSplitOptions.RemoveEmptyEntries));
    }

    private Path(string[] n, PathType pt) {
        nodes = n;
        pathType = pt;
    }

    public Path(Path p) {
        nodes = SimplifyNodes(p.nodes);
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
