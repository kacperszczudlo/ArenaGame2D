using UnityEngine;

[CreateAssetMenu(fileName = "Effect_DeepFreeze", menuName = "RPG System/Effects/Deep Freeze (Stun)")]
public class EffectDeepFreeze : SkillEffect
{
    public int duration = 2; // Ile rund postaæ bêdzie staæ jak sopel lodu

    public override void Execute(Combatant actor, Combatant target, AttackResult result, float baseChance, SkillLevelData levelData, Sprite skillIcon)
    {
        // Jeœli atak ca³kowicie spud³owa³, to nawet nie próbujemy
        if (!result.isHit) return;

        // --- KLUCZOWY WARUNEK: Czy cel ma ju¿ na sobie zwyk³e Zamro¿enie (Freeze)? ---
        bool hasRegularFreeze = target.activeStatuses.Exists(s => s.type == StatusType.Freeze);

        // Odpalamy twardego Stuna TYLKO jeœli gracz by³ ju¿ wych³odzony
        if (hasRegularFreeze)
        {
            // Sprawdzamy szansê na wejœcie G³êbokiego Zamro¿enia
            if (Random.Range(0f, 100f) <= baseChance)
            {
                // Tworzymy "klocek" statusu
                StatusEffect freezeEffect = new StatusEffect
                {
                    effectName = "G³êbokie Zamro¿enie",
                    type = StatusType.DeepFreeze,
                    duration = duration,
                    remainingHits = 0,
                    value = 0,
                    multiplier = 0f,
                    hitChanceMod = 0,
                    icon = skillIcon
                };

                // Nak³adamy poprawn¹ funkcj¹
                target.AddStatusEffect(freezeEffect);

                // Kolorujemy postaæ na niebiesko
                target.ToggleFreezeVisual(true);

                target.ShowFloatingText("ZAMRO¯ENIE!", DamagePopup.PopupType.CriticalDamage, skillIcon);
                if (skillIcon != null) target.PlaySkillEffect(skillIcon);
            }
        }
        else
        {
            // Jeœli boss u¿yje tego skilla za wczeœnie (gracz nie ma Freeze), to nic siê nie dzieje!
            // Mo¿emy opcjonalnie wyœwietliæ, ¿e combo siê nie uda³o:
            // target.ShowFloatingText("Brak wych³odzenia!", DamagePopup.PopupType.Miss);
        }
    }
}