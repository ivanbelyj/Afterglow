using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SegregatedMemoryManager : MonoBehaviour, ISegregatedMemoryProvider
{
    private Dictionary<string, IPerceptionSource> perceptionSourcesByKey = new();

    public IReadOnlyDictionary<string, IPerceptionSource> PerceptionSources
        => perceptionSourcesByKey;

    protected virtual void Awake()
    {
        
    }

    protected virtual void Start()
    {
        
    }

    public void RegisterPerceptionSource(IPerceptionSource perceptionSource)
    {
        perceptionSourcesByKey.Add(
            perceptionSource.PerceptionSourceKey,
            perceptionSource);
    }

    public bool TryGetRegisteredPerceptionSource(
        string perceptionSourceKey,
        out IPerceptionSource perceptionSource)
        => perceptionSourcesByKey.TryGetValue(perceptionSourceKey, out perceptionSource);

    public List<PerceptionEntry> GetPerceptions(
        string perceptionSourceKey,
        params string[] markers)
    {
        return GetPerceptions(perceptionSourcesByKey[perceptionSourceKey], markers);
    }

    public List<PerceptionEntry> GetPerceptions(
        IPerceptionSource perceptionSource,
        params string[] markers)
    {
        return perceptionSource
            .GetPerceptions(markers)
            .ToList();
    }
}
