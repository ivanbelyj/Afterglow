using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using static PerceptionMarkerUtils;

public class SomaticSensoryMemoryStorage :
    SensoryMemoryStorageBase<Guid, PerceptedPhysicalImpact, PerceivedPhysicalImpact>
{
    public SomaticSensoryMemoryStorage(IRecognizer<PerceivedPhysicalImpact> perceptionRecognizer)
        : base(perceptionRecognizer)
    {

    }

    public override string PerceptionSourceKey => CoreSegregatedPerceptionSources.SomaticSensoryMemory;

    public void BeginSensation(PerceivedPhysicalImpact physicalImpact)
    {
        var perceptedData = new PerceptedPhysicalImpact(physicalImpact, ToPerception(physicalImpact));
        Set(physicalImpact.PhysicalImpactId, perceptedData);
        Capture(perceptedData);
    }

    public void EndSensation(Guid id)
    {
        var perceptedData = GetByKey(id);
        RemoveByKey(id);
        Release(perceptedData);
    }

    protected override IEnumerable<string> GetPerceptionIdentifyingMarkers(PerceivedPhysicalImpact physicalImpact)
    {
        return GetSensationIdentifyingMarkers(physicalImpact.PhysicalImpactId);
    }

    protected override IEnumerable<string> GetPerceptionMarkers(PerceivedPhysicalImpact physicalImpact)
    {
        foreach (var marker in GetPerceptionIdentifyingMarkers(physicalImpact))
        {
            yield return marker;
        }
        yield return PerceptionMarkersCore.Soma;
    }
}
