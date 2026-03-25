using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Scriptable Objects/QuestData")]
public class QuestData : ScriptableObject
{
    public string Id;
    public string NpcId;
    public string Title;
    public string Description;

    [Tooltip("Time in seconds to complete the quest at base speed")]
    public float baseActionTime = 5f;

    [Tooltip("Skills that affec the speed of progress")]
    public List<SkillDefinition> relevantSkills;
}
