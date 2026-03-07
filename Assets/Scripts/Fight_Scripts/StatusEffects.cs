using UnityEngine;

[System.Serializable]
public class StatusEffect
{
    public string effectName;
    public int duration;      // Ile rund trwa
    public int value;         // Ile obra¿eñ/leczenia na rundê
    public bool isDamage;     // True = bije (ogieñ), False = leczy (regencacja)
    public Sprite icon;       // Ikonka do wyœwietlenia nad HP
}