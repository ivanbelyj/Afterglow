
using System;
using UnityEngine;

public interface IThreatPerception
{
    Guid EntityId { get; }
    SpatialAwarenessPosition Position { get; }
    ThreatKnowledge ThreatKnowledge { get; }
    double? Timestamp { get; }
}
