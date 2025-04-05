using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Percepted threat with elements of estimation
/// </summary>
public record ThreatPerceptionCompoundData
{
#region Potential
    public List<ThreatFactorPotential> Potentials { get; set; }

    /// <summary>
    /// Units per second
    /// </summary>
    public BoundCompoundProperty MovementSpeed { get; set; }
#endregion

#region Probability
    /// <summary>
    /// Current probability of encountering the threat again.
    /// 1: obvious right now or hiding; 0 - completely gone (typically such threats
    /// are not taken into account)
    /// </summary>
    public BoundCompoundProperty ThreatPresence { get; set; }

    /// <summary>
    /// The probability that the entity really poses a threat.
    /// 1: enemy. 0: not enemy (typically such 'threats' are not included)
    /// </summary>
    public BoundCompoundProperty PerceptorSuspicion { get; set; }

    /// <summary>
    /// Threat's awareness of the perceptor.
    /// 0 = hasn't noticed, 1 = fully aware and tracking.
    /// </summary>
    public BoundCompoundProperty ThreatAwareness { get; set; }

    /// <summary>
    /// Threat's activity or interest towards the perceptor.
    /// 0 = degree of interest is unknown or the threat is inactive towards
    /// the perceptor currently (focused on another opponent, for example);
    /// 1 = potential threat is fully focused on the perceptor
    /// </summary>
    public BoundCompoundProperty ThreatFocus { get; set; }
#endregion

#region Compound
    public static ThreatPerceptionCompoundData Compound(
        IEnumerable<ThreatPerceptionCompoundData> factors)
    {
        return new ThreatPerceptionCompoundData()
        {
            MovementSpeed = Sum(factors, x => x.MovementSpeed),
            PerceptorSuspicion = Sum(factors, x => x.PerceptorSuspicion),
            Potentials = factors
                .Select(x => x.Potentials)
                .Select(potentials => new ThreatFactorPotential () {
                    Potential = Sum(potentials, x => x.Potential),
                    Radius = Sum(potentials, x => x.Radius),
                    ActivationTimeSeconds = Sum(potentials, x => x.ActivationTimeSeconds),
                })
                .ToList(),

            ThreatAwareness = Sum(factors, x => x.ThreatAwareness),
            ThreatFocus = Sum(factors, x => x.ThreatFocus),
            ThreatPresence = Sum(factors, x => x.ThreatPresence),
        };
    }

    private static BoundCompoundProperty Sum<T>(
        IEnumerable<T> items,
        Func<T, BoundCompoundProperty> selector)
    {
        return items
            .Select(x => selector(x))
            .Aggregate((total, next) => total + next);
    }
#endregion
}
