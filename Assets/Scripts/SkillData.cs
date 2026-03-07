using UnityEngine;
using System.Collections.Generic;

// Twoja nowa, precyzyjna kategoria (ustala kolejnoœæ w rundzie!)
public enum SkillCategory
{
    PositiveCharm,  // 1. Uroki pozytywne (Leczenie, Buff)
    NegativeCharm,  // 2. Uroki negatywne & Ataki Psychiczne
    RangedMagic,    // 3. Magiczny atak dystansowy
    RangedPhysical, // 4. Fizyczny atak dystansowy
    MeleePhysical   // 5. Atak fizyczny (Wymaga podejœcia)
}

[System.Serializable]
public class SkillLevelData
{
    public string levelName = "I";

    [Tooltip("Poziom postaci wymagany, by móc odblokowaæ ten poziom skilla")]
    public int requiredCharacterLevel = 1;

    [Header("Koszty i Trudnoœæ")]
    public int staminaCost;
    public int manaCost;
    [Tooltip("Trudnoœæ skilla. Jeœli trudnoœæ wynosi np. 120, to wymaga 6 PA (1 PA = 20 trudnoœci)")]
    public int difficulty;

    [Header("Efekty G³ówne (Walka / Leczenie)")]
    [Tooltip("Modyfikator obra¿eñ (modU we wzorze), np. 1.2 dla 120%")]
    public float damageMultiplier = 1.0f;
    [Tooltip("Dodatkowa szansa na trafienie z poziomu umiejêtnoœci")]
    public float hitChanceBonus = 0f;

    [Header("Efekty Dodatkowe")]
    public int apDrain = 0;
    public int effectDurationRounds = 0;

    [Header("Efekty Statusowe")]
    public int statusEffectChance = 0;
    public string statusEffectID;
}

[CreateAssetMenu(fileName = "New Skill", menuName = "RPG System/Skill")]
public class SkillData : ScriptableObject
{
    [Header("Podstawowe Informacje")]
    public string skillName;
    public Sprite icon;
    public SkillCategory category = SkillCategory.MeleePhysical;
    public bool requiresWeapon = true;

    [Header("Animacja")]
    [Tooltip("Nazwa triggera: Attack_Melee, Attack_Ranged, Heal, Magic_Cast")]
    public string animTriggerName;
    public bool showCenterVFX = false;

    [Header("Wagi Statystyk (Wzór na obra¿enia)")]
    public float strengthWeight = 0f;
    public float agilityWeight = 0f;
    public float knowledgeWeight = 0f;
    public float powerWeight = 0f;

    [Header("Mno¿nik Broni")]
    [Range(0, 1)] public float weaponDamageWeight = 1.0f;

    [Header("Tabela Poziomów (I - VII)")]
    public List<SkillLevelData> progression;

    [Header("Logika Specjalna")]
    public string specialLogicID;

    // Pomocnicza funkcja, ¿eby ³atwo wyci¹gn¹æ dane konkretnego poziomu
    public SkillLevelData GetLevelData(int level)
    {
        int index = Mathf.Clamp(level - 1, 0, progression.Count - 1);
        return progression.Count > 0 ? progression[index] : null;
    }
}