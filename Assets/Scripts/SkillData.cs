using UnityEngine;
using System.Collections.Generic;

// Definicja typów ataku (naprawia b³¹d GetName/Namespace)
public enum AttackType { Physical, MagicPhysical, RangedPhysical, RangedMagic, MentalNegative, MentalPositive }

[System.Serializable]
public struct SkillLevelData
{
    public string levelName;      // I, II, III...
    public float damagePercent;   // np. 170, 182...
    public float hitChanceMod;    // np. 100, 104...
    public int staminaCost;
    public int manaCost;
}

[CreateAssetMenu(fileName = "New Skill", menuName = "RPG System/Skill")]
public class SkillData : ScriptableObject
{
    [Header("Podstawowe Informacje")]
    public string skillName;
    public Sprite icon;
    public AttackType type;
    public bool requiresWeapon = true; // Jeœli false, mo¿na biæ bez broni

    [Header("Wagi Statystyk (Wzór OBR)")]
    public float powerWeight = 0f;     // Moc
    public float knowledgeWeight = 0f; // Wiedza
    public float strengthWeight = 0f;  // Si³a
    public float agilityWeight = 0f;   // Zrêcznoœæ

    [Header("Mno¿nik Broni")]
    [Range(0, 1)] public float weaponDamageWeight = 1.0f; // 1.0 = 100% obra¿eñ broni

    [Header("Tabela Poziomów (I - VII)")]
    public List<SkillLevelData> progression;

    [Header("Logika Specjalna")]
    public string specialLogicID; // np. "druid_wrath"
}