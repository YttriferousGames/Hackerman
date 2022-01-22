using UnityEngine;
using System.Collections.Generic;

/// <summary>Star Wars from asciimation.co.nz</summary>
public class StarWars : Exe {
    private const float updateDelay = 0.067f;
    private const int width = 67;
    private const int height = 13;
    private string[] film;
    public StarWars(TextAsset data, NodeFlags flags = NodeFlags.None) : base("starwars", flags) {
        this.film = data.text.SplitNewlines();
    }

    protected override IEnumerable<int?> Run(ProgAPI d) {
        for (int f = 0; f >= 0 && f < film.Length / (height + 1); f++) {
            Cell[,] screen = new Cell[width, height];
            for (int l = 1; l < (height + 1); l++) {
                string line = film[(height + 1) * f + l];
                Cell[] lineD = Cell.CreateArray(line, Color.yellow);
                for (int i = 0; i < width; i++) {
                    if (i < lineD.Length) {
                        screen[i, l - 1] = lineD[i];
                    } else {
                        screen[i, l - 1] = new Cell(null, Color.yellow);
                    }
                }
            }
            d.Out.ScreenOverride = screen;

            float delay = (float)int.Parse(film[(height + 1) * f]) * updateDelay;
            float targetTime = Time.time + delay;
            while (targetTime > Time.time && f >= 0) {
                if (d.close) {
                    f = -1;
                } else {
                    yield return null;
                }
            }
            if (f < 0)
                break;
        }
        d.Out.ScreenOverride = null;
        yield return 0;
    }
}
