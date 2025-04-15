// public enum AIBehaviourType
// {
//     CriticalInaction,
//     CloseCombat,
//     Flee,
//     Consume,
//     Sleep,
//     DefaultActivity,
//     // ExploreWorld,
//     Rest, // Includes Chat, TurnOnMusic
//     Wander,
// }


public interface IAgentBehaviour
{
    void StartBehaviour();
    void EndBehaviour();
}