using System.Collections.Generic;
using UnityEngine;

public class ConstructPerceptionConfigBuilder
{
    private readonly ConstructPerceptionConfig config = new();
    private readonly List<ConstructPerceptionPropertyConfigBuilder> propertyBuilders = new();

    public ConstructPerceptionPropertyConfigBuilder AddAggregatedProperty()
    {
        var builder = new ConstructPerceptionPropertyConfigBuilder();
        propertyBuilders.Add(builder);
        return builder;
    }

    public ConstructPerceptionConfig Build()
    {
        foreach (var propertyBuilder in propertyBuilders)
        {
            config.Properties.Add(propertyBuilder.Build());
        }
        return config;
    }
}
