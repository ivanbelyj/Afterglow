using System.Collections.Generic;
using UnityEngine;

public abstract class PerceptorBase<TRepresentation, TPerceptedData, TSensoryMemoryStorage> : MonoBehaviour
    where TPerceptedData : IPerceptedSensoryData<TRepresentation>
    where TSensoryMemoryStorage : ISensoryMemoryStorage<TRepresentation>
{
    protected TSensoryMemoryStorage sensoryMemory;

    private IEnricherProvider<TRepresentation> enricherProvider;

    protected virtual void Awake()
    {
        enricherProvider = GetEnricherProvider();
    }

    protected virtual void Start()
    {
        InitializeSensoryMemory();
    }

    protected abstract TSensoryMemoryStorage CreateSensoryMemoryStorage();

    protected virtual IEnricherProvider<TRepresentation> GetEnricherProvider()
    {
        return GetComponent<IEnricherProvider<TRepresentation>>();
    }

    protected void EnrichSensoryMemoryPerceptions()
    {
        foreach (var perceptedData in sensoryMemory.GetPerceptedData())
        {
            foreach (var enricher in enricherProvider.GetEnrichers())
            {
                enricher.EnrichPerception(
                    perceptedData.PerceptionEntry, 
                    perceptedData.Representation);
            }
        }
    }

    private void InitializeSensoryMemory()
    {
        sensoryMemory = CreateSensoryMemoryStorage();
        GetComponent<PerceptionStorageRegistrar>()
            .RegisterSensoryMemory(sensoryMemory);
    }
}
