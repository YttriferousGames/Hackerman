// A combination of FSLayer "stacked"

using System.Linq;

// TODO layered FS where files are added/removed/modified as game progresses, per PC.
// There's the problem of mutability, perhaps I can make things read only
// Should I keep FSLayer immutable?
// Should I merge the layers in the creation of this?
// idk
public class StackedFS : FS {
    private List<FSLayer> layers;

    public override Node GetNode(Path path) {
        foreach (FSLayer layer in layers) {
            Node r = layer.GetNode(path);
            if (r != null)
                return r;
        }
        return null;
    }

    public StackedFS(IEnumerable<FSLayer> layers) {
        this.layers = layers.ToList();
    }
}
