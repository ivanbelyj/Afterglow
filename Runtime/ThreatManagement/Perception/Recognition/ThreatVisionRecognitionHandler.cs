using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThreatVisionRecognitionHandler : RecognitionHandlerBase
{
    private IThreatKnowledgeProvider threatKnowledgeProvider;

    protected override void Awake()
    {
        base.Awake();
        threatKnowledgeProvider = GetThreatKnowledgeProvider();
    }

    public override PerceptionRecognitionEstimate HandleRecognition(
        PerceptionEntry perceptionEntry)
    {
        return PerceptThreat(perceptionEntry);
    }

    protected virtual IThreatKnowledgeProvider GetThreatKnowledgeProvider()
    {
        if (!TryGetComponent<IThreatKnowledgeProvider>(out var provider))
        {
            Debug.LogError($"{nameof(IThreatKnowledgeProvider)} is required");
        }
        return provider;
    }

    private PerceptionRecognitionEstimate PerceptThreat(PerceptionEntry perception)
    {
        if (!perception.CanBeThreat())
        {
            // Threat recognition not triggered
            return new();
        }

        var knowledge = threatKnowledgeProvider.WhatAboutThreat(perception);

        // TODO:
        return new() {
            Intensity = 10f,
            Salience = 10f,
        };
    }
}
