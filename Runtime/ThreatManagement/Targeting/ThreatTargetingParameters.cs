using System;
using UnityEngine;

[Serializable]
public class ThreatTargetingParameters
{
    [Tooltip("Minimum probability to handle a threat")]
    public float threatProbabilityThreshold = 0.03f;

    // [Tooltip("Minimum probability necessary to consider a threat as critical")]
    // public float criticalThreatProbabilityThreshold = 0.5f;

}