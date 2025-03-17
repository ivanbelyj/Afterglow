using UnityEngine;

public class MemorySimulator : MonoBehaviour
{
    [SerializeField]
    private MemoryParameters memoryParameters;

    [SerializeField]
    private float simulationTickSeconds = 1f;

    public string debugOutput;

    private PerceptionStorage perceptionStorage;
    private float secondsSinceLastTick;

    public PerceptionStorage PerceptionStorage => perceptionStorage;

    private SensoryMemoryManager sensoryMemoryManager;

    protected virtual void Awake()
    {
        perceptionStorage = new PerceptionStorage(memoryParameters);
        sensoryMemoryManager = new(perceptionStorage);
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

    public void RegisterSensoryMemory(ISensoryMemoryStorage sensoryMemory)
    {
        sensoryMemoryManager.RegisterSensoryMemory(sensoryMemory);
    }

    public void AddMemory(PerceptionEntry perceptionEntry)
    {
        perceptionStorage.AddMemory(perceptionEntry);
    }

    public void AddMemory(
        string verbalRepresentation,
        float retentionIntensity,
        float accessibility,
        double? time = null)
    {
        var memory = new PerceptionEntry(verbalRepresentation, time)
        {
            RetentionIntensity = retentionIntensity,
            Accessibility = accessibility
        };
        perceptionStorage.AddMemory(memory);
    }

    protected virtual void SimulateMemoryDecay()
    {
        // Perform memory simulation
        perceptionStorage.TickMemoryDecay(simulationTickSeconds);

#if UNITY_EDITOR
        // Update debug output
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