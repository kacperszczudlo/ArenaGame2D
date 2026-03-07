using UnityEngine;

public abstract class SkillEffect : ScriptableObject
{
    // Zmieniamy definicjê na 5 argumentów (dodajemy Sprite skillIcon)
    public abstract void Execute(Combatant user, Combatant target, AttackResult result, float chance, Sprite skillIcon);
}