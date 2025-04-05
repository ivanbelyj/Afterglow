using System;
using UnityEngine;

/// <summary>
/// Object, that can be percepted visually
/// </summary>
[RequireComponent(typeof(EntityProvider))]
public abstract class Sight : MonoBehaviour
{
    private EntityProvider entityProvider;

    [TextArea]
    [SerializeField]
    private string verbalRepresentation;

    public string VerbalRepresentation {
        get => verbalRepresentation;
        set => verbalRepresentation = value;
    }

    public Vector3 Position { get => transform.position; }

    [SerializeField]
    private float spatialRadius = 1f;
    public float SpatialRadius { get => spatialRadius; }

    public Guid EntityId => entityProvider.Entity.Id;

    private void Awake()
    {
        entityProvider = GetComponent<EntityProvider>();
    }
}
