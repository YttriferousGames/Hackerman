using UnityEngine;
using System.Collections.Generic;

/// <summary>Rainbow sine wave demo</summary>
public class Demo : Exe {
    private float step;
    private float duration;
    public Demo(float step = 1f / 20f, float duration = 10f, NodeFlags flags = NodeFlags.None)
        : base("demo", flags) {
        this.step = step;
        this.duration = duration;
    }

    protected override IEnumerable<int?> Run(ProgAPI d) {
        float start = Time.time;
        float t = start;
        while (t - start < duration && !d.close) {
            t = Time.time;
            float n = ((float)d.Out.width - 1f) * (Mathf.Sin(t * 4f) / 2f + 0.5f);
            d.Out.Println();
            for (float j = 0f; j < n; j++) {
                d.Out.Print("#", new Color(n / 10, j / n, 0.8f));
            }
            while (t + step > Time.time) yield return null;
        }
        d.Out.Println();
        yield return 0;
    }
}
