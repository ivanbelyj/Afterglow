using System.Collections.Generic;
using UnityEngine;

public interface IPerceptionSource
{
    uint PerceptionSourceLayerMask { get; }
    IEnumerable<PerceptionEntry> GetPerceptions(params string[] markers);
}
