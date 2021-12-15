// A combination of FSLayer "stacked"

using System.Collections.Generic;
using System.Linq;

// TODO
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
