using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IRecognizer<SensationData>))]
public class SomaPerceptor : PerceptorBase<SensationData, PerceptedSensationData, SomaticSensoryMemoryStorage>
{
    // Todo: move out possibly
    [SerializeField]
    private Soma soma;

    public void ApplySensation(SensationData sensationData)
    {
        sensoryMemory.BeginSensation(sensationData);
    }

    protected override SomaticSensoryMemoryStorage CreateSensoryMemoryStorage()
    {
        return new SomaticSensoryMemoryStorage(GetComponent<IRecognizer<SensationData>>());
    }
}
