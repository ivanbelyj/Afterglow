using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MemorySimulator))]
public class SegregatedMemoryManager : MonoBehaviour, ISegregatedMemoryProvider
{
    private MemorySimulator memorySimulator;
    private SensoryMemoryManager sensoryMemoryManager;

    private List<IPerceptionSource> perceptionSources = new();

    protected virtual void Awake()
    {
        memorySimulator = GetComponent<MemorySimulator>();
    }

    protected virtual void Start()
    {
        sensoryMemoryManager = new(memorySimulator.PerceptionStorage);
        RegisterPerceptionSource(memorySimulator.PerceptionStorage);
    }

    public void RegisterSensoryMemory(ISensoryMemoryStorage sensoryMemory)
    {
        sensoryMemoryManager.RegisterSensoryMemory(sensoryMemory);
        RegisterPerceptionSource(sensoryMemory);
    }

    public void RegisterPerceptionSource(IPerceptionSource perceptionSource)
    {
        perceptionSources.Add(perceptionSource);
    }

    public List<PerceptionEntry> GetPerceptions(
        uint perceptionSourceMask,
        params string[] markers)
    {
        return GetPerceptionSources(perceptionSourceMask)
            .SelectMany(source => source.GetPerceptions(markers))
            .ToList();
    }

    private IEnumerable<IPerceptionSource> GetPerceptionSources(
        uint perceptionSourceLayerMask)
    {
        foreach (var perceptionSource in perceptionSources)
        {
            if (Satisfies(
                perceptionSource.PerceptionSourceLayerMask,
                perceptionSourceLayerMask))
            {
                yield return perceptionSource;
            }
        }
    }

    private bool Satisfies(uint baseLayerMask, uint requestedLayerMask)
        => (baseLayerMask & requestedLayerMask) != 0;
}
