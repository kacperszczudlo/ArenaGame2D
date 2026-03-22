using UnityEngine;

[CreateAssetMenu(fileName = "Effect_Status", menuName = "RPG System/Effects/Apply Status")]
public class EffectApplyStatus : SkillEffect
{
    public string effectName; 
    public StatusType type;
    public int duration;
    public int value;
    public int charges;
    public float multiplier; 
    public Sprite customIcon;
    public bool alwaysSucceeds = false; 
    public float hitChanceMod;

    public override void Execute(Combatant user, Combatant target, AttackResult result, float chance, SkillLevelData levelData, Sprite skillIcon)
    {
        if (!result.isHit && !alwaysSucceeds) return;

        float roll = Random.Range(0f, 100f);
        if (!alwaysSucceeds && roll > chance)
        {
            return;
        }

        StatusEffect se = new StatusEffect();
        se.effectName = string.IsNullOrEmpty(effectName) ? this.name : effectName;
        se.type = type;
        se.icon = (customIcon != null) ? customIcon : skillIcon;
        se.isDamage = (type == StatusType.DamageOverTime);

        if (levelData != null)
        {
            se.duration = levelData.effectDuration > 0 ? levelData.effectDuration : this.duration;
            se.remainingHits = levelData.effectCharges > 0 ? levelData.effectCharges : this.charges;
            se.value = levelData.effectValue != 0 ? levelData.effectValue : this.value;
            se.multiplier = levelData.effectMultiplier != 0f ? levelData.effectMultiplier : this.multiplier;

            se.hitChanceMod = levelData.effectHitChanceMod;
        }
        else
        {
            se.duration = this.duration;
            se.remainingHits = this.charges;
            se.value = this.value;
            se.multiplier = this.multiplier;
            se.hitChanceMod = this.hitChanceMod; 
        }

        if (se.type == StatusType.DamageOverTime)
        {
            se.value = Mathf.RoundToInt(result.rawDamage * se.multiplier);
            if (se.value < 1) se.value = 1;

            se.duration = se.remainingHits;
        }

        // KTO DOSTAJE STATUS?
        bool goesToUser = (type == StatusType.Shield || type == StatusType.Blessing || type == StatusType.Fury || type == StatusType.HealOverTime);
        Combatant finalTarget = goesToUser ? user : target;

        // Zabezpieczenie przed na³o¿eniem trucizny przy Uniku
        if (!goesToUser)
        {
            // SYTUACJA 1: Atak z obra¿eniami. Jeœli TakeDamage z³apa³o unik, my te¿ blokujemy status
            if (finalTarget.dodgedLastAttack)
            {
                return; 
            }

            // SYTUACJA 2: Czysty Urok (0 obra¿eñ).
            StatusEffect dodge = finalTarget.activeStatuses.Find(s => s.type == StatusType.Blessing && s.remainingHits > 0);
            if (dodge != null)
            {

                dodge.remainingHits--;
                finalTarget.ShowFloatingText("Cudowny Unik!", DamagePopup.PopupType.Miss);
                return;
            }
        }

        finalTarget.AddStatusEffect(se);
    }
}