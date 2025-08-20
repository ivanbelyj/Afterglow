using static PerceptionEntryCoreDataKeys;

public class VisionPerceptionEnricher : IPerceptionEnricher<Sight>
{
    public void EnrichPerception(PerceptionEntry perception, Sight sight)
    {
        perception.Accessibility = 1f;

        // TODO:
        // perception.VerbalRepresentation = GetVisionMemoryVerbalRepresentation(sight);

        perception.Set(EntityId, sight.EntityId);
        perception.Set(EntityType, sight.EntityType);

        if (sight.TryGetAgentAttentionData(out var attentionData))
        {
            perception.SetAgentAttentionData(attentionData);
        }

        if (sight.TryGetMovementSpeed(out var movementSpeed))
        {
            perception.SetMovementSpeed(movementSpeed);
        }

        // TODO: not that type
        // perception.PerceptionData[Position] = sight.transform.position;
    }

    private string GetVisionMemoryVerbalRepresentation(Sight sight)
    {
        return $"I saw the following sight: {sight.VerbalRepresentation}";
    }
}
