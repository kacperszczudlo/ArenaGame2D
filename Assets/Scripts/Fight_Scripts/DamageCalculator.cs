using UnityEngine;


public static class DamageCalculator
{
    public static AttackResult ProcessAttack(Combatant attacker, Combatant defender, CharacterSkill skill, int allocatedPA)
    {
        AttackResult result = new AttackResult();
        SkillData data = skill.data;
        SkillLevelData levelData = data.GetLevelData(skill.currentLevel);

        float attackerStat = 0;
        float defenderStat = 0;
        int defenderPA = 0;

        switch (data.category)
        {
            case SkillCategory.MeleePhysical:
                attackerStat = attacker.agility; defenderStat = defender.agility; defenderPA = defender.defenseMeleePA; break;
            case SkillCategory.RangedPhysical:
                attackerStat = attacker.agility; defenderStat = defender.agility; defenderPA = defender.defenseRangedPA; break;
            case SkillCategory.RangedMagic:
                attackerStat = attacker.knowledge; defenderStat = (defender.agility + defender.knowledge) / 2f; defenderPA = defender.defenseRangedPA; break;
            case SkillCategory.NegativeCharm:
                attackerStat = attacker.knowledge; defenderStat = defender.knowledge; defenderPA = defender.defenseMentalPA; break;
            case SkillCategory.PositiveCharm:
                break;
        }

        float PA_AttackMod = 1.0f + (allocatedPA * 0.2f);
        float PA_DefenseMod = 1.0f + (defenderPA * 0.2f);

        // --- FIX MNO¯NIKA CELNOŒCI (modU) ---
        // Pobieramy Twoje 2.4 lub 1.1 z Inspektora
        float hitModU = levelData != null ? levelData.hitChanceBonus : 1.0f;

        // Zabezpieczenie: jeœli wpisa³eœ w Unity "0" (np. na starych skillach), 
        // traktujemy to jako standardowe x1.0, ¿eby atak nie mia³ 0% szans.
        if (hitModU <= 0.01f) hitModU = 1.0f;

        // TERAZ hitModU prawid³owo mno¿y si³ê ataku (zgodnie z Twoim GDD!)
        float attackPower = (40f + attackerStat + attacker.currentLevel) * PA_AttackMod * hitModU;
        float defensePower = (40f + defenderStat + defender.currentLevel) * PA_DefenseMod;

        float hitChance = 0f;

        if (data.category == SkillCategory.PositiveCharm)
        {
            float playerPower = allocatedPA * 20f;
            float difficulty = (levelData != null) ? levelData.selfCastDifficulty : 100f;
            hitChance = (playerPower / difficulty) * 100f;
            result.damageDealt = 0;
        }
        else
        {
            if (defenderPA == 0) hitChance = 100f;
            else
            {
                hitChance = (attackPower / (attackPower + defensePower)) * 100f;
                // Dodajemy tylko karê p³ask¹ z Furii (-20%)
                hitChance *= attacker.GetCombatHitChanceMultiplier();
            }

            if (data.category == SkillCategory.NegativeCharm && attacker.currentLevel > defender.currentLevel)
                hitChance += (attacker.currentLevel - defender.currentLevel) * 2f;
        }

        hitChance = Mathf.Clamp(hitChance, 0f, 100f);
        result.hitChanceMultiplier = hitChance / 100f;
        result.chanceText = Mathf.RoundToInt(hitChance) + "%";

        if (Random.Range(0f, 100f) > hitChance) { result.isHit = false; return result; }
        result.isHit = true;

        if (data.category == SkillCategory.PositiveCharm) return result;

        float critChance = attacker.critChance;
        result.isCritical = Random.Range(0f, 100f) <= critChance;

        // --- MNO¯NIK OBRA¯EÑ ZOSTAWIONY W SPOKOJU ---
        float dmgModU = levelData != null ? levelData.damageMultiplier : 1.0f;
        float baseStatDmg = (data.strengthWeight * attacker.strength) + (data.agilityWeight * attacker.agility) + (data.knowledgeWeight * attacker.knowledge) + (data.powerWeight * attacker.power);
        float weaponDmg = attacker.weaponDamage * data.weaponDamageWeight;

        float rawDamage = (baseStatDmg + weaponDmg) * dmgModU * attacker.GetCombatDamageMultiplier();
        rawDamage *= Random.Range(0.7f, 1.3f);

        result.rawDamage = Mathf.RoundToInt(rawDamage);

        float reducedDamage = rawDamage;
        float activeArmor = 0;

        // Prosty podzia³: Fizyczne (Miecz/£uk) vs Magiczne (Magia/Uroki)
        if (data.category == SkillCategory.MeleePhysical || data.category == SkillCategory.RangedPhysical)
        {
            activeArmor = defender.GetCombatPhysicalArmor();
        }
        else
        {
            // Tutaj wpadnie RangedMagic (Mag Ognia) oraz NegativeCharm (Uroki)
            activeArmor = defender.GetCombatMagicResistance();
        }

        // --- TWÓJ NOWY SYSTEM PROCENTOWY ---
        // 1 punkt pancerza = 0.9% redukcji obra¿eñ
        float reductionPercentage = activeArmor * 0.9f;

        // Ograniczamy maksymaln¹ redukcjê do 90% (¿eby zawsze wejœæ za minimum 10%)
        // Zostawiamy dó³ otwarty (np. -999%), ¿eby Furia mog³a sprawiæ, ¿e bêdziesz bra³ WIÊCEJ obra¿eñ!
        reductionPercentage = Mathf.Clamp(reductionPercentage, -999f, 90f);

        // Przeliczamy to na mno¿nik (np. 90% redukcji = mno¿nik 0.1)
        float armorMultiplier = 1f - (reductionPercentage / 100f);

        // Aplikujemy pancerz do obra¿eñ
        reducedDamage *= armorMultiplier;

        // --- KRYTYK (Odpala siê po pancerzu, dok³adnie tak jak chcia³eœ!) ---
        if (result.isCritical)
        {
            if (Random.Range(0f, 100f) <= 20f) reducedDamage *= 3.0f;
            else reducedDamage *= 2.0f;
        }

        result.damageDealt = Mathf.RoundToInt(reducedDamage);
        if (result.damageDealt < 1 && data.category != SkillCategory.NegativeCharm) result.damageDealt = 1;

        return result;
    }
}

// TA KLASA MUSI BYÆ TUTAJ (pod DamageCalculator), aby b³¹d znikn¹³!
public class AttackResult
{
    public bool isHit;
    public int damageDealt;
    public bool isCritical;
    public string chanceText;
    public float hitChanceMultiplier;
    public int rawDamage;
}