using System;
using UnityEngine;

public class ThreatPerception : IThreatPerception
{
    private readonly PerceptionEntry perceptionEntry;

    public Guid EntityId => perceptionEntry.GetEntityId();
    public SpatialAwarenessPosition Position => perceptionEntry.GetPosition();

    public ThreatKnowledge ThreatKnowledge { get; private set; }

    public double? Timestamp => perceptionEntry.TimestampTo ?? perceptionEntry.TimestampFrom;

    public ThreatPerception(
        PerceptionEntry perceptionEntry,
        ThreatKnowledge knowledge)
    {
        this.perceptionEntry = perceptionEntry;
        ThreatKnowledge = knowledge;
    }
}
