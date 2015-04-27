using UnityEngine;
using System.Collections;

public static class Extensions {
    public static float SignZero(float num)
    {
        return num > 0 ? 1f : (num < 0 ? -1f : 0f);
    }

}
