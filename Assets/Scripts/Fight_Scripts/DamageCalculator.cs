using UnityEngine;

public static class DamageCalculator
{
    public static AttackResult ProcessAttack(Combatant attacker, Combatant defender, CharacterSkill skill, int allocatedPA, int defenderPA = 0)
    {
        AttackResult result = new AttackResult();
        SkillData data = skill.data;

        // Pobieramy dane dla konkretnego poziomu skilla (modU i bonusy)
        SkillLevelData levelData = data.GetLevelData(skill.currentLevel);
        float modU = levelData != null ? levelData.damageMultiplier : 1.0f;
        float hitChanceModU = levelData != null ? levelData.hitChanceBonus : 0f;

        // 1. WYBÓR STATYSTYK (Zgodnie z Twoj¹ list¹ trafienie/obrona)
        float attackerStat = 0;
        float defenderStat = 0;

        switch (data.category)
        {
            case SkillCategory.MeleePhysical:
            case SkillCategory.RangedPhysical:
                attackerStat = attacker.agility;
                defenderStat = defender.agility;
                break;
            case SkillCategory.RangedMagic:
                attackerStat = attacker.knowledge;
                // Obrona przed magi¹ dystansow¹: Zrêcznoœæ + Wiedza (œrednia)
                defenderStat = (defender.agility + defender.knowledge) / 2f;
                break;
            case SkillCategory.NegativeCharm: // Ataki psychiczne
                attackerStat = attacker.knowledge;
                // Jeœli obroñca nie da³ PA w obronê psychiczn¹, jego statystyki siê nie licz¹!
                defenderStat = (defenderPA > 0) ? defender.knowledge : 0;
                break;
            case SkillCategory.PositiveCharm:
                attackerStat = attacker.knowledge;
                defenderStat = 0; // Uroki pozytywne zawsze trafiaj¹
                break;
        }

        // 2. WZÓR NA POWODZENIE ATAKU
        float hitChance = 0;

        if (data.category == SkillCategory.PositiveCharm)
        {
            // --- NOWA LOGIKA DLA BUFFÓW (Tarcza itp.) ---
            // Sprawdzamy PA gracza przeciwko trudnoœci wpisanej w poziom skilla
            float playerPower = allocatedPA * 20f;

            // Jeœli nie przypisa³eœ levelData, dajemy domyœln¹ trudnoœæ 100, ¿eby nie wywali³o b³êdu
            float difficulty = (levelData != null) ? levelData.selfCastDifficulty : 100f;

            hitChance = (playerPower / difficulty) * 100f;
        }
        else
        {
            // --- LOGIKA DLA ATAKÓW ---
            float PA_AttackMod = 1.0f + (allocatedPA * 0.2f);
            float PA_DefenseMod = 1.0f + (defenderPA * 0.2f);

            float attackPower = (40f + attackerStat + attacker.currentLevel) * PA_AttackMod * modU;
            float defensePower = (40f + defenderStat + defender.currentLevel) * PA_DefenseMod;

            hitChance = (attackPower / (attackPower + defensePower)) * 100f + hitChanceModU;

            if (data.category == SkillCategory.NegativeCharm && attacker.currentLevel > defender.currentLevel)
            {
                hitChance += (attacker.currentLevel - defender.currentLevel) * 2f;
            }
        }

        // Zabezpieczenie: szansa nie mo¿e byæ ujemna
        hitChance = Mathf.Max(0f, hitChance);

        // Zapisujemy mno¿nik (potrzebny do BattleManagera, ¿eby wiedzieæ czy status wejdzie)
        result.hitChanceMultiplier = Mathf.Clamp01(hitChance / 100f);
        result.chanceText = Mathf.RoundToInt(hitChance) + "%";

        // 3. SPRAWDZENIE TRAFIENIA
        if (Random.Range(0f, 100f) > hitChance)
        {
            result.isHit = false;
            return result;
        }
        result.isHit = true;

       // --- LOGIKA KRYTYKÓW ---
        // Pobieramy szansê z atakuj¹cego 
        float critChance = attacker.critChance; 
        result.isCritical = Random.Range(0f, 100f) <= critChance;

        // 4. OBLICZANIE OBRA¯EÑ BAZOWYCH
        float baseStatDmg = (data.strengthWeight * attacker.strength) +
                            (data.agilityWeight * attacker.agility) +
                            (data.knowledgeWeight * attacker.knowledge) +
                            (data.powerWeight * attacker.power);

        float weaponDmg = attacker.weaponDamage * data.weaponDamageWeight;
        float rawDamage = (baseStatDmg + weaponDmg) * modU;

        // --- NOWOŒÆ: ROZRZUT (VARIANCE) +/- 30% ---
        float randomMultiplier = Random.Range(0.7f, 1.3f);
        rawDamage *= randomMultiplier;

        // Redukcja przez pancerz
        float reducedDamage = rawDamage;
        if (data.category == SkillCategory.MeleePhysical || data.category == SkillCategory.RangedPhysical)
            reducedDamage -= defender.physicalArmor;
        else
            reducedDamage -= defender.magicResistance;

        // 5. KRYTYKI (na samym koñcu, po rozrzucie)
        if (result.isCritical)
        {
            // Losujemy wartoœæ od 0 do 100, aby okreœliæ si³ê krytyka
            float critSeverityRoll = Random.Range(0f, 100f);

            // 20% szans na potê¿ny krytyk (x3)
            if (critSeverityRoll <= 20f)
            {
                reducedDamage *= 3.0f;
            }
            // Pozosta³e 80% szans na standardowy krytyk (x2)
            else
            {
                reducedDamage *= 2.0f;
            }
        }

        // Jeœli to czysty Debuff (NegativeCharm) i nie ustawiliœmy mu wag obra¿eñ, 
        // to damageDealt mo¿e byæ 0. 
        result.damageDealt = Mathf.RoundToInt(reducedDamage);

        // Zabezpieczenie: jeœli to atak (Damage/Physical), dajemy min. 1. 
        // Jeœli to tylko kl¹twa, pozwalamy na 0.
        if (result.damageDealt < 1 && data.category != SkillCategory.NegativeCharm)
            result.damageDealt = 1;

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
}