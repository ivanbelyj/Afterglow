using UnityEngine;

public class MemorySimulator : MonoBehaviour
{
    [SerializeField]
    private SegregatedMemoryManager segregatedMemoryManager;

    [SerializeField]
    private MemoryParameters memoryParameters;

    [SerializeField]
    private float simulationTickSeconds = 1f;

    public string debugOutput;

    private PerceptionMemoryStorage perceptionStorage;
    private float secondsSinceLastTick;

    public PerceptionMemoryStorage PerceptionStorage => perceptionStorage;

    protected virtual void Awake()
    {
        perceptionStorage = new PerceptionMemoryStorage(
            CoreSegregatedPerceptionSources.SimulatedMemory,
            memoryParameters);

        if (segregatedMemoryManager == null)
        {
            Debug.LogError($"{nameof(segregatedMemoryManager)} is required");
        }
        segregatedMemoryManager.RegisterPerceptionSource(perceptionStorage);
        
        secondsSinceLastTick = 0f;
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        secondsSinceLastTick += Time.deltaTime;

        // Simulate memory decay at the specified interval
        if (secondsSinceLastTick >= simulationTickSeconds)
        {
            SimulateMemoryDecay();
            secondsSinceLastTick = 0f;
        }
    }

    public void AddMemory(PerceptionEntry perceptionEntry)
    {
        perceptionStorage.AddMemory(perceptionEntry);
    }

    protected virtual void SimulateMemoryDecay()
    {
        // Perform memory simulation
        perceptionStorage.TickMemoryDecay(simulationTickSeconds);

#if UNITY_EDITOR
        UpdateDebugOutput();
#endif
    }

    private void UpdateDebugOutput()
    {
        debugOutput = $"Memory Count: {perceptionStorage.MemoryCount}\n" +
            $"Deep Memory Count: {perceptionStorage.DeepMemoryCount}\n" +
            $"Work Memory Count: {perceptionStorage.GetWorkMemoryCount()}\n" +
            $"Long-term Memory Count: {perceptionStorage.GetLongTermMemoryCount()}\n\n" +

            "=================\n" +
            "=  Work Memory  =\n" +
            "=================\n" +
            perceptionStorage.GetWorkMemoryVerbalRepresentation(true) + "\n\n" +

            "======================\n" +
            "=  Long-term Memory  =\n" +
            "======================\n" +
            perceptionStorage.GetLongTermMemoryVerbalVerbalRepresentation(true);
    }
}