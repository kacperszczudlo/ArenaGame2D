using UnityEngine;

public abstract class SkillEffect : ScriptableObject
{
    public abstract void Execute(Combatant user, Combatant target, AttackResult result, float chance, SkillLevelData levelData, Sprite skillIcon);
}