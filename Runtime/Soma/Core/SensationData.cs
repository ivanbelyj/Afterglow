using System.Collections.Generic;
using UnityEngine;
using System;

public record SensationData : IUntypedStorage
{
    public Guid SensationId { get; }
    public string PhysicalImpactType { get; set; }
    public string AffectedPartName { get; set; }
    public Dictionary<string, object> Data { get; } = new();

    public SensationData(Guid sensationId)
    {
        SensationId = sensationId;
    }
}