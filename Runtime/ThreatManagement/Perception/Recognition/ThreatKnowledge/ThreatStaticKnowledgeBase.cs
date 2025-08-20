using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "New Threat Static Knowledge Base",
    menuName = "Afterglow/Threat Static Knowledge Base",
    order = 52)]
public class ThreatStaticKnowledgeBase : ScriptableObject
{
    public List<ThreatEntityTypeStaticKnowledgeItem> items;
}

[System.Serializable]
public class ThreatEntityTypeStaticKnowledgeItem
{
    public string entityType;
    public ThreatEntityTypeKnowledge knowledge;
}
