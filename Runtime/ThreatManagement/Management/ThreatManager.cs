
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ColorExtensions
{
    public static Color WithAlpha(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
}

public record ThreatProcessingResult
{
    public List<ThreatEstimate> CurrentThreatEstimates { get; set; }
    public ThreatTargetingResult Targeting { get; set; }
}

public record ThreatInfo
{
    public ThreatEstimate threatEstimate;
    public ThreatTargetingEstimate targeting;
}

[RequireComponent(typeof(IThreatPerceptionProvider))]
[RequireComponent(typeof(EntityProvider))]
public class ThreatManager : MonoBehaviour, IAgentAttentionDataProvider
{
    // Internal for using in debugging visualization
    internal ThreatEstimator ThreatEstimator { get; private set; }

    [SerializeField]
    private PerceptionStorageRegistrar perceptionStorageRegistrar;

    [SerializeField]
    private Sight sight;
    
    private EntityProvider entityProvider;

    private IThreatPerceptionProvider threatPerceptionProvider;
    private IThreatTargeter threatTargeter;
    private IThreatHandler[] threatHandlers;

    private ThreatProcessingResult currentProcessingResult;

    private void Awake()
    {
        entityProvider = GetComponent<EntityProvider>();
        threatPerceptionProvider = GetComponent<IThreatPerceptionProvider>();

        if (perceptionStorageRegistrar == null)
        {
            Debug.LogError($"{nameof(perceptionStorageRegistrar)} is required");
        }
        ThreatEstimator = new(perceptionStorageRegistrar);

        if (sight == null)
        {
            Debug.LogError($"{nameof(sight)} is required");
        }

        threatTargeter = GetThreatTargeter();
        if (threatTargeter == null)
        {
            Debug.LogError($"{nameof(threatTargeter)} must not be null");
        }

        threatHandlers = GetThreatHandlers();
        if (threatHandlers == null || threatHandlers.Length == 0)
        {
            Debug.LogWarning($"No threat handlers defined for {nameof(ThreatManager)}");
        }
    }

    public ThreatTargetingResult GetCurrentTargeting()
        => currentProcessingResult?.Targeting;

    public void Tick()
    {
        var threats = threatPerceptionProvider.GetActualThreats();

        currentProcessingResult = Process(threats);

        HandleThreats(currentProcessingResult);
    }

    public List<ThreatInfo> GetThreatsInfo()
    {
        return currentProcessingResult?.Targeting
            .OrderedTargetingEstimates
            .Select(x => new ThreatInfo() {
                targeting = x,
                threatEstimate = x.ThreatEstimate
            })
            .Concat(currentProcessingResult?.Targeting.Ignored
                .Select(x => new ThreatInfo() { threatEstimate = x })
                ?? Array.Empty<ThreatInfo>())
            .ToList();
    }

    // Internal to use the method in debugging visualization
    internal SpatialAwarenessPosition GetEstimateOriginPosition()
    {
        return new()
        {
            // Get directly by sight (but maybe it is not the most correct way)
            Position = sight.Position,
            Radius = sight.SpatialRadius,
            // All threats are percepted relatively to the subject,
            // so it's supposed that the perceptor is always awared about his
            // position in some way (even if he doesn't see or hear himself).
            // As an exception we use here fictive spatial awareness
            // (without actual perception entry)
            Perception = null
        };
    }

    protected virtual IThreatTargeter GetThreatTargeter()
    {
        return GetComponent<IThreatTargeter>();
    }

    protected virtual IThreatHandler[] GetThreatHandlers()
    {
        return GetComponents<IThreatHandler>().ToArray();
    }

    IEnumerable<AgentAttentionData> IAgentAttentionDataProvider.GetAttentionData()
    {
        var estimates = currentProcessingResult
            .Targeting
            .OrderedTargetingEstimates
            .Select(x => new { estimate =  x.ThreatEstimate, utility = x.Utility })
            .Concat(currentProcessingResult
                .Targeting
                .Ignored
                .Select(x => new { estimate = x, utility = 0f }));
        
        var currentTarget = currentProcessingResult
            .Targeting
            .TargetedThreat;

        var maxUtility = currentTarget?.Utility ?? 0f;
        
        foreach (var estimate in estimates)
        {
            var focusDecrease = Mathf.Approximately(maxUtility, 0f)
                ? 0
                : (estimate.utility / maxUtility);
            yield return new AgentAttentionData() {
                TargetEntityId = estimate.estimate.EntityId,
                ThreatAwareness = 1f,
                ThreatFocus = 1f - focusDecrease,
            };
        }
    }

    private ThreatProcessingResult Process(IEnumerable<IThreatPerception> threats)
    {
        var processingResult = new ThreatProcessingResult
        {
            CurrentThreatEstimates = ThreatEstimator
                .Estimate(entityProvider.Entity.Id, GetEstimateOriginPosition(), threats)
                .ToList()
        };

        processingResult.Targeting = threatTargeter.GetTargeting(
            processingResult.CurrentThreatEstimates);

        return processingResult;
    }

    private void HandleThreats(ThreatProcessingResult processingResult)
    {
        foreach (var handler in threatHandlers)
        {
            handler.Handle(processingResult);
        }
    }
}
