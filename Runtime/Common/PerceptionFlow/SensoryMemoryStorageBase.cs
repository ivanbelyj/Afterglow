using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class SensoryMemoryStorageBase<TPerceptedDataKey, TPerceptedData, TRepresentation>
    : ISensoryMemoryStorage<TRepresentation>
    where TPerceptedData : IPerceptedSensoryData<TRepresentation>
{
    public event EventHandler<SensoryPerceptionEventArgs> SensoryPerceptionReleased;

    private readonly Dictionary<TPerceptedDataKey, TPerceptedData> perceptedDataByKey = new();
    private readonly IRecognizer<TRepresentation> perceptionRecognizer;

    public SensoryMemoryStorageBase(IRecognizer<TRepresentation> perceptionRecognizer)
    {
        this.perceptionRecognizer = perceptionRecognizer;
    }

    public abstract string PerceptionSourceKey { get; }

    public IEnumerable<PerceptionEntry> GetPerceptions(params string[] markers)
    {
        return perceptedDataByKey.Values.Select(x => x.PerceptionEntry);
    }

    public IEnumerable<IPerceptedSensoryData<TRepresentation>> GetPerceptedData()
    {
        return perceptedDataByKey.Values.Cast<IPerceptedSensoryData<TRepresentation>>();
    }

    protected abstract IEnumerable<string> GetPerceptionMarkers(TRepresentation representation);
    protected abstract IEnumerable<string> GetPerceptionIdentifyingMarkers(TRepresentation representation);

    protected PerceptionEntry ToPerception(TRepresentation representation)
    {
        var perception = perceptionRecognizer.Recognize(representation);
        foreach (var marker in GetPerceptionMarkers(representation))
        {
            perception.Markers.Add(marker);
        }
        return perception;
    }

    protected void Release(TPerceptedData perceptedData)
    {
        SensoryPerceptionReleased?.Invoke(
            this,
            new(
                GetPerceptionIdentifyingMarkers(perceptedData.Representation).ToArray(),
                perceptedData.PerceptionEntry));
    }

    protected TPerceptedData GetByKey(TPerceptedDataKey key)
        => perceptedDataByKey[key];

    protected void RemoveByKey(TPerceptedDataKey key)
        => perceptedDataByKey.Remove(key);

    protected void Set(TPerceptedDataKey key, TPerceptedData value)
        => perceptedDataByKey[key] = value;
}
