using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Basic Fighter Brain", menuName = "ArenaRPG/AI/Basic Fighter Brain")]
public class AIBrain_BasicFighter : EnemyAIBrain
{
    public override List<CombatAction> DecideTurn(Combatant me, Combatant player, ref int actionCounter)
    {
        List<CombatAction> actions = new List<CombatAction>();

        me.ResetDefensePA();
        int tacticRoll = Random.Range(1, 4);
        Debug.Log($"<color=orange>{me.combatantName} (Mózg: Podstawowy Wojownik) wybiera Taktykê nr: {tacticRoll}</color>");

        if (tacticRoll == 1) 
        {
            if (me.mySkills.Count > 0)
            {
                actions.Add(new CombatAction { actor = me, target = player, skill = me.mySkills[1], paInvested = 2, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = me.mySkills[1], paInvested = 2, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = me.mySkills[1], paInvested = 3, originalIndex = actionCounter++ });


                me.defenseMeleePA = 3;
                me.defenseRangedPA = 3;
                me.defenseMentalPA = 3;
            }
        }
        else if (tacticRoll == 2)
        {
            for (int i = 0; i < 8; i++)
            {
                actions.Add(new CombatAction
                {
                    actor = me,
                    target = player,
                    skill = me.mySkills[0], // Zatruty Cios
                    paInvested = 1, // Koszt w Punktach Akcji (PA)
                    originalIndex = actionCounter++
                });
            }
            // --- NAPRAWA: Zmienne obrony s¹ teraz bezpieczne w œrodku bloku! ---
            me.defenseMeleePA = 0;
            me.defenseRangedPA = 0;
            me.defenseMentalPA = 0;
        }
        else
        {
            if (me.mySkills.Count >= 2)
                actions.Add(new CombatAction { actor = me, target = player, skill = me.mySkills[2], paInvested = 4, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = me.mySkills[2], paInvested = 4, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = me.mySkills[2], paInvested = 6, originalIndex = actionCounter++ });


            me.defenseMeleePA = 5;
            me.defenseRangedPA = 5;
            me.defenseMentalPA = 5;
        }

        Debug.Log($"<color=red>{me.combatantName} ustawi³ obronê: Zwarcie {me.defenseMeleePA}, Dystans {me.defenseRangedPA}, Umys³ {me.defenseMentalPA}</color>");
        return actions;
    }
}