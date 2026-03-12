using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Class", menuName = "RPG System/Character Class")]
public class ClassData : ScriptableObject
{
    public string className;
    public Sprite classIcon;

    // Lista 6 umiejêtnoœci przypisanych do tej klasy
    public List<SkillData> classSkills = new List<SkillData>();

    [Header("Statystyki Bazowe Klasy")]
    public int startingSila;
    public int startingMoc;
    public int startingKondycja;
}