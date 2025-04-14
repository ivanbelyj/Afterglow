using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Exposes agent's attention on entities
/// </summary>
public class AgentAttentionExposer : MonoBehaviour
{
    [SerializeField]
    private Sight sight;

    private IAgentAttentionDataProvider entityAttentionDataProvider;

    private void Awake()
    {
        entityAttentionDataProvider = GetEntityAttentionDataProvider();
    }

    public void Tick()
    {
        foreach (var attentionData in entityAttentionDataProvider.GetAttentionData())
        {
            sight.AddAgentAttentionData(attentionData);
        }
    }

    protected virtual IAgentAttentionDataProvider GetEntityAttentionDataProvider()
    {
        if (!TryGetComponent<IAgentAttentionDataProvider>(out var provider))
        {
            Debug.LogError($"{nameof(IAgentAttentionDataProvider)} is required");
        }
        return provider;
    }
}
