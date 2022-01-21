using UnityEngine;
using System.Collections.Generic;

public class SelfDestruct : Exe {
    public SelfDestruct() : base("selfdestruct") {}
    protected override IEnumerable<int?> Run(ProgAPI d) {
        ParticleSystem p = d.sys.GetComponent<ParticleSystem>();
        p.Play();
        yield return 0;
    }
}
