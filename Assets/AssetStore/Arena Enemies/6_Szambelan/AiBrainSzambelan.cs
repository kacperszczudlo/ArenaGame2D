using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Szambelan Brain", menuName = "ArenaRPG/AI/Szambelan Brain")]
public class AIBrain_Szambelan : EnemyAIBrain
{
    public override List<CombatAction> DecideTurn(Combatant me, Combatant player, ref int actionCounter)
    {
        List<CombatAction> actions = new List<CombatAction>();
        me.ResetDefensePA();

        if (me.mySkills.Count < 4)
        {
            return actions;
        }

        CharacterSkill poisonArrow = me.mySkills[0];
        CharacterSkill eyeArrow = me.mySkills[1];
        CharacterSkill iceArrow = me.mySkills[2];
        CharacterSkill fireArrow = me.mySkills[3];

        int round = BattleManager.Instance.currentRound;

        // FAZA 1: TOTALNY DEBUFF (Runda 1, 8, 15, 22...)
        if (round == 1 || (round - 1) % 7 == 0)
        {

            actions.Add(new CombatAction { actor = me, target = player, skill = poisonArrow, paInvested = 4, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = poisonArrow, paInvested = 5, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = poisonArrow, paInvested = 2, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = eyeArrow, paInvested = 2, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = eyeArrow, paInvested = 3, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = eyeArrow, paInvested = 3, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = iceArrow, paInvested = 2, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = iceArrow, paInvested = 2, originalIndex = actionCounter++ });

            me.defenseMeleePA = 1;
            me.defenseRangedPA = 1;
            me.defenseMentalPA = 1;
        }
        // FAZA 2: G£ÓWNY OSTRZA£ OGNIA (Rundy nastêpuj¹ce po debuffie: 2, 9, 16...)
        else if (round == 2 || (round - 2) % 7 == 0)
        {

            actions.Add(new CombatAction { actor = me, target = player, skill = fireArrow, paInvested = 2, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = fireArrow, paInvested = 4, originalIndex = actionCounter++ });
            

            me.defenseMeleePA = 5;
            me.defenseRangedPA = 5;
            me.defenseMentalPA = 5;
        }
        // FAZA 3: REGULARNA WALKA 
        else
        {
            Debug.Log($"<color=orange>{me.combatantName} (Runda {round}): Ogieñ i kontrola t³umu!</color>");

            // 1 potê¿na strza³a ognia do zadawania HP
            actions.Add(new CombatAction { actor = me, target = player, skill = fireArrow, paInvested = 3, originalIndex = actionCounter++ });

            //mieszanka debuffów do kradzie¿y PA i zatrucia
            actions.Add(new CombatAction { actor = me, target = player, skill = eyeArrow, paInvested = 3, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = iceArrow, paInvested = 3, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = poisonArrow, paInvested = 2, originalIndex = actionCounter++ });

            me.defenseMeleePA = 2;
            me.defenseRangedPA = 4;
            me.defenseMentalPA = 1;
        }

        return actions;
    }
}