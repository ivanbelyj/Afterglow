using System.Collections.Generic;
using UnityEngine;

public class AgentBehaviourProvider : MonoBehaviour, IAgentBehaviourProvider
{
    private Dictionary<string, IAgentBehaviour> behavioursById = new();

    public void RegisterBehaviour(string id, IAgentBehaviour behaviour)
    {
        behavioursById.Add(id, behaviour);
    }

    public IAgentBehaviour GetBehaviour(string behaviourId)
    {
        if (!behavioursById.TryGetValue(behaviourId, out var result))
        {
            Debug.LogWarning($"Behaviour was not registered (id: {behaviourId})");
            return new MockAgentBehaviour();
        }
        return result;
    } 
}