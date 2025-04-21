using UnityEngine;

public enum PerceptionState
{
    /// <summary>
    /// Direct real-time perception. As a rule, it does not participate
    /// in memory simulation.
    /// </summary>
    SensoryMemory = 0,
    /// <summary>
    /// The most relevant perceptions from the past at the current moment
    /// </summary>
    WorkMemory = 1,
    /// <summary>
    /// Common memory
    /// </summary>
    LongTermMemory = 2,
    /// <summary>
    /// Memory that is inaccessible, but will not be destructed
    /// during the memory simulation
    /// </summary>
    DeepMemory = 3,
    /// <summary>
    /// Inaccessible memory that should be (or already) removed from the memory
    /// </summary>
    Destructed = 4
}
