using UnityEngine;

public class SensoryMemoryManager
{
    private readonly PerceptionStorage perceptionStorage;

    public SensoryMemoryManager(PerceptionStorage perceptionStorage)
    {
        this.perceptionStorage = perceptionStorage;
    }

    public void RegisterSensoryMemory(ISensoryMemoryStorage sensoryMemory)
    {
        sensoryMemory.SensoryPerceptionReleased += OnSensoryPerceptionReleased;
    }

    private void OnSensoryPerceptionReleased(
        object sender,
        SensoryPerceptionReleasedEventArgs eventArgs)
    {
        // TODO: implement
        // Deduplicate ?
        perceptionStorage.AddMemory(eventArgs.perceptionEntry);
    }
}
