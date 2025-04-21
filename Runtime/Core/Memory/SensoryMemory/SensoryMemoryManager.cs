using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MemorySimulator))]
public class SensoryMemoryManager : MonoBehaviour
{
    private PerceptionMemoryStorage perceptionStorage;

    private void Start()
    {
        perceptionStorage = GetComponent<MemorySimulator>().PerceptionStorage;
    }

    public void RegisterSensoryMemory(ISensoryMemoryStorageCore sensoryMemory)
    {
        sensoryMemory.SensoryPerceptionReleased += OnSensoryPerceptionReleased;
    }

    private void OnSensoryPerceptionReleased(
        object sender,
        SensoryPerceptionEventArgs eventArgs)
    {
        if (eventArgs.PerceptionEntry.TimestampTo == null)
        {
            eventArgs.PerceptionEntry.TimestampTo = GameTimeProvider.Instance.GetGameTime();
        }

        perceptionStorage.AddOrReplace(
            eventArgs.IdentifyingMarkers,
            eventArgs.PerceptionEntry);
    }
}
