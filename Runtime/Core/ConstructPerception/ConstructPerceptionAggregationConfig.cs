using System;
using System.Collections.Generic;
using UnityEngine;

public record ConstructPerceptionAggregationConfig<TAggregated, TValue>
{
    public string AggregatedPerceptionDataKey { get; set; }
    public string NewPerceptionDataKey { get; set; }
    public Func<TAggregated, TValue, bool> ShouldAggregate { get; set; }
    public Func<TAggregated, TValue, TAggregated> Aggregate { get; set; }

    public static ConstructPerceptionAggregationConfig<TValue, TValue> AggregateMin(
        string aggregatedPerceptionDataKey,
        string newPerceptionDataKey)
    {
        return CreateCompareAggregationConfig(
            aggregatedPerceptionDataKey,
            newPerceptionDataKey,
            x => x < 0);
    }

    public static ConstructPerceptionAggregationConfig<TValue, TValue> AggregateMax(
        string aggregatedPerceptionDataKey,
        string newPerceptionDataKey)
    {
        return CreateCompareAggregationConfig(
            aggregatedPerceptionDataKey,
            newPerceptionDataKey,
            x => x > 0);
    }

    public static ConstructPerceptionAggregationConfig<float, float> AggregateSum(
        string aggregatedPerceptionDataKey,
        string newPerceptionDataKey)
    {
        return new ConstructPerceptionAggregationConfig<float, float>
        {
            AggregatedPerceptionDataKey = aggregatedPerceptionDataKey,
            NewPerceptionDataKey = newPerceptionDataKey,
            ShouldAggregate = null,
            Aggregate = (aggregatedVal, newVal) => aggregatedVal + newVal
        };
    }

    public static ConstructPerceptionAggregationConfig<int, float> AggregateCount(
        string aggregatedPerceptionDataKey,
        string newPerceptionDataKey)
    {
        return new ConstructPerceptionAggregationConfig<int, float>
        {
            AggregatedPerceptionDataKey = aggregatedPerceptionDataKey,
            NewPerceptionDataKey = newPerceptionDataKey,
            ShouldAggregate = null,
            Aggregate = (aggregatedVal, newVal) => aggregatedVal + 1
        };
    }

    private static ConstructPerceptionAggregationConfig<TValue, TValue> CreateCompareAggregationConfig(
        string aggregatedPerceptionDataKey,
        string newPerceptionDataKey,
        Func<int, bool> shouldAggregate)
    {
        return new ConstructPerceptionAggregationConfig<TValue, TValue>
        {
            AggregatedPerceptionDataKey = aggregatedPerceptionDataKey,
            NewPerceptionDataKey = newPerceptionDataKey,
            ShouldAggregate = (existing, newVal) => shouldAggregate(Comparer<TValue>.Default.Compare(newVal, existing)),
            Aggregate = (_, newVal) => newVal
        };
    }
}
