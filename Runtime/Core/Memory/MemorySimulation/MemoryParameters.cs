using UnityEngine;

[System.Serializable]
public record MemoryParameters
{
    /// <summary>
    /// Minimal memory entry accessibility value to treat it as work memory
    /// </summary>
    public float workMemoryAccessibilityThreshold = 0.9f;

    /// <summary>
    /// The minimum accessibility value required for memory existence
    /// (or moving important memory to deep memory)
    /// </summary>
    public float memoryAccessibilityThreshold = 0.05f;
}
