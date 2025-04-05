using UnityEngine;

internal static class PerceptionEntryExtensions
{
    public static bool IsDeepMemory(
        this PerceptionEntry perceptionEntry,
        MemoryParameters memoryParameters)
    {
        return perceptionEntry.Accessibility < memoryParameters.memoryAccessibilityThreshold
            && perceptionEntry.RetentionIntensity == null;
    }

    /// <summary>
    /// Classifies perception that became a memory.
    /// Doesn't take into account perceptions of sensory memory
    /// </summary>
    public static PerceptionState ClassifyStateInMemory(
        this PerceptionEntry perceptionEntry,
        MemoryParameters memoryParameters)
    {
        var a = perceptionEntry.Accessibility;

        if (a >= memoryParameters.workMemoryAccessibilityThreshold)
        {
            return PerceptionState.WorkMemory;
        }
        else if (a >= memoryParameters.memoryAccessibilityThreshold)
        {
            return PerceptionState.LongTermMemory;
        }
        else if (perceptionEntry.IsDeepMemory(memoryParameters))
        {
            return PerceptionState.DeepMemory;
        }
        else
        {
            return PerceptionState.Destructed;
        }
    }
}
