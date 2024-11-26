using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static void ShuffleList<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            (ts[i], ts[r]) = (ts[r], ts[i]);
        }
    }

    public static bool CheckInternetAvailability()
    {
        if (Application.internetReachability is NetworkReachability.ReachableViaCarrierDataNetwork or
            NetworkReachability.ReachableViaLocalAreaNetwork)
            return true;
        if (Application.internetReachability is NetworkReachability.NotReachable)
            return false;
        return false;
    }

}
