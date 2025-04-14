
using System;
using UnityEngine;

public interface IThreatPerception
{
    Guid EntityId { get; }
    string EntityType { get; }
    SpatialAwarenessPosition Position { get; }
    ThreatKnowledge ThreatKnowledge { get; }
    double? Timestamp { get; }
    bool IsDestructured { get; }
}
