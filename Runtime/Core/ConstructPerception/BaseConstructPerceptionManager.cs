using System;
using System.Collections.Generic;
using UnityEngine;

public record ConstructPerceptionAggregationConfig<TValue>
{
    public string AggregatedPerceptionDataKey { get; set; }
    public string NewPerceptionDataKey { get; set; }
    public Func<TValue, TValue, bool> ShouldAggregate { get; set; }
    public Func<TValue, TValue, TValue> Aggregate { get; set; }

    public static ConstructPerceptionAggregationConfig<TValue> AggregateMin(
        string aggregatedPerceptionDataKey,
        string newPerceptionDataKey)
    {
        return CreateCompareAggregationConfig(
            aggregatedPerceptionDataKey,
            newPerceptionDataKey,
            x => x < 0);
    }

    public static ConstructPerceptionAggregationConfig<TValue> AggregateMax(
        string aggregatedPerceptionDataKey,
        string newPerceptionDataKey)
    {
        return CreateCompareAggregationConfig(
            aggregatedPerceptionDataKey,
            newPerceptionDataKey,
            x => x > 0);
    }

    private static ConstructPerceptionAggregationConfig<TValue> CreateCompareAggregationConfig(
        string aggregatedPerceptionDataKey,
        string newPerceptionDataKey,
        Func<int, bool> shouldAggregate)
    {
        return new ConstructPerceptionAggregationConfig<TValue>
        {
            AggregatedPerceptionDataKey = aggregatedPerceptionDataKey,
            NewPerceptionDataKey = newPerceptionDataKey,
            ShouldAggregate = (existing, newVal) => shouldAggregate(Comparer<TValue>.Default.Compare(newVal, existing)),
            Aggregate = (_, newVal) => newVal
        };
    }
}

public abstract class BaseConstructPerceptionManager : MonoBehaviour
{
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

    protected void HandleConstruct<TValue>(
        PerceptionEntry construct,
        PerceptionEntry newPerception,
        IEnumerable<ConstructPerceptionAggregationConfig<TValue>> configs)
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

    protected void AggregateValue<TValue>(
        PerceptionEntry aggregatedPerception,
        PerceptionEntry newPerception,
        string aggregatedPerceptionDataKey,
        string newPerceptionDataKey,
        Func<TValue, TValue, bool> shouldUpdate,
        Func<TValue, TValue, TValue> aggregate)
    {
        if (newPerception.TryGet<TValue>(
            newPerceptionDataKey,
            out var newValue))
        {
            var hasExistingValue = aggregatedPerception.TryGet<TValue>(
                aggregatedPerceptionDataKey,
                out var existingValue);
            if (!hasExistingValue || shouldUpdate(existingValue, newValue))
            {
                aggregatedPerception.Set(
                    aggregatedPerceptionDataKey,
                    aggregate(existingValue, newValue));
            }
        }
    }
}
