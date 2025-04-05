using UnityEngine;

public record VisionSensoryMemoryEventArgs
{
    public Sight Sight { get; set; }
    public PerceptedSightData PerceptedSightData { get; set; }
}
