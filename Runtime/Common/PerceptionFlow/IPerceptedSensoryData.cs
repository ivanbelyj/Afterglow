using UnityEngine;

public interface IPerceptedSensoryData<TRepresentation>
{
    PerceptionEntry PerceptionEntry { get; }
    TRepresentation Representation { get; }
}
