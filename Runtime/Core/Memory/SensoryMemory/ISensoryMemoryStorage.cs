using System;
using UnityEngine;

public interface ISensoryMemoryStorage : IPerceptionSource
{
    event EventHandler<SensoryPerceptionEventArgs> SensoryPerceptionReleased;
}
