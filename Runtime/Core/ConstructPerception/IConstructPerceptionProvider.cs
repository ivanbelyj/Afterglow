using UnityEngine;

/// <summary>
/// Constructs are perceptions that serve as generalizations,
/// inferences, or accumulations, created independently of external world signals
/// typically based on other perceptions.
/// </summary>
public interface IConstructPerceptionProvider
{
    PerceptionEntry GetConstruct(string constructKey);
    bool TryGetConstruct(string constructKey, out PerceptionEntry perceptionEntry);
}
