using UnityEngine;

[System.Serializable]
public record MemoryParameters
{
    /// <summary>
    /// Minimal memory entry accessibility value to treat it as active memory
    /// </summary>
    public float activeMemoryAccessibilityThreshold = 0.9f;

    /// <summary>
    /// The minimum accessibility value required for memory existence
    /// (or moving important memory to deep memory)
    /// </summary>
    public float memoryAccessibilityThreshold = 0.05f;
}
