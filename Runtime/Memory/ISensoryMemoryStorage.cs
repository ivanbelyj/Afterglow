using System;
using UnityEngine;

public class SensoryPerceptionReleasedEventArgs
{
    public PerceptionEntry perceptionEntry;
}

public interface ISensoryMemoryStorage
{
    event EventHandler<SensoryPerceptionReleasedEventArgs> SensoryPerceptionReleased;
}
