using System.Collections.Generic;
using UnityEngine;

using static PerceptionEntryCoreDataKeys;

public class VisionPerceptionEnricher : IPerceptionEnricher<Sight>
{
    public void EnrichPerception(PerceptionEntry perception, Sight sight)
    {
        perception.Accessibility = 1f;
        // TODO:
        // perception.VerbalRepresentation = GetVisionMemoryVerbalRepresentation(sight);
        perception.PerceptionData[EntityId] = sight.EntityId;
        // TODO: not that type
        // perception.PerceptionData[Position] = sight.transform.position;
    }

    private string GetVisionMemoryVerbalRepresentation(Sight sight)
    {
        return $"I saw the following sight: {sight.VerbalRepresentation}";
    }
}
