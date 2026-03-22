using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Enemy", menuName = "RPG System/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Podstawowe Informacje")]
    public string enemyName = "Nowy Wróg";
    public Sprite avatarImage;
    public GameObject enemyVisualPrefab;
    public int level = 1;

    [Header("Atrybuty")]
    public int maxHP = 100;
    public int maxMana = 50;
    public int maxStamina = 50;

    public int strength = 10;
    public int agility = 10;
    public int knowledge = 10;
    public int power = 10;

    [Header("Odpornoœci i Walka")]
    public int physicalArmor = 5;
    public int magicResistance = 0;
    public int critChance = 5;
    public int weaponDamage = 10;

    [Header("Odleg³oœæ do Zwarcia")]
    public float meleeStoppingDistance = 1.5f;

    [Header("Ksiêga Umiejêtnoœci")]
    [Tooltip("Lista skilli, których mo¿e u¿ywaæ przeciwnik")]
    public List<CharacterSkill> enemySkills;


    [Header("Sztuczna Inteligencja")]
    public EnemyAIBrain aiBrain;

    [Header("Nagrody za pokonanie")]
    public int goldReward = 0;
    public int expReward = 0;

    [Header("Nagrody Specjalne (Gemy)")]
    public int gemRewardAmount = 0; // Ile gemów zrzuci (np. 10, 20)
    [Range(0, 100)]
    public float gemRewardChance = 0f; // Szansa na drop (np. 10%, 100%)
}