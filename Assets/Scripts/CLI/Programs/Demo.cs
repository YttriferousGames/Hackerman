using UnityEngine;
using System.Collections.Generic;

public class Demo : Exe {
    private float step;
    public Demo(float step = 1f / 20f, NodeFlags flags = NodeFlags.None) : base("demo", flags) {
        this.step = step;
    }

    protected override IEnumerable<int?> Run(ProgData d) {
        while (true) {
            float t = Time.time;
            float n = ((float)d.sys.disp.width - 1f) * (Mathf.Sin(t * 4f) / 2f + 0.5f);
            d.sys.Println();
            for (float j = 0f; j < n; j++) {
                d.sys.Print("#", new Color(n / 10, j / n, 0.8f));
            }
            while (t + step > Time.time) yield return null;
        }
    }
}
