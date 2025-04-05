using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ThreatStorage))]
[RequireComponent(typeof(IThreatKnowledgeProvider))]
public class ThreatVisionRecognitionHandler : RecognitionHandlerBase
{
    private ThreatStorage threatStorage;
    private IThreatKnowledgeProvider threatKnowledgeBase;

    protected override void Awake()
    {
        base.Awake();
        threatStorage = GetComponent<ThreatStorage>();
        threatKnowledgeBase = GetComponent<IThreatKnowledgeProvider>();
    }

    public override PerceptionRecognitionEstimate HandleRecognition(
        PerceptionEntry perceptionEntry)
    {
        var threat = PerceptThreat(perceptionEntry, out var recognition);
        if (threat != null)
        {
            threatStorage.CaptureThreat(threat);
        }

        return recognition;
    }

    private IThreatPerception PerceptThreat(
        PerceptionEntry perception,
        out PerceptionRecognitionEstimate recognition)
    {
        if (!CanBeThreat(perception))
        {
            // Threat recognition not triggered
            recognition = new() {
                Intensity = 0,
                Salience = 0,
            };
            return null;
        }

        var knowledge = threatKnowledgeBase.WhatAboutThreat(perception);

        // TODO:
        recognition = new() {
            Intensity = 10f,
            Salience = 10f,
        };

        return new ThreatPerception(
            perception,
            knowledge);
    }

    private bool CanBeThreat(PerceptionEntry perception)
    {
        return perception.HasEntityId() && perception.HasPosition();
    }
}

// Todo: Get all properties by knowledge of type / entity.
// Ideally, knowledge are dynamic (so NPC can learn)
