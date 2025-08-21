using System;
using UnityEngine;

public abstract class ConstructPerceptionExtractedPropertyConfigBuilder
{
    internal abstract ConstructPerceptionPropertyConfig Build();
}

public class ConstructPerceptionExtractedPropertyConfigBuilder<TAggregated, TValue>
    : ConstructPerceptionExtractedPropertyConfigBuilder
{
    private readonly string aggregatedKey;
    private readonly string newPerceptionKey;
    private Func<TAggregated, TValue, bool> shouldAggregate;
    private Func<TAggregated, TValue, TAggregated> aggregate;

    public ConstructPerceptionExtractedPropertyConfigBuilder(
        string aggregatedKey,
        string newPerceptionKey)
    {
        this.aggregatedKey = aggregatedKey;
        this.newPerceptionKey = newPerceptionKey;
    }

    public ConstructPerceptionExtractedPropertyConfigBuilder<TAggregated, TValue> ShouldAggregateWhen(
        Func<TAggregated, TValue, bool> shouldAggregate)
    {
        this.shouldAggregate = shouldAggregate;
        return this;
    }

    public ConstructPerceptionExtractedPropertyConfigBuilder<TAggregated, TValue> Aggregate(
        Func<TAggregated, TValue, TAggregated> aggregate)
    {
        this.aggregate = aggregate;
        return this;
    }

    internal override ConstructPerceptionPropertyConfig Build()
    {
        return new ConstructPerceptionPropertyConfig
        {
            ShouldAggregate = (aggregated, newPerception) =>
            {
                if (!newPerception.TryGet<TValue>(
                    newPerceptionKey,
                    out var newValue))
                {
                    return false;
                }

                if (shouldAggregate == null)
                {
                    return true;
                }

                aggregated.TryGet<TAggregated>(
                    aggregatedKey,
                    out var aggregatedValue);

                return shouldAggregate(aggregatedValue, newValue);
            },
            Aggregate = (aggregated, newPerception) =>
            {
                if (aggregate != null)
                {
                    newPerception.TryGet<TValue>(
                        newPerceptionKey,
                        out var newValue);

                    aggregated.TryGet<TAggregated>(
                        aggregatedKey,
                        out var aggregatedValue);

                    aggregated.Set(
                        aggregatedKey,
                        aggregate.Invoke(aggregatedValue, newValue));
                }
            }
        };
    }
}
