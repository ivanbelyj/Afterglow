using UnityEngine;

public enum PerceptionBasedNavigationType
{
    /// <summary>
    /// Using path-finding algorithm (in particular, A*)
    /// </summary>
    Known,
    /// <summary>
    /// Using heuristics and / or map research algorithms
    /// </summary>
    Intuitive
}
