using UnityEngine;

[CreateAssetMenu(fileName = "Voodoo Execution Effect", menuName = "RPG System/Effects/Voodoo Execution")]
public class EffectVoodooExecution : SkillEffect
{
    [Header("Ustawienia Egzekucji")]
    [Tooltip("Szansa (w %) na natychmiastowe zabicie, jeœli cel ma 3 kl¹twy.")]
    public float executionChance = 30f;

    public override void Execute(Combatant actor, Combatant target, AttackResult result, float baseChance, SkillLevelData levelData, Sprite icon)
    {
        // Jeœli atak spud³owa³, efekt nie wchodzi
        if (!result.isHit) return;

        // 1. SPRAWDZAMY CZY CEL MA "ŒWIÊT¥ TRÓJCÊ" DEBUFFÓW VOODOO
        bool hasPoison = target.activeStatuses.Exists(s => s.type == StatusType.Poison);
        bool hasBlindness = target.activeStatuses.Exists(s => s.type == StatusType.Blindness);
        bool hasVoodooCurse = target.activeStatuses.Exists(s => s.type == StatusType.VoodooCurse);

        if (hasPoison && hasBlindness && hasVoodooCurse)
        {
            // 2. LOSUJEMY SZANSÊ NA ZGON (od 0 do 100)
            float roll = Random.Range(0f, 100f);

            if (roll <= executionChance)
            {

                target.ShowFloatingText("NAG£Y ZGON!", DamagePopup.PopupType.CriticalDamage, icon);

                target.TakeDamage(99999, true, "Zgon", false, SkillCategory.NegativeCharm);
            }
            
        }
    }
}