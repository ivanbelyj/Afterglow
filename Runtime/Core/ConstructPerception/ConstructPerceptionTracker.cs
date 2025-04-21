using System;
using System.Collections.Generic;
using UnityEngine;

public class ConstructPerceptionTracker : IPerceptionTrackingHandler
{
    private readonly PerceptionEntry constructPerception;
    private PerceptionConstructPredicate shouldBeTrackedByConstruct;
    private PerceptionConstructHandler constructHandler;

    // Called to updated hyper-construct for which the constructPerception
    // is related
    private Action<PerceptionEntry> hyperConstructHandler;

    private HashSet<string> subscribedHyperConstructsByConstructKey = new();

    public PerceptionEntry ConstructPerception => constructPerception;

    public ConstructPerceptionTracker(
        PerceptionEntry constructPerception,
        PerceptionConstructPredicate shouldBeTrackedByConstruct,
        PerceptionConstructHandler constructHandler)
    {
        this.constructPerception = constructPerception;
        this.shouldBeTrackedByConstruct = shouldBeTrackedByConstruct;
        this.constructHandler = constructHandler;
    }

    public void HandleHyperConstructSubscription(
        string hyperConstructKey,
        PerceptionEntry hyperConstruct,
        Action<PerceptionEntry> hyperConstructHandler)
    {
        if (!subscribedHyperConstructsByConstructKey.Contains(hyperConstructKey))
        {
            this.hyperConstructHandler += hyperConstructHandler;
            subscribedHyperConstructsByConstructKey.Add(hyperConstructKey);

            // If hyper-construct was forgotten, remove it via subscription
            hyperConstruct.PerceptionStateChanges += (sender, eventArgs) =>
            {
                if (eventArgs.NewState == PerceptionState.Destructed)
                {
                    subscribedHyperConstructsByConstructKey.Remove(hyperConstructKey);
                }
            };
        }
    }

    public void Track(PerceptionEntry perception)
    {
        HandleRelatedPerception(perception);
    }

    public void HandleRelatedPerception(PerceptionEntry perception)
    {
        if (shouldBeTrackedByConstruct(constructPerception, perception))
        {
            constructHandler?.Invoke(constructPerception, perception);

            // It's supposed that our construct was updated
            // so we call hyper-construct's callback to update it next
            hyperConstructHandler?.Invoke(constructPerception);
        }
    }
}
