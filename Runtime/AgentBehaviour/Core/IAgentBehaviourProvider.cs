using UnityEngine;

public interface IAgentBehaviourProvider
{
    void RegisterBehaviour(string behaviourId, IAgentBehaviour behaviour);
    IAgentBehaviour GetBehaviour(string behaviourId);
}
