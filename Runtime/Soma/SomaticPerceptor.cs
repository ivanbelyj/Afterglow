using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IRecognizer<PerceivedPhysicalImpact>))]
public class SomaticPerceptor : PerceptorBase<PerceivedPhysicalImpact, PerceptedPhysicalImpact, SomaticSensoryMemoryStorage>
{
    [SerializeField, Required]
    private InterfaceField<IRecognizer<PerceivedPhysicalImpact>> recognizer;

    public void ApplyInstant(PerceivedPhysicalImpact physicalImpact)
    {
        sensoryMemory.BeginSensation(physicalImpact);
        sensoryMemory.EndSensation(physicalImpact.PhysicalImpactId);
    }
    
    protected override SomaticSensoryMemoryStorage CreateSensoryMemoryStorage()
    {
        return new SomaticSensoryMemoryStorage(recognizer.Value);
    }
}