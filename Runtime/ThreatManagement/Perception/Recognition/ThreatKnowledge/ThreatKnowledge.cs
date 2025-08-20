using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ThreatKnowledge
{
    public string threatType;

    /// <summary>
    /// Factors compound a threat together. For example, a threat of a person
    /// can be compounded from: 
    /// <list type="bullet">
    /// <item>common species characteristics (close combat min / max estimations, etc.)</item>
    /// <item>individuality (suspiciousness of a certain person, known strength, etc.)</item>
    /// <item>weapon (radius, potential, etc.)</item>
    /// <item>etc.</item>
    /// </list>
    /// </summary>
    public List<ThreatPerceptionCompoundData> factors;

    public ThreatPerceptionCompoundData GetCompoundData()
        => ThreatPerceptionCompoundData.Compound(factors);
}
