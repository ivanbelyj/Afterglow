using System.Collections.Generic;
using UnityEngine;

public interface IPerceptionSource
{
    string PerceptionSourceKey { get; }
    IEnumerable<PerceptionEntry> GetPerceptions(params string[] markers);
}
