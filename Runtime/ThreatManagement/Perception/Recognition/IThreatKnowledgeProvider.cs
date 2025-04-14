using UnityEngine;

public interface IThreatKnowledgeProvider
{
    /// <returns>null - not a threat</returns>
    ThreatKnowledge WhatAboutThreat(PerceptionEntry perceptionEntry);
}
