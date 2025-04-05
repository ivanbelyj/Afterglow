using UnityEngine;

public interface IThreatKnowledgeProvider
{
    ThreatKnowledge WhatAboutThreat(PerceptionEntry perceptionEntry);
}
