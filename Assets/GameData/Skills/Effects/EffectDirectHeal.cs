using UnityEngine;

[CreateAssetMenu(fileName = "Effect_DirectHeal", menuName = "RPG System/Effects/Direct Heal")]
public class EffectDirectHeal : SkillEffect
{
    public int baseHeal = 100; // Bazowa wartoœæ leczenia
    public float statMultiplier = 2.0f; // Jak mocno leczenie roœnie od statystyki moc

    public override void Execute(Combatant user, Combatant target, AttackResult result, float chance, SkillLevelData levelData, Sprite skillIcon)
    {
        int healAmount = (levelData != null && levelData.effectValue > 0) ? levelData.effectValue : baseHeal;

        // Dodajemy bonus od statystyk lecz¹cego
        int bonus = Mathf.RoundToInt(user.power * statMultiplier);
        int totalHeal = healAmount + bonus;

        // Leczmy cel 
        target.Heal(totalHeal, "");

        if (skillIcon != null) target.PlaySkillEffect(skillIcon);
    }
}