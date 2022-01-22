// Contains the FS state of all layers
// to be combined for different system configurations

public enum FSLayerName {
    Base,
    EasterEggs,
}

// This stuff would be cool to put in a ScriptableObject, but it's a real pain in the ass
// So for now, this will do fine.
public static class FSLayers {
    private static readonly FSLayer Base = new FSLayer(
        new Node[] { new Dir("bin",
                             new Node[] {
                                 new Echo(),
                                 new CD(),
                                 new PWD(),
                                 new LS(),
                                 new Cat(),
                                 new Shell(NodeFlags.Hidden),
                             }),
                     new Dir("home",
                             new Node[] { new Dir(
                                 "geff", new Node[] { new File("README.txt", "Hello, world!") }) }),
                     new Dir(
                         "usr",
                         new Node[] { new Dir("share", new Node[] { new Dir("doc", new Node[] {
                                                  new File("help.txt", Resources.Load<TextAsset>(
                                                                           "usr/share/doc/help")),
                                                  new File("commands.txt",
                                                           Resources.Load<TextAsset>("usr/share/doc/cmd"))
                                              }) }) }) });

    private static readonly FSLayer EasterEggs = new FSLayer(new Node[] {
        new Dir("bin",
                new Node[] {
#if UNITY_STANDALONE
                    new External("pacman", NodeFlags.Hidden),
                        new Symlink("yay", "./pacman", NodeFlags.Hidden),
#endif
                        new Demo(flags: NodeFlags.Hidden),
                        new StarWars(Resources.Load<TextAsset>("starwars"), NodeFlags.Hidden),
                }),
    });

    public static FSLayer Create(this FSLayerName layer) {
        switch (layer) {
            case FSLayerName.Base:
                return Base;
            case FSLayerName.EasterEggs:
                return EasterEggs;
            default:
                return null;
        }
    }
}
