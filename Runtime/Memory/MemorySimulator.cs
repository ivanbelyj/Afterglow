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

    protected virtual void Awake()
    {
        perceptionStorage = new PerceptionStorage(memoryParameters);
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
            $"Active Memory Count: {perceptionStorage.GetActiveMemoryCount()}\n" +
            $"Passive Memory Count: {perceptionStorage.GetPassiveMemoryCount()}\n\n" +

            "=======================================\n" +
            "= Active Memory Verbal Representation =\n" +
            "=======================================\n" +
            perceptionStorage.GetActiveMemoryVerbalRepresentation(true) + "\n\n" +

            "========================================\n" +
            "= Passive Memory Verbal Representation =\n" +
            "========================================\n" +
            perceptionStorage.GetPassiveMemoryVerbalVerbalRepresentation(true);
    }
}