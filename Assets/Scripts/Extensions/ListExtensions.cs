using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using UnityEngine;

public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = list.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (Byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    public static void SortByName<T>(this IList<T> list) where T : MonoBehaviour
    {
        var sortedList = new List<T>(list);
        sortedList.Sort((x, y) => CompareNatural(x.gameObject.name, y.gameObject.name));
        for (int i = 0; i < sortedList.Count; i++)
        {
            list[i] = sortedList[i];
        }
    }
    
    
    private static int CompareNatural(string a, string b)
    {
        // Regular expression to separate numeric and non-numeric parts
        var regex = new Regex(@"\d+|\D+");
        var aParts = regex.Matches(a);
        var bParts = regex.Matches(b);

        int i = 0;
        while (i < aParts.Count && i < bParts.Count)
        {
            var aPart = aParts[i].Value;
            var bPart = bParts[i].Value;

            // Check if both parts are numeric
            if (int.TryParse(aPart, out int aNum) && int.TryParse(bPart, out int bNum))
            {
                // Compare numeric parts
                int result = aNum.CompareTo(bNum);
                if (result != 0) return result;
            }
            else
            {
                // Compare non-numeric parts lexicographically
                int result = string.Compare(aPart, bPart, System.StringComparison.Ordinal);
                if (result != 0) return result;
            }

            i++;
        }

        // If all parts so far are equal, compare by length (remaining parts)
        return aParts.Count.CompareTo(bParts.Count);
    }
}
