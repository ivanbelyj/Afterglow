using UnityEngine;
using System;

public record PerceptedSightData
{
    public float Distance { get; }
    public PerceptionEntry PerceptionEntry { get; }

    public PerceptedSightData(float distance, PerceptionEntry perceptionEntry)
    {
        if (distance < 0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(distance), 
                "Distance cannot be negative");
        }

        Distance = distance;
        PerceptionEntry = perceptionEntry ?? throw new ArgumentNullException(nameof(perceptionEntry));
    }
}