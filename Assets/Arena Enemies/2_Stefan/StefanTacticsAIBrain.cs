using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StefanBrain", menuName = "ArenaRPG/AI/Stefan Brain")]
public class AIBrain_RoundArcher : EnemyAIBrain
{
    public override List<CombatAction> DecideTurn(Combatant me, Combatant player, ref int actionCounter)
    {
        List<CombatAction> actions = new List<CombatAction>();
        me.ResetDefensePA();

        if (me.mySkills.Count < 2) return actions;

        CharacterSkill fastArrow = me.mySkills[0];
        CharacterSkill normalArrow = me.mySkills[1];

        int round = BattleManager.Instance.currentRound;

        

        if (round == 1)
        {
            // --- RUNDA 1: Zmiêkczenie na start ---
            // £ucznik ZAWSZE zaczyna walkê od szybkiej strza³y i mocnej obrony.
            actions.Add(new CombatAction { actor = me, target = player, skill = fastArrow, paInvested = 1, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = fastArrow, paInvested = 1, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = fastArrow, paInvested = 1, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = fastArrow, paInvested = 1, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = fastArrow, paInvested = 1, originalIndex = actionCounter++ });

            me.defenseMeleePA = 5;
            me.defenseRangedPA = 5;
            me.defenseMentalPA = 5;
        }
        else if (round % 3 == 0)
        {
            // --- RUNDA 3, 6, 9, 12... ---
            // £ucznik opuszcza gardê i ³aduje potê¿ny, morderczy strza³ za 5 PA
            
            actions.Add(new CombatAction { actor = me, target = player, skill = normalArrow, paInvested = 5, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = normalArrow, paInvested = 5, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = normalArrow, paInvested = 5, originalIndex = actionCounter++ });



            me.defenseMeleePA = 1;
            me.defenseRangedPA = 0;
            me.defenseMentalPA = 0;
        }
        else
        {
            // --- POZOSTA£E RUNDY ---
            // Standardowy, powtarzalny ostrza³. 
            actions.Add(new CombatAction { actor = me, target = player, skill = normalArrow, paInvested = 2, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = fastArrow, paInvested = 3, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = fastArrow, paInvested = 3, originalIndex = actionCounter++ });

            me.defenseMeleePA = 3;
            me.defenseRangedPA = 3;
            me.defenseMentalPA = 3;
        }

        return actions;
    }
}