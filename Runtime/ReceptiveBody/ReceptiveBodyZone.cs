using UnityEngine;
using System;

[Serializable]
public record ReceptiveBodyZone
{
    [SerializeField]
    private string name;

    [SerializeField]
    private string kind;

    public string Name
    {
        get => name;
        set => name = value;
    }

    /// <summary>
    /// Typically material or structure type. For example, "OrganicFlesh"
    /// </summary>
    public string Kind
    {
        get => kind;
        set => kind = value;
    }
}