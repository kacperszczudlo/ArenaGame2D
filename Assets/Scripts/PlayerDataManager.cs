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

    [Header("Doœwiadczenie i Poziom")]
    public int currentLevel = 1;
    public int currentExperience = 0;
    public int experienceToNextLevel = 100;
    public int deathCount = 0;

    [Header("Punkty do Rozdania")]
    public int availableSkillPoints = 2; 
    public int availableStatPoints = 5;

    [Header("Podstawowe Statystyki (Zale¿ne od Klasy i Poziomu)")]
    public int baseMaxHP = 230;
    public int baseMaxMana = 200;
    public int baseMaxStamina = 200;

    public int baseStrength = 10;
    public int baseAgility = 10;
    public int baseKnowledge = 10;
    public int basePower = 10;

    public int basePhysicalArmor = 0;   // Np. Rycerz mo¿e mieæ 2 na start, Mag 0
    public int baseMagicResistance = 0;
    

    [Header("Bonusy z Ekwipunku / Buffów")]
    public int bonusMaxHP = 0;          // np. Pierœcieñ ¯ycia +20 HP
    public int bonusMaxMana = 0;
    public int bonusMaxStamina = 0;

    public int bonusStrength = 0;
    public int bonusAgility = 0;
    public int bonusKnowledge = 0;
    public int bonusPower = 0;

    public int bonusPhysicalArmor = 0;  // Zbroja
    public int bonusMagicResistance = 0;// Amulet
        // Ostry miecz daje +10% do Crita

    public int weaponDamage = 0;       // Si³a samej broni

    [Header("Bonusy Unikalne z ekwipunku")]
    public int baseCritChance = 2;      // Baza
    public int bonusCritChance = 0;    // szansa na krytyk 
    public int baseDodgeChance = 0;     // Bazowa szansa na unik (np. klasa £otrzyka mo¿e mieæ 5%)
    public int bonusDodgeChance = 0;    // Unik z butów/p³aszcza

    public float bonusDamageMultiplier = 0f;    // Np. 0.2f oznacza +20% obra¿eñ z jakiegoœ artefaktu
    public float bonusHitChanceMultiplier = 0f; // Np. 0.2f oznacza +20% celnoœci z magicznego wizjera

    // Zsumowane wartoœci:
    public int TotalDodgeChance => baseDodgeChance + bonusDodgeChance;

    [Header("Odblokowane Umiejêtnoœci")]
    public List<PlayerSkillSaveData> unlockedSkills = new List<PlayerSkillSaveData>();

    // --- ZSUMOWANE W£AŒCIWOŒCI (To z nich korzysta Kalkulator Walki!) ---
    public int TotalMaxHP => baseMaxHP + bonusMaxHP;
    public int TotalMaxMana => baseMaxMana + bonusMaxMana;
    public int TotalMaxStamina => baseMaxStamina + bonusMaxStamina;
    public int TotalStrength => baseStrength + bonusStrength;
    public int TotalAgility => baseAgility + bonusAgility;
    public int TotalKnowledge => baseKnowledge + bonusKnowledge;
    public int TotalPower => basePower + bonusPower;

    public int TotalPhysicalArmor => basePhysicalArmor + bonusPhysicalArmor;
    public int TotalMagicResistance => baseMagicResistance + bonusMagicResistance;
    public int TotalCritChance => baseCritChance + bonusCritChance; // O to dok³adnie Ci chodzi³o!

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Tê funkcjê wywo³uje TournamentManager po klikniêciu "Wycofaj siê"
    public void AddExperience(int amount)
    {
        // Jeœli mamy maksymalny poziom, ignorujemy expa
        if (currentLevel >= 35) return;

        currentExperience += amount;
        Debug.Log($"Zdobyto {amount} punktów doœwiadczenia!");

        // Magiczna pêtla: Dzia³a dopóki mamy doœæ expa na kolejny poziom i nie dobiliœmy do 35 lvl
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
        // Najpierw "p³acimy" expem za ten poziom
        currentExperience -= GetRequiredExpForNextLevel();

        // Wbijamy poziom
        currentLevel++;

        // --- NAGRODY ZA LEVEL ---
        availableSkillPoints += 2;
        availableStatPoints += 5;

        Debug.Log($"<color=cyan>AWANS! Osi¹gniêto {currentLevel} poziom postaci! Przyznano 2 pkt umiejêtnoœci i 5 pkt statystyk.</color>");
    }

    // W tym miejscu w przysz³oœci wpiszesz dok³adne wymagania!
    public int GetRequiredExpForNextLevel()
    {
        // Na ten moment to prosty wzór:
        // Lvl 1 -> wymaga 100 expa
        // Lvl 2 -> wymaga 150 expa
        // Lvl 3 -> wymaga 200 expa itd.
        // PóŸniej zamienimy to na dok³adn¹ listê wartoœci, jakie tylko sobie wymyœlisz!

        return 100 + (currentLevel - 1) * 50;
    }

    // Ta funkcja w przysz³oœci obs³u¿y wybór klasy w Menu G³ównym!
    /*
    public void InitializeClass(ClassData chosenClass)
    {
        baseStrength = chosenClass.startStrength;
        // ... itd ...
        unlockedSkills.Clear();
        foreach(var startSkill in chosenClass.startingSkills)
        {
            unlockedSkills.Add(new PlayerSkillSaveData { skill = startSkill, currentLevel = 1 });
        }
    }
    */
}