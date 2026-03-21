using UnityEngine;

[CreateAssetMenu(fileName = "Effect_DirectHeal", menuName = "RPG System/Effects/Direct Heal")]
public class EffectDirectHeal : SkillEffect
{
    public int baseHeal = 100; // Bazowa wartoœæ leczenia
    public float statMultiplier = 2.0f; // Jak mocno leczenie roœnie np. od statystyki "Knowledge"

    public override void Execute(Combatant user, Combatant target, AttackResult result, float chance, SkillLevelData levelData, Sprite skillIcon)
    {
        // Pobieramy bazê leczenia z danych poziomu (jeœli s¹), a jak nie, to z naszego klocka
        int healAmount = (levelData != null && levelData.effectValue > 0) ? levelData.effectValue : baseHeal;

        // Dodajemy bonus od statystyk lecz¹cego (zak³adam, ¿e masz zmienn¹ knowledge albo power)
        int bonus = Mathf.RoundToInt(user.power * statMultiplier);
        int totalHeal = healAmount + bonus;

        // Leczmy cel (Rycerz bêdzie celowa³ w samego siebie)
        target.Heal(totalHeal, "");

        

        // Opcjonalnie odtwarzamy ikonkê/efekt na postaci
        if (skillIcon != null) target.PlaySkillEffect(skillIcon);
    }
}