using UnityEngine;
using System.Collections.Generic;

public enum SkillCategory
{
    PositiveCharm,  // 1. Uroki pozytywne
    NegativeCharm,  // 2. Uroki negatywne
    RangedMagic,    // 3. Magia dystansowa
    RangedPhysical, // 4. Fizyczny dystans
    MeleePhysical   // 5. Zwarcie
}

[System.Serializable]
public class SkillLevelData
{
    public string levelName = "I";
    public int requiredCharacterLevel = 1;
    public int staminaCost;
    public int manaCost;
    public float damageMultiplier = 1.0f;
    public float hitChanceBonus = 0f;
    [Header("Szanse na Efekty")]
    [Range(0, 100)] public float statusEffectChance = 10f;
    [Header("Trudnoœæ Buffa (Dla PositiveCharm)")]
    public int selfCastDifficulty = 120; // Np. 120 na 1 poziomie, 40 na 7 poziomie

    [Header("Ustawienia Efektów (np. Tarcza)")]
    public int effectCharges = 2; // Tu wpiszesz 2 dla poziomu 1, a 4 dla poziomu 7
    public int effectValue = 20;   // Opcjonalnie: si³a krwawienia/leczenia na tym poziomie
    public int effectDuration = 3;
    public float effectMultiplier = 1.5f;
    public float effectHitChanceMod = -20f; // mod celnosci
}

[CreateAssetMenu(fileName = "New Skill", menuName = "RPG System/Skill")]
public class SkillData : ScriptableObject
{
    [Header("Podstawowe Informacje")]
    public string skillName;
    public Sprite icon;
    public SkillCategory category = SkillCategory.MeleePhysical;

    [Header("Animacja")]
    public string animTriggerName;
    public bool showCenterVFX = false;

    [Header("Logika Efektów")]
    [Tooltip("Tu wrzucasz skrypty efektów (np. Tarcza, Krwawienie, Drain Many)")]
    public List<SkillEffect> effects;

    // Opcjonalnie: Efekt Bierny (Passive)
    public bool isPassive;

    [Header("Wagi Statystyk")]
    public float strengthWeight = 0f;
    public float agilityWeight = 0f;
    public float knowledgeWeight = 0f;
    public float powerWeight = 0f;
    [Range(0, 1)] public float weaponDamageWeight = 1.0f;

    public List<SkillLevelData> progression;

    public SkillLevelData GetLevelData(int level)
    {
        int index = Mathf.Clamp(level - 1, 0, progression.Count - 1);
        return progression.Count > 0 ? progression[index] : null;
    }
}