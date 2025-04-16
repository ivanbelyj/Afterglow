using System;
using UnityEngine;

public interface ISensoryMemoryStorageCore : IPerceptionSource
{
    event EventHandler<SensoryPerceptionEventArgs> SensoryPerceptionReleased;
}
