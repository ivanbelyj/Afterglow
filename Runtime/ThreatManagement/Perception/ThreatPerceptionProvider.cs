using System;
using System.Collections.Generic;
using System.Drawing;
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
                CoreSegregatedPerceptionSources.Threat,
                new PerceptionListTracker(x => x.CanBeThreat()));
    }

    public IEnumerable<IThreatPerception> GetActualThreats()
    {
        return segregatedMemoryProvider
            .GetPerceptions(CoreSegregatedPerceptionSources.Threat)
            .Select(x => {
                var knowledge = threatKnowledgeProvider.WhatAboutThreat(x);
                return knowledge == null ? null : new ThreatPerception(x, knowledge);
            })
            .Where(x => x != null)
            .GroupBy(x => x.EntityId)
            .Select(g => g.OrderByDescending(t => t.Timestamp).First())
            .ToList();
    }
}