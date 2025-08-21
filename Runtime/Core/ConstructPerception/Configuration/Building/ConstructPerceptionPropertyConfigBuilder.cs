using System;
using UnityEngine;

public class ConstructPerceptionPropertyConfigBuilder
{
    private readonly ConstructPerceptionPropertyConfig propertyConfig = new();
    private ConstructPerceptionExtractedPropertyConfigBuilder extractedBuilder;

    public ConstructPerceptionExtractedPropertyConfigBuilder<TAggregated, TValue>
        ExtractAs<TAggregated, TValue>(string aggregatedKey, string newPerceptionKey)
    {
        var builder = new ConstructPerceptionExtractedPropertyConfigBuilder<TAggregated, TValue>(
            aggregatedKey,
            newPerceptionKey
        );
        extractedBuilder = builder;
        return builder;
    }

    public ConstructPerceptionPropertyConfigBuilder ShouldAggregateWhen(
        Func<PerceptionEntry, PerceptionEntry, bool> shouldAggregate)
    {
        propertyConfig.ShouldAggregate = shouldAggregate;
        return this;
    }

    public ConstructPerceptionPropertyConfigBuilder Aggregate(
        Action<PerceptionEntry, PerceptionEntry> aggregate)
    {
        propertyConfig.Aggregate = aggregate;
        return this;
    }

    internal ConstructPerceptionPropertyConfig Build()
    {
        if (extractedBuilder != null)
        {
            var extractedConfig = extractedBuilder.Build();

            propertyConfig.ShouldAggregate = extractedConfig.ShouldAggregate;
            propertyConfig.Aggregate = extractedConfig.Aggregate;
        }

        return propertyConfig;
    }
}
