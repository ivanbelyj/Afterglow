using System;
using UnityEngine;

using static PerceptionEntryCoreDataKeys;

public class ThreatPerception : IThreatPerception
{
    private readonly PerceptionEntry perceptionEntry;

    public Guid EntityId => perceptionEntry.Get<Guid>(PerceptionEntryCoreDataKeys.EntityId);

    public string EntityType => perceptionEntry.Get<string>(PerceptionEntryCoreDataKeys.EntityType);

    public SpatialAwarenessPosition Position
        => perceptionEntry.Get<SpatialAwarenessPosition>(PerceptionEntryCoreDataKeys.Position);

    public ThreatKnowledge ThreatKnowledge { get; private set; }

    public double? Timestamp => perceptionEntry.Timestamp;

    public bool IsDestructured => perceptionEntry.IsDestructed;

    public ThreatPerception(
        PerceptionEntry perceptionEntry,
        ThreatKnowledge knowledge)
    {
        this.perceptionEntry = perceptionEntry;
        ThreatKnowledge = knowledge;
    }
}
