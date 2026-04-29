using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerSkillSaveData
{
    public SkillData skill;
    public int currentLevel = 1;

}

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    [Header("Do�wiadczenie i Poziom")]
    public int currentLevel = 1;
    public int currentExperience = 0;
    public int experienceToNextLevel = 100;
    public int deathCount = 0;

    [Header("Punkty do Rozdania")]
    public int availableSkillPoints = 2; 
    public int availableStatPoints = 5;

    public bool hasUnseenSkillPoints = true;
    public bool hasUnseenStatPoints = true;

    [Header("Podstawowe Statystyki (Zale�ne od Klasy i Poziomu)")]
    public int baseMaxHP = 230;
    public int baseMaxMana = 200;
    public int baseMaxStamina = 200;

    public int baseStrength = 10;
    public int baseAgility = 10;
    public int baseKnowledge = 10;
    public int basePower = 10;

    public int basePhysicalArmor = 0; 
    public int baseMagicResistance = 0;
    

    [Header("Bonusy z Ekwipunku / Buff�w")]
    public int bonusMaxHP = 0;
    public int bonusMaxMana = 0;
    public int bonusMaxStamina = 0;

    public int bonusStrength = 0;
    public int bonusAgility = 0;
    public int bonusKnowledge = 0;
    public int bonusPower = 0;

    public int bonusPhysicalArmor = 0;
    public int bonusMagicResistance = 0;

    public int weaponDamage = 0;

    [Header("Bonusy Unikalne z ekwipunku")]
    public int baseCritChance = 2;
    public int bonusCritChance = 0;
    public int baseDodgeChance = 0;
    public int bonusDodgeChance = 0;

    public float bonusDamageMultiplier = 0f;    // Np. 0.2f oznacza +20% obra�e� z jakiego� itemu
    public float bonusHitChanceMultiplier = 0f; // Np. 0.2f oznacza +20% celno�ci z jakiegos itemu

    // Zsumowane warto�ci:
    public int TotalDodgeChance => baseDodgeChance + bonusDodgeChance;

    [Header("Odblokowane Umiej�tno�ci")]
    public List<PlayerSkillSaveData> unlockedSkills = new List<PlayerSkillSaveData>();

    //ZSUMOWANE W�A�CIWO�CI (To z nich korzysta Kalkulator Walki)
    public int TotalMaxHP => baseMaxHP + bonusMaxHP;
    public int TotalMaxMana => baseMaxMana + bonusMaxMana;
    public int TotalMaxStamina => baseMaxStamina + bonusMaxStamina;
    public int TotalStrength => baseStrength + bonusStrength;
    public int TotalAgility => baseAgility + bonusAgility;
    public int TotalKnowledge => baseKnowledge + bonusKnowledge;
    public int TotalPower => basePower + bonusPower;

    public int TotalPhysicalArmor => basePhysicalArmor + bonusPhysicalArmor;
    public int TotalMagicResistance => baseMagicResistance + bonusMagicResistance;
    public int TotalCritChance => baseCritChance + bonusCritChance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadPlayerData();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // T� funkcj� wywo�uje TournamentManager po klikni�ciu "Wycofaj si�"
    public void AddExperience(int amount)
    {
        // Je�li mamy maksymalny poziom, ignorujemy expa
        if (currentLevel >= 35) return;

        currentExperience += amount;
        Debug.Log($"Zdobyto {amount} punkt�w do�wiadczenia!");

        // p�tla: Dzia�a dop�ki mamy do�� expa na kolejny poziom i nie dobili�my do 35 lvl
        while (currentExperience >= GetRequiredExpForNextLevel() && currentLevel < 35)
        {
            LevelUp();
        }

        // Blokada po wbiciu max levela
        if (currentLevel >= 35)
        {
            currentLevel = 35;
            currentExperience = 0;
        }
    }

    private void LevelUp()
    {
        // Najpierw "p�acimy" expem za ten poziom
        currentExperience -= GetRequiredExpForNextLevel();

        // Wbijamy poziom
        currentLevel++;

        // NAGRODY ZA LEVEL
        availableSkillPoints += 2;
        availableStatPoints += 5;

        hasUnseenSkillPoints = true;
        hasUnseenStatPoints = true;

        Debug.Log($"<color=cyan>AWANS! Osi�gni�to {currentLevel} poziom postaci! Przyznano 2 pkt umiej�tno�ci i 5 pkt statystyk.</color>");
    }

    //TABELA DO�WIADCZENIA
    // Indeks 0 to koszt przej�cia z 1 na 2 poziom, Indeks 1 to z 2 na 3, itd.
    private readonly int[] expTable = new int[]
    {
        10,   // Lvl 1 -> 2
        20,   // Lvl 2 -> 3
        30,   // Lvl 3 -> 4
        60,   // Lvl 4 -> 5
        90,   // 5 -> 6
        120,   // 6 -> 7
        240,  // 7 -> 8
        360,  // 8 -> 9
        390,  // 9 -> 10
        780,  // 10 -> 11
        1170,  // 11 -> 12
        1200,  // 12 -> 13
        2400,  // 13 -> 14
        3600,  // 14 -> 15
        3600,  // 15 -> 16
        3600,  // 16 -> 17
        3600,  // 17 -> 18
        4000, // 18 -> 19
        8000, // 19 -> 20
        12000, // 20 -> 21
        12000, // 21 -> 22
        12000, // 22 -> 23
        12000, // 23 -> 24
        12000, // 24 -> 25
        15000, // 25 -> 26
        30000, // 26 -> 27
        45000, // 27 -> 28
        45000, // 28 -> 29
        45000, // 29 -> 30
        45000, // 30 -> 31
        45000, // 31 -> 32
        45000, // 32 -> 33
        45000, // 33 -> 34
        45000  // 34 -> 35
    };

    public int GetRequiredExpForNextLevel()
    {
        if (currentLevel >= 35) return 999999; // Zabezpieczenie dla Max Levela

        // currentLevel = 1 pobierze indeks 0 (czyli 100)
        return expTable[currentLevel - 1];
    }

    public void AddStatPoints(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        availableStatPoints += amount;
        hasUnseenStatPoints = true;
        Debug.Log($"[WYMIANA] Dodano {amount} punkt(ów) statystyk. Aktualnie: {availableStatPoints}");
    }


    // --- SYSTEM ZAPISU I WCZYTYWANIA STATYSTYK ---

    public void SavePlayerData()
    {
        PlayerPrefs.SetInt("PlayerLevel", currentLevel);
        PlayerPrefs.SetInt("PlayerExp", currentExperience);
        PlayerPrefs.SetInt("DeathCount", deathCount);
        PlayerPrefs.SetInt("SkillPoints", availableSkillPoints);
        PlayerPrefs.SetInt("StatPoints", availableStatPoints);

        PlayerPrefs.SetInt("BaseHP", baseMaxHP);
        PlayerPrefs.SetInt("BaseMana", baseMaxMana);
        PlayerPrefs.SetInt("BaseStamina", baseMaxStamina);
        PlayerPrefs.SetInt("BaseStr", baseStrength);
        PlayerPrefs.SetInt("BaseAgi", baseAgility);
        PlayerPrefs.SetInt("BaseKno", baseKnowledge);
        PlayerPrefs.SetInt("BasePow", basePower);

        // Save equipment bonuses
        PlayerPrefs.SetInt("BonusHP", bonusMaxHP);
        PlayerPrefs.SetInt("BonusMana", bonusMaxMana);
        PlayerPrefs.SetInt("BonusStamina", bonusMaxStamina);
        PlayerPrefs.SetInt("BonusStr", bonusStrength);
        PlayerPrefs.SetInt("BonusAgi", bonusAgility);
        PlayerPrefs.SetInt("BonusKno", bonusKnowledge);
        PlayerPrefs.SetInt("BonusPow", bonusPower);
        PlayerPrefs.SetInt("BonusPhysArm", bonusPhysicalArmor);
        PlayerPrefs.SetInt("BonusMagRes", bonusMagicResistance);
        PlayerPrefs.SetInt("WeaponDmg", weaponDamage);
        PlayerPrefs.SetInt("BonusCrit", bonusCritChance);
        PlayerPrefs.SetInt("BonusDodge", bonusDodgeChance);
        PlayerPrefs.SetFloat("BonusDmgMult", bonusDamageMultiplier);
        PlayerPrefs.SetFloat("BonusHitMult", bonusHitChanceMultiplier);

        // --- NOWO��: Zapisywanie listy umiej�tno�ci! ---
        foreach (var savedSkill in unlockedSkills)
        {
            if (savedSkill.skill != null)
            {
                // Tworzymy unikalny klucz dla ka�dego skilla, np. "SkillLvl_SzybkiCios"
                PlayerPrefs.SetInt("SkillLvl_" + savedSkill.skill.name, savedSkill.currentLevel);
            }
        }

        PlayerPrefs.Save();
        Debug.Log("<color=green>[ZAPIS] Statystyki, poziom, UMIEJ�TNO�CI i BONUSY z ekwipunku zapisane! Bonus HP: " + bonusMaxHP + ", Bonus Str: " + bonusStrength + "</color>");
    }

    public void LoadPlayerData()
    {
        currentLevel = PlayerPrefs.GetInt("PlayerLevel", currentLevel);
        currentExperience = PlayerPrefs.GetInt("PlayerExp", currentExperience);
        deathCount = PlayerPrefs.GetInt("DeathCount", deathCount);
        availableSkillPoints = PlayerPrefs.GetInt("SkillPoints", availableSkillPoints);
        availableStatPoints = PlayerPrefs.GetInt("StatPoints", availableStatPoints);

        baseMaxHP = PlayerPrefs.GetInt("BaseHP", baseMaxHP);
        baseMaxMana = PlayerPrefs.GetInt("BaseMana", baseMaxMana);
        baseMaxStamina = PlayerPrefs.GetInt("BaseStamina", baseMaxStamina);
        baseStrength = PlayerPrefs.GetInt("BaseStr", baseStrength);
        baseAgility = PlayerPrefs.GetInt("BaseAgi", baseAgility);
        baseKnowledge = PlayerPrefs.GetInt("BaseKno", baseKnowledge);
        basePower = PlayerPrefs.GetInt("BasePow", basePower);

        // Load equipment bonuses
        bonusMaxHP = PlayerPrefs.GetInt("BonusHP", bonusMaxHP);
        bonusMaxMana = PlayerPrefs.GetInt("BonusMana", bonusMaxMana);
        bonusMaxStamina = PlayerPrefs.GetInt("BonusStamina", bonusMaxStamina);
        bonusStrength = PlayerPrefs.GetInt("BonusStr", bonusStrength);
        bonusAgility = PlayerPrefs.GetInt("BonusAgi", bonusAgility);
        bonusKnowledge = PlayerPrefs.GetInt("BonusKno", bonusKnowledge);
        bonusPower = PlayerPrefs.GetInt("BonusPow", bonusPower);
        bonusPhysicalArmor = PlayerPrefs.GetInt("BonusPhysArm", bonusPhysicalArmor);
        bonusMagicResistance = PlayerPrefs.GetInt("BonusMagRes", bonusMagicResistance);
        weaponDamage = PlayerPrefs.GetInt("WeaponDmg", weaponDamage);
        bonusCritChance = PlayerPrefs.GetInt("BonusCrit", bonusCritChance);
        bonusDodgeChance = PlayerPrefs.GetInt("BonusDodge", bonusDodgeChance);
        bonusDamageMultiplier = PlayerPrefs.GetFloat("BonusDmgMult", bonusDamageMultiplier);
        bonusHitChanceMultiplier = PlayerPrefs.GetFloat("BonusHitMult", bonusHitChanceMultiplier);

        // --- NOWO��: Wczytywanie listy umiej�tno�ci! ---
        foreach (var savedSkill in unlockedSkills)
        {
            if (savedSkill.skill != null)
            {
                // Je�li znajdzie zapis, podmieni level. Je�li nie, zostawi domy�lny z Inspektora
                savedSkill.currentLevel = PlayerPrefs.GetInt("SkillLvl_" + savedSkill.skill.name, savedSkill.currentLevel);
            }
        }

        Debug.Log("<color=yellow>[WCZYTYWANIE] Statystyki, poziom, UMIEJ�TNO�CI i BONUSY z ekwipunku za�adowane! Bonus HP: " + bonusMaxHP + ", Bonus Str: " + bonusStrength + "</color>");
    }

    private void OnApplicationQuit()
    {
        SavePlayerData();
    }

    public void RestoreBonusesIfZero()
    {
        // Bezpiecznik: jeśli bonusy są zerowe ale w save mamy wartości, przywróć je
        // (może się zdarzyć jeśli EquipmentStatsCalculator się nie inicjalizuje)
        if (bonusMaxHP == 0 && bonusStrength == 0 && bonusAgility == 0)
        {
            int savedHP = PlayerPrefs.GetInt("BonusHP", 0);
            if (savedHP > 0)
            {
                bonusMaxHP = savedHP;
                bonusMaxMana = PlayerPrefs.GetInt("BonusMana", 0);
                bonusMaxStamina = PlayerPrefs.GetInt("BonusStamina", 0);
                bonusStrength = PlayerPrefs.GetInt("BonusStr", 0);
                bonusAgility = PlayerPrefs.GetInt("BonusAgi", 0);
                bonusKnowledge = PlayerPrefs.GetInt("BonusKno", 0);
                bonusPower = PlayerPrefs.GetInt("BonusPow", 0);
                bonusPhysicalArmor = PlayerPrefs.GetInt("BonusPhysArm", 0);
                bonusMagicResistance = PlayerPrefs.GetInt("BonusMagRes", 0);
                weaponDamage = PlayerPrefs.GetInt("WeaponDmg", 0);
                bonusCritChance = PlayerPrefs.GetInt("BonusCrit", 0);
                bonusDodgeChance = PlayerPrefs.GetInt("BonusDodge", 0);
                bonusDamageMultiplier = PlayerPrefs.GetFloat("BonusDmgMult", 0f);
                bonusHitChanceMultiplier = PlayerPrefs.GetFloat("BonusHitMult", 0f);
                Debug.Log("<color=orange>[BEZPIECZNIK] Przywrócono bonusy ze save! HP: " + bonusMaxHP + ", Str: " + bonusStrength + "</color>");
            }
        }
    }


}