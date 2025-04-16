using System;
using UnityEngine;

/// <summary>
/// Built-in perception layers. Perception layers allow to extract perceptions
/// separately and therefore more efficiently, affecting only necessary sources.
/// </summary>
public static class CoreSegregatedPerceptionSources
{
    public const string SimulatedMemory = nameof(SimulatedMemory);
    public const string VisionSensoryMemory = nameof(VisionSensoryMemory);
    public const string SomaticSensoryMemory = nameof(SomaticSensoryMemory);
    public const string SpatialAwareness = nameof(SpatialAwareness);
    public const string PossibleThreat = nameof(PossibleThreat);
}
