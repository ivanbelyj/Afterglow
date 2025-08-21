using System.Collections.Generic;
using UnityEngine;

public record ConstructPerceptionConfig
{
    public List<ConstructPerceptionPropertyConfig> Properties { get; set; } = new();
}
