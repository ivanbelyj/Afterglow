using UnityEngine;
using Zor.UtilityAI.Core;

public class SwitchAgentBehaviourAction : Action, ISetupable<string>
{
    private string behaviourId;

    public void Setup(string behaviourId)
    {
        this.behaviourId = behaviourId;
    }

    protected override void OnBegin()
    {
        blackboard.SetClassValue(AIBlackboardPropertyNamesCore.CurrentBehaviourId, behaviourId);
    }
}
