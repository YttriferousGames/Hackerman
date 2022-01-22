using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;

/// <summary>Misc. utility code for use throughout the game</summary>
public static class Util {
    /// <summary>Array of all types of newlines, in order of precedence</summary>
    public static readonly string[] NEWLINES = new string[] { "\r\n", "\r", "\n" };

    /// <summary>The color white, as a <see cref="Color32"/></summary>
    public static readonly Color32 WHITE = new Color32(255, 255, 255, 255);

    /// <summary>Splits a string by its newlines (supports all types of newlines)</summary>
    public static string[] SplitNewlines(this string s) {
        return s.Split(NEWLINES, System.StringSplitOptions.None);
    }

    /// <summary>Replaces non-standard newlines with \n</summary>
    public static string FixNewlines(this string s) {
        return s.Replace("\r\n", "\n").Replace('\r', '\n');
    }

    /// <summary>Concatenates two arrays into a single array</summary>
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