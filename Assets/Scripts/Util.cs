global using UnityEngine;
global using UnityEngine.Assertions;
global using System;
global using System.Collections.Generic;

public static class Util {
    public static readonly string[] NEWLINES = new string[] { "\r\n", "\r", "\n" };

    public static readonly Color32 WHITE = new Color32(255, 255, 255, 255);

    public static string[] SplitNewlines(this string s) {
        return s.Split(NEWLINES, System.StringSplitOptions.None);
    }

    public static string FixNewlines(this string s) {
        return s.Replace("\r\n", "\n").Replace('\r', '\n');
    }

    public static T[] Concat<T>(this T[] a, T[] b) {
        if (b.Length == 0) {
            return a;
        } else if (a.Length == 0) {
            return b;
        } else {
            var c = new T[a.Length + b.Length];
            a.CopyTo(c, 0);
            b.CopyTo(c, a.Length);
            return c;
        }
    }
}