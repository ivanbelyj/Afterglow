using System;
using UnityEngine;

public static class ConstructPerceptionConfigExtensions
{
    private static class SumAggregator<T> where T : struct
    {
        public static T Aggregate(T aggregated, T newValue)
        {
            if (typeof(T) == typeof(int))
                return (T)(object)((int)(object)aggregated + (int)(object)newValue);
            if (typeof(T) == typeof(float))
                return (T)(object)((float)(object)aggregated + (float)(object)newValue);
            if (typeof(T) == typeof(double))
                return (T)(object)((double)(object)aggregated + (double)(object)newValue);
            if (typeof(T) == typeof(decimal))
                return (T)(object)((decimal)(object)aggregated + (decimal)(object)newValue);
            if (typeof(T) == typeof(long))
                return (T)(object)((long)(object)aggregated + (long)(object)newValue);

            throw new NotSupportedException($"Sum aggregation is not supported for type {typeof(T)}");
        }
    }

    public static void ApplyToConstruct(
        this ConstructPerceptionConfig config,
        PerceptionEntry construct,
        PerceptionEntry newPerception)
    {
        foreach (var propertyConfig in config.Properties)
        {
            if (propertyConfig.ShouldAggregate(construct, newPerception))
            {
                propertyConfig.Aggregate(construct, newPerception);
            }
        }
    }

    public static ConstructPerceptionConfig Combine(
        this ConstructPerceptionConfig config1,
        ConstructPerceptionConfig config2)
    {
        var combined = new ConstructPerceptionConfig();
        combined.Properties.AddRange(config1.Properties);
        combined.Properties.AddRange(config2.Properties);
        return combined;
    }

    public static ConstructPerceptionConfig Combine(
        params ConstructPerceptionConfig[] configs)
    {
        var combined = new ConstructPerceptionConfig();
        foreach (var config in configs)
        {
            combined.Properties.AddRange(config.Properties);
        }
        return combined;
    }

    #region Group functions

    public static ConstructPerceptionConfigBuilder AggregateMax<T>(
        this ConstructPerceptionConfigBuilder builder,
        string aggregatedKey,
        string newPerceptionKey = null)
        where T : IComparable<T>
    {
        newPerceptionKey ??= aggregatedKey;

        builder.AddAggregatedProperty()
            .ExtractAs<T, T>(aggregatedKey, newPerceptionKey)
            .Aggregate((aggregated, newValue) =>
                newValue.CompareTo(aggregated) > 0 ? newValue : aggregated);

        return builder;
    }

    public static ConstructPerceptionConfigBuilder AggregateMin<T>(
        this ConstructPerceptionConfigBuilder builder,
        string aggregatedKey,
        string newPerceptionKey = null)
        where T : IComparable<T>
    {
        newPerceptionKey ??= aggregatedKey;

        builder.AddAggregatedProperty()
            .ExtractAs<T, T>(aggregatedKey, newPerceptionKey)
            .Aggregate((aggregated, newValue) =>
                newValue.CompareTo(aggregated) < 0 ? newValue : aggregated);

        return builder;
    }

    public static ConstructPerceptionConfigBuilder AggregateSum<T>(
        this ConstructPerceptionConfigBuilder builder,
        string aggregatedKey,
        string newPerceptionKey = null)
        where T : struct
    {
        newPerceptionKey ??= aggregatedKey;

        builder.AddAggregatedProperty()
            .ExtractAs<T, T>(aggregatedKey, newPerceptionKey)
            .Aggregate(SumAggregator<T>.Aggregate);

        return builder;
    }

    public static ConstructPerceptionConfigBuilder AggregateCount(
        this ConstructPerceptionConfigBuilder builder,
        string aggregatedKey,
        string newPerceptionKey)
    {
        builder.AddAggregatedProperty()
            .ExtractAs<int, object>(aggregatedKey, newPerceptionKey)
            .ShouldAggregateWhen((aggregated, newValue) => newValue != null)
            .Aggregate((aggregated, newValue) => aggregated + 1);

        return builder;
    }

    public static ConstructPerceptionConfigBuilder AggregateFirst<T>(
        this ConstructPerceptionConfigBuilder builder,
        string aggregatedKey,
        string newPerceptionKey = null)
    {
        newPerceptionKey ??= aggregatedKey;

        builder.AddAggregatedProperty()
            .ExtractAs<T, T>(aggregatedKey, newPerceptionKey)
            .ShouldAggregateWhen((aggregated, newValue) => aggregated == null)
            .Aggregate((aggregated, newValue) => newValue);

        return builder;
    }

    public static ConstructPerceptionConfigBuilder AggregateLast<T>(
        this ConstructPerceptionConfigBuilder builder,
        string aggregatedKey,
        string newPerceptionKey = null)
    {
        newPerceptionKey ??= aggregatedKey;

        builder.AddAggregatedProperty()
            .ExtractAs<T, T>(aggregatedKey, newPerceptionKey)
            .Aggregate((aggregated, newValue) => newValue);

        return builder;
    }

    public static ConstructPerceptionConfigBuilder AggregateBoolOr(
        this ConstructPerceptionConfigBuilder builder,
        string aggregatedKey,
        string newPerceptionKey = null)
    {
        newPerceptionKey ??= aggregatedKey;

        builder.AddAggregatedProperty()
            .ExtractAs<bool, bool>(aggregatedKey, newPerceptionKey)
            .Aggregate((aggregated, newValue) => aggregated || newValue);

        return builder;
    }

    public static ConstructPerceptionConfigBuilder AggregateBoolAnd(
        this ConstructPerceptionConfigBuilder builder,
        string aggregatedKey,
        string newPerceptionKey = null)
    {
        newPerceptionKey ??= aggregatedKey;

        builder.AddAggregatedProperty()
            .ExtractAs<bool, bool>(aggregatedKey, newPerceptionKey)
            .Aggregate((aggregated, newValue) => aggregated && newValue);

        return builder;
    }

    #endregion
}
