using System;
using UnityEngine;

public record ConstructPerceptionPropertyConfig
{
    public Func<PerceptionEntry, PerceptionEntry, bool> ShouldAggregate { get; set; }
    public Action<PerceptionEntry, PerceptionEntry> Aggregate { get; set; }
}