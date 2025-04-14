using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object, that can be percepted visually
/// </summary>
[RequireComponent(typeof(EntityProvider))]
public abstract class Sight : MonoBehaviour, IUntypedStorage
{
    private EntityProvider entityProvider;

    [Tooltip("Set if necessary. Required for some entities in some cases (in particular, for threat management)")]
    [SerializeField]
    private string entityType = "";

    [TextArea]
    [SerializeField]
    private string verbalRepresentation;

    public string EntityType => entityType;

    public string VerbalRepresentation {
        get => verbalRepresentation;
        set => verbalRepresentation = value;
    }

    public Vector3 Position { get => transform.position; }

    [SerializeField]
    private float spatialRadius = 1f;
    public float SpatialRadius { get => spatialRadius; }

    public Guid EntityId => entityProvider.Entity.Id;

    public Dictionary<string, object> SightData { get; private set; } = new();

    Dictionary<string, object> IUntypedStorage.Data => SightData;

    private void Awake()
    {
        entityProvider = GetComponent<EntityProvider>();
    }
}
