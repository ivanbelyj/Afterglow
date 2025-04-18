using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public record ReceptiveBody
{
    [SerializeField]
    private List<ReceptiveBodyZone> zones;

    public List<ReceptiveBodyZone> Zones
    {
        get => zones;
        set => zones = value;
    }
}