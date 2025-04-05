using UnityEngine;

public record PerceptedSightData
{
    public float Distance { get; set; }
    public PerceptionEntry PerceptionEntry { get; set; }
}
