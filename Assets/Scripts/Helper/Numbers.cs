using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Numbers
{
    public static bool RangeOfFloats(float value, float minInclusive, float maxInclusive)
    {
        if (value >= minInclusive && value <= maxInclusive)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
