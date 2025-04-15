using System.Collections.Generic;
using UnityEngine;

using static SightDataKeys;

public static class SightDataExtensions
{
    public static bool TryGetMovementSpeed(
        this Sight sight,
        out float movementSpeed)
        => sight.TryGet(MovementSpeed, out movementSpeed);

    public static void SetMovementSpeed(
        this Sight sight,
        float movementSpeed)
        => sight.Set(MovementSpeed, movementSpeed);
}
