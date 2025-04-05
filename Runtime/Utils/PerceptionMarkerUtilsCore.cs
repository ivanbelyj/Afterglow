using System;
using UnityEngine;

public static class PerceptionMarkerUtilsCore
{
    // TODO
    public static string[] GetForEntity(Guid entityId)
    {
        return new string[] { GetUniqueForVisionPerception(entityId) };
    }

    public static string GetUniqueForVisionPerception(Guid entityId)
        => $"sight_{entityId}";
}
