using UnityEngine;

public enum StatusType { DamageOverTime, HealOverTime, Shield }

[System.Serializable]
public class StatusEffect
{
    public string effectName;
    public StatusType type;
    public int duration;
    public int value;
    public int remainingHits;
    public bool isDamage;
    public Sprite icon;

    public StatusEffect() { }

    
}