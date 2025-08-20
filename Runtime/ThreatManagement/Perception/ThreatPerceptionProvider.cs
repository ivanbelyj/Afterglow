using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(IThreatKnowledgeProvider))]
[RequireComponent(typeof(PerceptionStorageRegistrar))]
public class ThreatPerceptionProvider : MonoBehaviour, IThreatPerceptionProvider
{
    private ISegregatedMemoryProvider segregatedMemoryProvider;
    private IThreatKnowledgeProvider threatKnowledgeProvider;

    private void Awake()
    {
        threatKnowledgeProvider = GetComponent<IThreatKnowledgeProvider>();
        segregatedMemoryProvider = GetSegregatedMemoryProvider();
        RegisterMemoryLayer();
    }

    protected virtual ISegregatedMemoryProvider GetSegregatedMemoryProvider()
    {
        return GetComponent<SegregatedMemoryManager>();
    }

    private void RegisterMemoryLayer()
    {
        GetComponent<PerceptionStorageRegistrar>()
            .RegisterStorage(
                CoreSegregatedPerceptionSources.PossibleThreat,
                new PerceptionListTracker(x => x.SatisfiesThreatNecessaryCondition()));
    }

    public IEnumerable<IThreatPerception> GetActualThreats(Guid estimateOriginEntityId)
    {
        return segregatedMemoryProvider
            .GetPerceptions(CoreSegregatedPerceptionSources.PossibleThreat)
            .Select(x =>
            {
                if (x.TryGetEntityId(out var entityId) && entityId == estimateOriginEntityId)
                {
                    // Don't consider entity itself as a threat
                    // from its estimation point of view
                    return null;
                }
                var knowledge = threatKnowledgeProvider.WhatAboutThreat(x);
                return knowledge == null ? null : new ThreatPerception(x, knowledge);
            })
            .Where(x => x != null)
            .GroupBy(x => x.EntityId)
            .Select(g => g.OrderByDescending(t => t.Timestamp).First())
            .ToList();
    }
}