// Contains the FS state of all layers
// Used to create new content
using UnityEngine;

public enum FSLayerName {
    Base,
}

// This stuff would be cool to put in a ScriptableObject, but it's a real pain in the ass
// So for now, this will do fine.
// TODO order alphabetically
public static class FSLayerCreator {
    public static FSLayer Create(this FSLayerName layer) {
        switch (layer) {
            case FSLayerName.Base:
                return new FSLayer(
                    new Node
                    [] { new Dir("bin",
                                 new Node[] {
#if UNITY_STANDALONE
                                     new External("pacman", NodeFlags.Hidden),
                                         new Symlink("yay", "./pacman", NodeFlags.Hidden),
#endif
                                         new Echo(), new CD(), new PWD(), new LS(), new Cat(),
                                         new Demo(flags: NodeFlags.Hidden),
                                         new Shell(NodeFlags.Hidden),
                                         new StarWars(Resources.Load<TextAsset>("starwars"),
                                                      NodeFlags.Hidden),
                                 }),
                         new Dir("home", new Node[] { new Dir(
                                             "geff", new Node[] { new File("README.txt",
                                                                           "Hello, world!") }) }),
                         new Dir(
                             "usr", new Node[] { new Dir("share", new Node[] {
                                 new Dir(
                                     "doc", new Node
                                     [] { new File("help.txt", Resources.Load<TextAsset>("usr/share/doc/help")),
                                          new File("commands.txt", Resources.Load<TextAsset>("usr/share/doc/cmd")) })
                             }) }) });
            default:
                return null;
        }
    }
}
