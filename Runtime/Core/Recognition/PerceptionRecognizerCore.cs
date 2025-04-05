using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gate for creating every entry in perception flow
/// </summary>
public abstract class PerceptionRecognizerCore : MonoBehaviour
{
    private readonly List<IRecognitionHandler> recognitionHandlers = new();
    private IPerceptionTrackingStorage trackingStorage;

    protected virtual void Awake()
    {
        trackingStorage = GetTrackingStorage();
    }

    public void RegisterHandler(IRecognitionHandler recognitionHandler)
    {
        recognitionHandlers.Add(recognitionHandler);
    }

    public PerceptionEntry RecognizeTranscoded<TRepresentation>(
        IEnumerable<IPerceptionEnricher<TRepresentation>> enrichers,
        TRepresentation representation)
    {
        var perception = CreateBasePerception();
        
        if (perception != null)
        {
            foreach (var enricher in enrichers)
            {
                enricher.EnrichPerception(perception, representation);
            }

            perception.RetentionIntensity = HandleRecognition(perception);

            trackingStorage.Track(perception);
        }
        
        return perception;
    }

    protected virtual IPerceptionTrackingStorage GetTrackingStorage()
    {
        return GetComponent<PerceptionTrackingStorage>();
    }

    protected virtual PerceptionEntry CreateBasePerception()
    {
        return new(GameTimeProvider.Instance.GetGameTime());
    }

    protected abstract float ToPerceptionRetentionIntensity(PerceptionRecognitionEstimate perceptionRecognition);

    /// <returns>Perception retention intensity</returns>
    private float HandleRecognition(PerceptionEntry perceptionEntry)
    {
        if (recognitionHandlers.Count == 0)
        {
            Debug.LogError($"No recognition handlers registered");
        }

        float maxRetentionIntensity = float.MinValue;
        foreach (var handler in recognitionHandlers)
        {
            var recognition = handler.HandleRecognition(perceptionEntry);
            var currentRetentionIntensity = ToPerceptionRetentionIntensity(recognition);
            if (currentRetentionIntensity > maxRetentionIntensity)
            {
                maxRetentionIntensity = currentRetentionIntensity;
            }
        }
        return maxRetentionIntensity;
    }
}
