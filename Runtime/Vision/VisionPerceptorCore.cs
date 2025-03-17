using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(MemorySimulator))]
public abstract class VisionPerceptorCore : MonoBehaviour
{
    [SerializeField]
    [Tooltip(
        "Max vision radius that will be monitored. Typically checked infrequently " +
        "for optimization")]
    private float longVisionRadius = 30f;

    [SerializeField]
    [Tooltip(
        "Vision radius most commonly monitored. Typically checked with a frequency, " +
        "comparable to the reaction time of the human for optimization"
    )]
    private float commonVisionRadius = 10f;

    [SerializeField]
    private RadiusDetectingComponent radiusDetectingComponent;

    [SerializeField]
    private LayerMask sightLayerMask;
    private VisionSensoryMemoryStorage sensoryMemory;

    private void Start()
    {
        InitializeSensoryMemory();
        InitializeRadiusDetection();
    }

    protected abstract IRadiusDetectorFilter GetVisionRadiusDetectorFilter();

    private void InitializeRadiusDetection()
    {
        var filters = new List<IRadiusDetectorFilter> {
            GetVisionRadiusDetectorFilter()
        };

        radiusDetectingComponent.AddCommonRadiusMediumUpdateDetector(
            new() { MaxRadius = commonVisionRadius },
            sightLayerMask,
            OnRadiusEnter,
            OnRadiusExit,
            filters);

        radiusDetectingComponent.AddLongRadiusInfrequentUpdateDetector(
            new() { MinRadius = commonVisionRadius, MaxRadius = longVisionRadius },
            sightLayerMask,
            OnRadiusEnter,
            OnRadiusExit,
            filters);
    }

    private void InitializeSensoryMemory()
    {
        sensoryMemory = new(GameTimeProvider.Instance);
        GetComponent<MemorySimulator>().RegisterSensoryMemory(sensoryMemory);
    }

    private void OnRadiusEnter(object sender, RadiusDetectorEventArgs eventArgs)
    {
        ForEachSight(eventArgs.objectsWithDistance, sensoryMemory.CaptureSight);
    }

    private void OnRadiusExit(object sender, RadiusDetectorEventArgs eventArgs)
    {
        ForEachSight(
            eventArgs.objectsWithDistance,
            (sight, _) => sensoryMemory.ReleaseSight(sight));
    }

    private void ForEachSight(
        IEnumerable<(GameObject, float)> objectsWithDistance,
        Action<Sight, float> action)
    {
        var sights = GetSights(objectsWithDistance);
        foreach (var (sight, distance) in sights)
        {
            action(sight, distance);
        }
    }

    private IEnumerable<(Sight, float)> GetSights(
        IEnumerable<(GameObject, float)> objectsWithDistance)
    {
        return objectsWithDistance
            .Select(x => (x.Item1.GetComponent<Sight>(), x.Item2))
            .Where(x => x.Item1 != null);
    }
}
