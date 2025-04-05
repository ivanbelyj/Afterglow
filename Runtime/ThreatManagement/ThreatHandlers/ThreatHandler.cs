using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityProvider))]
[RequireComponent(typeof(IDisplayNameProvider))]
public class ThreatHandler : MonoBehaviour, IThreatHandler
{
    private string debugAgentName;
    private Guid debugEntityId;
    
    private void Start()
    {
        debugAgentName = GetComponent<IDisplayNameProvider>().GetDisplayName();
        debugEntityId = GetComponent<EntityProvider>().Entity.Id;
    }

    public void Handle(IEnumerable<ThreatEstimate> threats)
    {
        // foreach (var threat in threats)
        // {
        //     Debug.Log(FormatThreatEstimate(threat));
        // }
    }

    private string FormatThreatEstimate(ThreatEstimate threat)
    {
        string header = 
            $"<b><color=#9b59b6>Threat detected by agent</color></b>\n" +
            $"<b>Agent:</b> <color=#00cec9>{debugAgentName}</color> (entity Id: {debugEntityId}\n" +
            $"<b>Threat:</b> <color=#FFA3A3>{threat.ThreatType}</color>\n" +
            $"<b>Threat ID:</b> <color=#74B9FF>{threat.EntityId}</color>\n" +
            $"<b>Probability:</b> <color=#FDCB6E>{threat.Probability:P0}</color>\n";

        string potentialsSection = "<b>Threat Potentials:</b>\n";
        if (threat.Potentials != null && threat.Potentials.Length > 0)
        {
            foreach (var potential in threat.Potentials)
            {
                potentialsSection += $"  ├ <color=#a29bfe>{potential}</color>\n";
            }
        }
        else
        {
            potentialsSection += "  └ <color=#636e72><i>No potentials data</i></color>\n";
        }

        string divider = $"<color=#dfe6e9>{System.DateTime.Now:HH:mm:ss} ───────────────────</color>";
        
        return $"{header}{potentialsSection}{divider}";
    }
}
