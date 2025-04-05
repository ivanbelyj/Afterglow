using System;
using UnityEngine;

/// <summary>
/// Built-in perception layers. Perception layers allow to extract perceptions
/// separately and therefore more efficiently, affecting only necessary sources.
/// </summary>
public enum SegregatedPerceptionLayerCore : uint
{
    CommonMemory = 1,
    VisionSensoryMemory = 2,
    SpatialAwareness = 3,
}
