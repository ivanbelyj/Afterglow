using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PerceptionTrackingStorageRegistrar))]
public class PerceptionBasedSpatialAwareness : MonoBehaviour, ISpatialAwarenessProvider
{
    [SerializeField]
    private float trackedPositionsUpdateIntervalInSeconds = 0.2f;

    private ISegregatedMemoryProvider segregatedMemoryProvider;

    private PerceptionPositionTracker perceptionPositionTracker = new();
    private TimedAction timedAction = new();

    private void Awake()
    {
        timedAction.AddAction(
            () => perceptionPositionTracker.Tick(),
            trackedPositionsUpdateIntervalInSeconds);
        
        segregatedMemoryProvider = GetSegregatedMemoryProvider();

        RegisterMemoryLayer();
    }

    private void Update()
    {
        timedAction.Update();
    }

    private void RegisterMemoryLayer()
    {
        GetComponent<PerceptionTrackingStorageRegistrar>()
            .RegisterStorage(
                (uint)SegregatedPerceptionLayerCore.SpatialAwareness,
                new PerceptionListTracker(entry => entry.HasPosition()));
    }

    protected virtual ISegregatedMemoryProvider GetSegregatedMemoryProvider()
    {
        return GetComponent<SegregatedMemoryManager>();
    }

    public IEnumerable<SpatialAwarenessPosition> GetKnownPositions()
    {
        return segregatedMemoryProvider
            .GetPerceptions((uint)SegregatedPerceptionLayerCore.SpatialAwareness)
            .Select(x => x.GetSpatialAwarenessPosition());
    }

    /// <summary>
    /// Please, track Transform only when it's directly percepted in real-time,
    /// then untrack it when it becomes a memory
    /// </summary>
    public void TrackPositionRegularly(PerceptionEntry perceptionEntry, Transform transform)
    {
        perceptionPositionTracker.Track(perceptionEntry, transform);
    }

    public void Untrack(PerceptionEntry perceptionEntry)
    {
        perceptionPositionTracker.Untrack(perceptionEntry);
    }
}
