using UnityEngine;
using Zor.SimpleBlackboard.Core;

public class AgentBehaviourUpdater
{
    private readonly Blackboard blackboard;
    private readonly IAgentBehaviourProvider agentBehaviourProvider;

    private string currentBehaviourId;
    private IAgentBehaviour currentAgentBehaviour;

    public AgentBehaviourUpdater(Blackboard blackboard, IAgentBehaviourProvider agentBehaviourProvider)
    {
        this.blackboard = blackboard;
        this.agentBehaviourProvider = agentBehaviourProvider;
    }

    public void Update()
    {
        var newBehaviourId = GetNewBehaviourId();
        UpdateCurrentBehaviour(newBehaviourId);
    }

    private string GetNewBehaviourId()
    {
        if (!blackboard.TryGetClassValue(
            AIBlackboardPropertyNamesCore.CurrentBehaviourId,
            out string behaviourId))
        {
            Debug.LogError(
                $"{nameof(AgentBehaviourUpdater)} requires " +
                $"{AIBlackboardPropertyNamesCore.CurrentBehaviourId} " +
                $"to be set in {nameof(blackboard)}");
        }
        return behaviourId;
    }

    private void UpdateCurrentBehaviour(string newBehaviourId)
    {
        if (newBehaviourId != currentBehaviourId)
        {
            currentBehaviourId = newBehaviourId;
            currentAgentBehaviour?.EndBehaviour();

            currentAgentBehaviour = agentBehaviourProvider.GetBehaviour(currentBehaviourId);
            currentAgentBehaviour.StartBehaviour();
        }
    }
}
