using UnityEngine;

[CreateAssetMenu(fileName = "Effect_Status", menuName = "RPG System/Effects/Apply Status")]
public class EffectApplyStatus : SkillEffect
{
    public string effectName; // np. "Podpalenie"
    public StatusType type;
    public int duration;
    public int value;
    public int charges;
    public float multiplier; // Awaryjne pole, gdyby nie by³o poziomu
    public Sprite customIcon;
    public bool alwaysSucceeds = false; // Dla skilli typu Tarcza, które wchodz¹ zawsze
    public float hitChanceMod;

    // ZMIENIONA SYGNATURA: Odbieramy SkillLevelData bezpoœrednio z BattleManagera!
    public override void Execute(Combatant user, Combatant target, AttackResult result, float chance, SkillLevelData levelData, Sprite skillIcon)
    {
        if (!result.isHit && !alwaysSucceeds) return;

        Debug.Log($"<color=orange>[TEST SZANSY] Próbujê na³o¿yæ: {effectName} | Otrzymana szansa: {chance}% | Czy AlwaysSucceeds: {alwaysSucceeds}</color>");
        float roll = Random.Range(0f, 100f);
        if (!alwaysSucceeds && roll > chance)
        {
            Debug.Log($"<color=yellow>Status {effectName} nie wszed³ (Roll: {roll} > Szansa: {chance}%)</color>");
            return;
        }

        StatusEffect se = new StatusEffect();
        se.effectName = string.IsNullOrEmpty(effectName) ? this.name : effectName;
        se.type = type;
        se.icon = (customIcon != null) ? customIcon : skillIcon;
        se.isDamage = (type == StatusType.DamageOverTime);

        // LOGIKA PROGRESJI: Bierzemy z poziomu skilla (jeœli istnieje), w przeciwnym razie domyœlne.
        if (levelData != null)
        {
            se.duration = levelData.effectDuration > 0 ? levelData.effectDuration : this.duration;
            se.remainingHits = levelData.effectCharges > 0 ? levelData.effectCharges : this.charges;
            se.value = levelData.effectValue != 0 ? levelData.effectValue : this.value;
            se.multiplier = levelData.effectMultiplier != 0f ? levelData.effectMultiplier : this.multiplier;

            // NOWOŒÆ: Przepisujemy modyfikator celnoœci! (Nie sprawdzamy != 0, bo mo¿e byæ 0 przy zwyk³ych buffach)
            se.hitChanceMod = levelData.effectHitChanceMod;
        }
        else
        {
            se.duration = this.duration;
            se.remainingHits = this.charges;
            se.value = this.value;
            se.multiplier = this.multiplier;
            se.hitChanceMod = this.hitChanceMod; // Awaryjne pole dodane u góry klasy
        }

        if (se.type == StatusType.DamageOverTime)
        {
            se.value = Mathf.RoundToInt(result.rawDamage * se.multiplier);
            if (se.value < 1) se.value = 1;

            // NAPRAWA UI: Mówimy ikonce, ¿e rundy = stacki!
            se.duration = se.remainingHits;
        }

        // KTO DOSTAJE STATUS? (Wymieniamy tu wszystkie buffy, ¿eby Furia i Modlitwa na pewno sz³y na nas!)
        // KTO DOSTAJE STATUS?
        bool goesToUser = (type == StatusType.Shield || type == StatusType.Blessing || type == StatusType.Fury || type == StatusType.HealOverTime);
        Combatant finalTarget = goesToUser ? user : target;

        // --- NAPRAWA: Zabezpieczenie przed na³o¿eniem trucizny przy Uniku ---
        // --- NAPRAWA: Zabezpieczenie przed na³o¿eniem trucizny przy Uniku ---
        if (!goesToUser)
        {
            // SYTUACJA 1: Atak z obra¿eniami. Jeœli TakeDamage z³apa³o unik, my te¿ blokujemy status!
            if (finalTarget.dodgedLastAttack)
            {
                Debug.Log($"<color=cyan>Status {se.effectName} zablokowany, bo cios bazowy zosta³ unikniêty!</color>");
                return; // Przerywamy! (£adunek zosta³ ju¿ prawid³owo zabrany przez LogicBlessing)
            }

            // SYTUACJA 2: Czysty Urok (0 obra¿eñ). TakeDamage go zignorowa³o, wiêc my ³apiemy Modlitwê!
            StatusEffect dodge = finalTarget.activeStatuses.Find(s => s.type == StatusType.Blessing && s.remainingHits > 0);
            if (dodge != null)
            {
                Debug.Log($"<color=cyan>Status {se.effectName} (Urok) odparty przez Modlitwê!</color>");

                // Odbieramy ³adunek sami, bo LogicBlessing zignorowa³ ten atak
                dodge.remainingHits--;
                finalTarget.ShowFloatingText("Cudowny Unik!", DamagePopup.PopupType.Miss);
                return;
            }
        }

        // Jeœli nie by³o uniku, nak³adamy status normalnie
        finalTarget.AddStatusEffect(se);
    }
}