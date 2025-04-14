using System.Collections.Generic;
using UnityEngine;

public interface IThreatTargeter
{
    ThreatTargetingResult GetTargeting(IEnumerable<ThreatEstimate> threats);
}
