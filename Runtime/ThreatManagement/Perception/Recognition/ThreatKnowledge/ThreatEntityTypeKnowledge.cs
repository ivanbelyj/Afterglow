using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public record ThreatEntityTypeKnowledge
{
    [SerializeField]
    private BoundCompoundProperty movementSpeed;
    [SerializeField]
    private BoundCompoundProperty typicalSuspicion;
    [SerializeField]
    private List<ThreatFactorPotential> basePotentials;

    public BoundCompoundProperty MovementSpeed
    {
        get => movementSpeed; set => movementSpeed = value;
    }
    public BoundCompoundProperty TypicalSuspicion
    {
        get => typicalSuspicion; set => typicalSuspicion = value;
    }
    public List<ThreatFactorPotential> BasePotentials
    {
        get => basePotentials; set => basePotentials = value;
    }
}
