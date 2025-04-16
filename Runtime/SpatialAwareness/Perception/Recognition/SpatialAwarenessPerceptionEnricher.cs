using UnityEngine;

using static PerceptionEntrySpatialAwarenessDataKeys;

public class SpatialAwarenessPerceptionEnricher : IPerceptionEnricher<Sight>
{
    public void EnrichPerception(PerceptionEntry perception, Sight sight)
    {
        var spatialAwarenessPosition = new SpatialAwarenessPosition() {
            Perception = perception,
            Position = sight.Position,
            Radius = sight.SpatialRadius
        };
        SetPositionPerceptionData(perception, spatialAwarenessPosition);
    }

    private void SetPositionPerceptionData(
        PerceptionEntry perceptionEntry,
        SpatialAwarenessPosition spatialAwarenessPosition
        // params string[] markers
        )
    {
        perceptionEntry.PerceptionData[Position] = spatialAwarenessPosition;
        // foreach (var marker in markers) 
        // {
        //     perceptionEntry.Markers.Add(marker);
        // }
    }
}
