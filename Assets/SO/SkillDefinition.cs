using UnityEngine;

[CreateAssetMenu(fileName = "SkillDefinition", menuName = "Scriptable Objects/SkillDefinition")]
public class SkillDefinition : ScriptableObject
{
    public string skillName;
    public Sprite icon;

    public AnimationClip actionAnimation;

    public AnimationCurve successChance;
    public AnimationCurve actionSpeed;
}
