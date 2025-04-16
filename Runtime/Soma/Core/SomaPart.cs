using UnityEngine;
using System;

[Serializable]
public record SomaPart
{
    [SerializeField]
    private string kind;
    
    [SerializeField]
    private string name;

    /// <summary>
    /// Typically material. For example, "OrganicFlesh"
    /// </summary>
    public string Kind
    {
        get => kind;
        set => kind = value;
    }

    public string Name
    {
        get => name;
        set => name = value;
    }
}