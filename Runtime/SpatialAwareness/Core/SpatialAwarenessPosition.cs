using System.Collections.Generic;
using UnityEngine;

public record SpatialAwarenessPosition
{
    public Vector3 Position { get; set; }
    public float Radius { get; set; }
    public PerceptionEntry Perception { get; set; }
}
