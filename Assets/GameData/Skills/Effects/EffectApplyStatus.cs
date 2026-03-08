using UnityEngine;

[CreateAssetMenu(fileName = "Effect_Status", menuName = "RPG System/Effects/Apply Status")]
public class EffectApplyStatus : SkillEffect
{
    public string effectName; // np. "Podpalenie"
    public StatusType type;
    public int duration;
    public int value;
    public int charges;
    public Sprite customIcon;
    public bool alwaysSucceeds = false; // Dla skilli typu Tarcza, które wchodz¹ zawsze

    public override void Execute(Combatant user, Combatant target, AttackResult result, float chance, Sprite skillIcon)
    {
        if (!result.isHit) return;
        // 1. Losowanie szansy (zostaje bez zmian)

        Debug.Log($"<color=orange>[TEST SZANSY] Próbujê na³o¿yæ: {effectName} | Otrzymana szansa z Managera: {chance}% | Czy AlwaysSucceeds: {alwaysSucceeds}</color>");
        float roll = Random.Range(0f, 100f);
        if (!alwaysSucceeds && roll > chance)
        {
            Debug.Log($"<color=yellow>Status {effectName} nie wszed³ (Roll: {roll} > Szansa: {chance}%)</color>");
            return;
        }

        
        

        // 2. Pobieramy dane z aktualnego poziomu skilla!
        // Szukamy poziomu skilla u u¿ytkownika (user), który wywo³a³ ten efekt
        CharacterSkill activeSkill = user.mySkills.Find(s => s.data.effects.Contains(this));
        SkillLevelData levelData = activeSkill?.data.GetLevelData(activeSkill.currentLevel);

        StatusEffect se = new StatusEffect();
        se.effectName = string.IsNullOrEmpty(effectName) ? this.name : effectName;
        se.type = type;
        se.duration = duration;
        se.icon = (customIcon != null) ? customIcon : skillIcon;
        se.isDamage = (type == StatusType.DamageOverTime);

        // LOGIKA PROGRESJI: 
        // Jeœli w levelData mamy coœ wpisane (>0), bierzemy to. Jeœli nie, bierzemy domyœlne z "kostki".
        se.remainingHits = (levelData != null && levelData.effectCharges > 0) ? levelData.effectCharges : this.charges;
        se.value = (levelData != null && levelData.effectValue > 0) ? levelData.effectValue : this.value;

        // 3. Nak³adanie (zostaje bez zmian)
        Combatant finalTarget = (type == StatusType.Shield) ? user : target;
        finalTarget.AddStatusEffect(se);
    }
}