using UnityEngine;

/// <summary>
/// Transforms representation (for example, a sound or a vision) into perception
/// </summary>
public interface IPerceptionEnricher<TRepresentation>
{
    void EnrichPerception(PerceptionEntry perception, TRepresentation representation);
}
