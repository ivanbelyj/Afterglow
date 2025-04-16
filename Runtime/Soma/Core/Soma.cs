using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public record Soma
{
    [SerializeField]
    private List<SomaPart> parts;

    public List<SomaPart> Parts
    {
        get => parts;
        set => parts = value;
    }
}