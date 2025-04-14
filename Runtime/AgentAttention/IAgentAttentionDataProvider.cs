using System.Collections.Generic;
using UnityEngine;

public interface IAgentAttentionDataProvider
{
    IEnumerable<AgentAttentionData> GetAttentionData();
}
