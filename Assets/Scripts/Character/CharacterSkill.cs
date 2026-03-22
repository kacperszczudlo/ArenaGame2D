using System;
using UnityEngine;

// [Serializable] sprawia, ¿e ta klasa poka¿e siê w Inspektorze w Unity
[Serializable]
public class CharacterSkill
{
    public SkillData data;       
    public int currentLevel = 1;     // Poziom skilla u TEJ KONKRETNEJ postaci
    public bool isUnlocked = true;   // Czy postaæ ma to odblokowane?
}