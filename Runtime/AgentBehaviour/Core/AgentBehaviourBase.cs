using UnityEngine;
using Zor.SimpleBlackboard.Components;
using Zor.SimpleBlackboard.Core;

[RequireComponent(typeof(AgentBehaviourProvider))]
[RequireComponent(typeof(SimpleBlackboardContainer))]
public abstract class AgentBehaviourBase : MonoBehaviour, IAgentBehaviour
{
    [SerializeField]
    private string behaviourName;

    protected Blackboard blackboard;

    protected bool IsBehaviourActive { get; private set; }

    public virtual void StartBehaviour()
    {
        IsBehaviourActive = true;
    }

    public virtual void EndBehaviour()
    {
        IsBehaviourActive = true;
    }

    protected virtual void Awake()
    {
        blackboard = GetComponent<SimpleBlackboardContainer>().blackboard;
        GetComponent<AgentBehaviourProvider>().RegisterBehaviour(behaviourName, this);
    }
}
