using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConstructPerceptionManagerBase<TConstructArgs> : MonoBehaviour
{
    [SerializeField, Required]
    protected PerceptionConstructGate perceptionConstructGate;

    protected abstract IEnumerable<ConstructPerceptionConfig> GetConfiguration();

    protected abstract string GetConstructKey(TConstructArgs constructArgs);
    protected abstract bool ShouldBeTrackedByConstruct(
        TConstructArgs constructArgs,
        PerceptionEntry construct,
        PerceptionEntry newPerception);
    protected virtual void HandleConstruct(
        PerceptionEntry construct,
        PerceptionEntry newPerception)
    {
        AggregateConstructByConfigs(construct, newPerception, GetConfiguration());
    }

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

    protected void AggregateConstructByConfigs(
        PerceptionEntry construct,
        PerceptionEntry newPerception,
        IEnumerable<ConstructPerceptionConfig> configs)
    {
        foreach (var config in configs)
        {
            config.ApplyToConstruct(construct, newPerception);
        }
    }
}
