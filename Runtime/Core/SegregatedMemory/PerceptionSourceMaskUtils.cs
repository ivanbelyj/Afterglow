using UnityEngine;

public static class PerceptionSourceMaskUtils
{
    public static bool SatisfiesMask(uint perceptionSourceMask, uint requestedMask)
        => (perceptionSourceMask & requestedMask) != 0;
}
