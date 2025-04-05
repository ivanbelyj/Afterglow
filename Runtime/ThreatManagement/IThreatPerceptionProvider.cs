using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IThreatPerceptionProvider
{
    IEnumerable<IThreatPerception> GetActualThreats();
}
