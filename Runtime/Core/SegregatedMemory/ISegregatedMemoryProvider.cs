using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISegregatedMemoryProvider
{
    List<PerceptionEntry> GetPerceptions(
        string perceptionSourceKey,
        params string[] markers);
}
