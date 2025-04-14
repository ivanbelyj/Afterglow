using System.Collections.Generic;
using UnityEngine;

public class ThreatTargeter : MonoBehaviour, IThreatTargeter
{
    [SerializeField]
    private ThreatTargetingParameters targetingParameters;

    private ThreatTargetingCalculator threatTargetingCalculator;

    protected virtual void Awake()
    {
        threatTargetingCalculator = new(targetingParameters);
    }

    public ThreatTargetingResult GetTargeting(IEnumerable<ThreatEstimate> threats)
    {
        return threatTargetingCalculator.CalculateTargeting(threats);
    }
}
