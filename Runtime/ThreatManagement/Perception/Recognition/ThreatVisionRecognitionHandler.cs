using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThreatVisionRecognitionHandler : RecognitionHandlerBase
{
    [SerializeField]
    [Tooltip(
        "Max salience value for threat perceptions, " +
        "used to set scale of the salience range")]
    private float salienceScale = 1200;

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
        if (!perception.SatisfiesThreatNecessaryCondition())
        {
            // Threat recognition not triggered
            return new();
        }

        var knowledge = threatKnowledgeProvider.WhatAboutThreat(perception);

        return knowledge == null
            ? new() // Not a threat, recognition not triggered
            : new()
            {
                Intensity = 1f,
                Salience = GetThreatSalience(knowledge),
            };
    }

    private float GetThreatSalience(ThreatKnowledge knowledge)
    {
        // TODO: implement threat salience calculation.
        // Just get suspicion for now
        var compoundData = knowledge.GetCompoundData();
        return compoundData.PerceptorSuspicion.GetValue() * salienceScale;
    }
}
