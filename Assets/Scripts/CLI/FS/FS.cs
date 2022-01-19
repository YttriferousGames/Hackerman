// Abstract class for a filesystem

// TODO this should be a ScriptableObject
// So they can be edited in the UI, and then layered
public abstract class FS {
    private const int SYMLINK_DEPTH = 4;
    // Gets a Node at an absolute path
    public abstract Node GetNode(Path path);

    // TODO what about directory symlinks in the middle of a path
    private Node GetNode(Path path, int recurseSymlinks) {
        if (recurseSymlinks <= 0) {
            return GetNode(path);
        } else {
            Node n = GetNode(path);
            if (n is Symlink l) {
                return GetNode(l.Target, recurseSymlinks - 1);
            } else {
                return n;
            }
        }
    }

    // Gets a Node at an absolute path, recursing through symlinks
    public Node GetNode(Path path, bool recurseSymlinks) {
        if (!recurseSymlinks)
            return GetNode(path);
        else
            return GetNode(path, SYMLINK_DEPTH);
    }

    // Gets a Node of type T at an absolute path
    public T GetNode<T>(Path path)
        where T : class {
        if (GetNode(path) is T t) {
            return t;
        } else {
            return null;
        }
    }

    // Gets a Node of type T at an absolute path, recursing through symlinks
    public T GetNode<T>(Path path, bool recurseSymlinks)
        where T : class {
        if (GetNode(path, recurseSymlinks) is T t) {
            return t;
        } else {
            return null;
        }
    }

    // Resolves a Node to a version without symlinks
    public Node ResolveSymlink(Node node) {
        if (node is Symlink l) {
            return GetNode(l.Target, SYMLINK_DEPTH - 1);
        } else {
            return node;
        }
    }

    // Resolves a Node of type T to a version without symlinks
    public T ResolveSymlink<T>(Node node)
        where T : class {
        if (ResolveSymlink(node) is T t) {
            return t;
        } else {
            return null;
        }
    }
}
