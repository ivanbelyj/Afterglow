using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using static PerceptionMarkerUtils;

public class SomaticSensoryMemoryStorage : SensoryMemoryStorageBase<Guid, PerceptedSensationData, SensationData>
{
    public SomaticSensoryMemoryStorage(IRecognizer<SensationData> perceptionRecognizer)
        : base(perceptionRecognizer)
    {
        
    }

    public override string PerceptionSourceKey => CoreSegregatedPerceptionSources.SomaticSensoryMemory;

    public void BeginSensation(SensationData sensation)
    {
        var perceptedData = new PerceptedSensationData(sensation, ToPerception(sensation));
        Set(sensation.SensationId, perceptedData);
    }

    public void EndSensation(Guid sensationId)
    {
        var perceptedData = GetByKey(sensationId);
        RemoveByKey(sensationId);
        Release(perceptedData);
    }

    protected override IEnumerable<string> GetPerceptionIdentifyingMarkers(SensationData sensation)
    {
        return GetSensationIdentifyingMarkers(sensation.SensationId);
    }

    protected override IEnumerable<string> GetPerceptionMarkers(SensationData sensation)
    {
        foreach (var marker in GetPerceptionIdentifyingMarkers(sensation))
        {
            yield return marker;
        }
        yield return PerceptionMarkersCore.Soma;
    }
}
