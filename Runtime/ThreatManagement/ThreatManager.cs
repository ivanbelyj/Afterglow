
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(IThreatPerceptionProvider))]
public class ThreatManager : MonoBehaviour
{
    private IThreatPerceptionProvider threatPerceptionProvider;
    private ThreatEstimator threatEstimator;
    private List<IThreatHandler> threatHandlers;
    private List<ThreatEstimate> currentThreatEstimates = new();

    private void Awake()
    {
        threatPerceptionProvider = GetComponent<IThreatPerceptionProvider>();
        threatEstimator = new();
        threatHandlers = GetThreatHandlers();
        if (threatHandlers.Count == 0)
        {
            Debug.LogError($"{nameof(threatHandlers)} must not be empty");
        }
    }

    public List<ThreatEstimate> GetCurrentThreatEstimates()
    {
        return currentThreatEstimates;
    }

    public void Tick()
    {
        var threats = threatPerceptionProvider.GetActualThreats();
        currentThreatEstimates = threatEstimator.Estimate(threats).ToList();
        
        ExecuteThreatResponse(currentThreatEstimates);
    }

    protected virtual List<IThreatHandler> GetThreatHandlers()
    {
        return GetComponents<IThreatHandler>().ToList();
    }

    private void ExecuteThreatResponse(IEnumerable<ThreatEstimate> threats)
    {
        foreach (var handler in threatHandlers)
        {
            handler.Handle(threats);
        }
    }
}
