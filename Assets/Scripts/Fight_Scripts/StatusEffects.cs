using UnityEngine;

public enum StatusType { DamageOverTime, HealOverTime, Shield, Blessing, Fury, Poison, Freeze, Blindness, FireShield, VoodooCurse, DeepFreeze }
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
    public float multiplier;
    public float hitChanceMod;

    public StatusEffect() { }

    
}