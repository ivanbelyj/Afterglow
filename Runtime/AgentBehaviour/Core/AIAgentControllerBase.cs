using Mirror;
using UnityEngine;
using Zor.SimpleBlackboard.Components;
using Zor.SimpleBlackboard.Core;
using Zor.UtilityAI.Components;

[RequireComponent(typeof(IAgentBehaviourProvider))]
[RequireComponent(typeof(UtilityAIAgent))]
[RequireComponent(typeof(SimpleBlackboardContainer))]
public abstract class AIAgentControllerBase : NetworkBehaviour
{
    protected Blackboard blackboard;

    private UtilityAIAgent utilityAIAgent;
    private IAgentBehaviourProvider agentBehaviourProvider;

    private AgentBehaviourUpdater agentBehaviourUpdater;

    protected virtual void Awake()
    {
        blackboard = GetComponent<SimpleBlackboardContainer>().blackboard;
        utilityAIAgent = GetComponent<UtilityAIAgent>();
        agentBehaviourProvider = GetComponent<IAgentBehaviourProvider>();

        agentBehaviourUpdater = new(blackboard, agentBehaviourProvider);
    }

    [Server]
    protected virtual void Tick()
    {
        UpdateBehaviour();
    }

    protected void UpdateBehaviour()
    {
        utilityAIAgent.Tick();
        agentBehaviourUpdater.Update();
    }
}
