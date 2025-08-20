using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConstructPerceptionManagerBase<TConstructArgs> : MonoBehaviour
{
    [SerializeField, Required]
    protected PerceptionConstructGate perceptionConstructGate;

    protected abstract string GetConstructKey(TConstructArgs constructArgs);
    protected abstract bool ShouldBeTrackedByConstruct(
        TConstructArgs constructArgs,
        PerceptionEntry construct,
        PerceptionEntry newPerception);
    protected abstract void HandleConstruct(
        PerceptionEntry construct,
        PerceptionEntry newPerception);
    protected virtual void HandleConstructBeforePush(
        TConstructArgs constructArgs,
        PerceptionEntry constructPerception)
    { }

    protected virtual (string key, PerceptionEntry construct) EnsureConstructPushed(
        TConstructArgs constructKeyData)
    {
        var constructKey = GetConstructKey(constructKeyData);
        return (constructKey, perceptionConstructGate.EnsureConstructPushed(
            constructKey,
            (construct, newPerception) => ShouldBeTrackedByConstruct(
                constructKeyData,
                construct,
                newPerception),
            HandleConstruct,
            constructPerception => HandleConstructBeforePush(
                constructKeyData,
                constructPerception)));
    }

    protected static float? GetPerceptionValue(
        PerceptionEntry perception,
        string key)
    {
        if (perception.TryGet(key, out float value))
        {
            return value;
        }
        return null;
    }

    protected void HandleConstruct<TAggregatedValue, TValue>(
        PerceptionEntry construct,
        PerceptionEntry newPerception,
        IEnumerable<ConstructPerceptionAggregationConfig<TAggregatedValue, TValue>> configs)
    {
        foreach (var config in configs)
        {
            AggregateValue(
                construct,
                newPerception,
                config.AggregatedPerceptionDataKey,
                config.NewPerceptionDataKey,
                config.ShouldAggregate,
                config.Aggregate);
        }
    }

    protected void AggregateValue<TAggregatedValue, TValue>(
        PerceptionEntry aggregatedPerception,
        PerceptionEntry newPerception,
        string aggregatedPerceptionDataKey,
        string newPerceptionDataKey,
        Func<TAggregatedValue, TValue, bool> shouldUpdate,
        Func<TAggregatedValue, TValue, TAggregatedValue> aggregate)
    {
        if (newPerception.TryGet<TValue>(
            newPerceptionDataKey,
            out var newValue))
        {
            var hasExistingValue = aggregatedPerception.TryGet<TAggregatedValue>(
                aggregatedPerceptionDataKey,
                out var existingValue);
            if (!hasExistingValue
                || shouldUpdate == null
                || shouldUpdate(existingValue, newValue))
            {
                aggregatedPerception.Set(
                    aggregatedPerceptionDataKey,
                    aggregate(existingValue, newValue));
            }
        }
    }
}
