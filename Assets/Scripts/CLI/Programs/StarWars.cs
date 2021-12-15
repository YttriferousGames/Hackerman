using System;
using UnityEngine;
using System.Collections.Generic;

public class StarWars : Exe {
    private const float updateDelay = 0.067f;
    private const int width = 67;
    private const int height = 13;
    private string[] film;
    public StarWars(TextAsset data, NodeFlags flags = NodeFlags.None) : base("starwars", flags) {
        this.film = data.text.Split('\n', StringSplitOptions.None);
    }

    protected override IEnumerable<int?> Run(ProgData d) {
        d.sys.disp.overrideRender = true;
        for (int f = 0; f < film.Length / (height + 1); f++) {
            Cell[,] screen = new Cell[width, height];
            for (int l = 1; l < (height + 1); l++) {
                string line = film[(height + 1) * f + l];
                byte[] lineD = BitFont.codePage.GetBytes(line);
                for (int i = 0; i < width; i++) {
                    if (i < lineD.Length) {
                        screen[i, 12 - (l - 1)] = new Cell(lineD[i], Color.yellow);
                    } else {
                        screen[i, 12 - (l - 1)] = new Cell((byte)' ', Color.yellow);
                    }
                }
            }
            d.sys.SetScreen(screen);

            float delay = (float)int.Parse(film[(height + 1) * f]) * updateDelay;
            float targetTime = Time.time + delay;
            while (targetTime > Time.time) yield return null;
        }
        d.sys.disp.overrideRender = false;
        yield return 0;
    }
}
