using System.Collections.Generic;
using UnityEngine;
using System;

public record PerceivedPhysicalImpact : IUntypedStorage
{
    public Guid PhysicalImpactId { get; }
    public string PhysicalImpactType { get; set; }
    public string AffectedZoneName { get; set; }
    public Guid? CauseEntityId { get; set; }
    public Dictionary<string, object> Data { get; } = new();

    public PerceivedPhysicalImpact(Guid physicalImpactId, Guid? causeEntityId)
    {
        PhysicalImpactId = physicalImpactId;
        CauseEntityId = causeEntityId;
    }
}