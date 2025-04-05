using System.Collections.Generic;
using UnityEngine;

public interface IThreatHandler
{
    void Handle(IEnumerable<ThreatEstimate> threats);
}
