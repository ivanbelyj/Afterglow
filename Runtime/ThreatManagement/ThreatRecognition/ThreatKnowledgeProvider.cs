using UnityEngine;

public class ThreatKnowledgeProvider : MonoBehaviour, IThreatKnowledgeProvider
{
    public ThreatKnowledge WhatAboutThreat(PerceptionEntry perceptionEntry)
    {
        // TODO: get actual theat knowledges
        return new() {
            factors = new() { new ThreatPerceptionCompoundData() { 
                MovementSpeed = new(0, 1),
                PerceptorSuspicion = new(0, 1),
                Potentials = new() { new ThreatFactorPotential() {
                    ActivationTimeSeconds = new(0, 1),
                    Potential = new(0, 1),
                    Radius = new(0, 1)
                }},
                ThreatAwareness = new(0, 1),
                ThreatFocus = new(0, 1),
                ThreatPresence = new(0, 1)
            }}
        };
    }
}
