using System.Linq;
using UnityEngine;

public class SensoryMemoryManager
{
    private readonly PerceptionMemoryStorage perceptionStorage;

    public SensoryMemoryManager(PerceptionMemoryStorage perceptionStorage)
    {
        this.perceptionStorage = perceptionStorage;
    }

    public void RegisterSensoryMemory(ISensoryMemoryStorage sensoryMemory)
    {
        sensoryMemory.SensoryPerceptionReleased += OnSensoryPerceptionReleased;
    }

    private void OnSensoryPerceptionReleased(
        object sender,
        SensoryPerceptionEventArgs eventArgs)
    {
        perceptionStorage.AddOrReplace(
            eventArgs.UniqueMarker,
            eventArgs.PerceptionEntry);
    }
}
