using UnityEngine;

/// <summary>
/// Manages objects that implement <see cref="IPerceptionTrackingHandler"/>
/// </summary>
public interface IPerceptionTrackingStorage
{
    void Track(PerceptionEntry perceptionEntry);
    void RegisterHandler(IPerceptionTrackingHandler tracker);
}
