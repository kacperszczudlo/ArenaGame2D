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
            Debug.LogError($"<color=red>UWAGA: {me.combatantName} potrzebuje 4 skilli (0: Zatrucie, 1: W oko, 2: Lód, 3: Ogieñ)!</color>");
            return actions;
        }

        CharacterSkill poisonArrow = me.mySkills[0];
        CharacterSkill eyeArrow = me.mySkills[1];
        CharacterSkill iceArrow = me.mySkills[2];
        CharacterSkill fireArrow = me.mySkills[3];

        int round = BattleManager.Instance.currentRound;

        // ========================================================
        // FAZA 1: TOTALNY DEBUFF (Runda 1, 8, 15, 22...)
        // ========================================================
        if (round == 1 || (round - 1) % 7 == 0)
        {
            Debug.Log($"<color=green>{me.combatantName} (Runda {round}): Deszcz os³abieñ! Pe³en ostrza³ debuffów!</color>");

            // 8 morderczych strza³
            actions.Add(new CombatAction { actor = me, target = player, skill = poisonArrow, paInvested = 4, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = poisonArrow, paInvested = 5, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = poisonArrow, paInvested = 2, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = eyeArrow, paInvested = 2, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = eyeArrow, paInvested = 3, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = eyeArrow, paInvested = 3, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = iceArrow, paInvested = 2, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = iceArrow, paInvested = 2, originalIndex = actionCounter++ });

            // Maksymalna obrona fizyczna i mentalna, chroni siê przed wszystkim
            me.defenseMeleePA = 1;
            me.defenseRangedPA = 1;
            me.defenseMentalPA = 1;
        }
        // ========================================================
        // FAZA 2: G£ÓWNY OSTRZA£ OGNIA (Rundy nastêpuj¹ce po debuffie: 2, 9, 16...)
        // ========================================================
        else if (round == 2 || (round - 2) % 7 == 0)
        {
            Debug.Log($"<color=red>{me.combatantName} (Runda {round}): Przebicie pancerza! 2x Ognista Strza³a!</color>");

            actions.Add(new CombatAction { actor = me, target = player, skill = fireArrow, paInvested = 2, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = fireArrow, paInvested = 4, originalIndex = actionCounter++ });
            

            // Opuszcza trochê gardê na magiê (Szansa dla Gracza na u¿ycie potê¿nych zaklêæ!)
            me.defenseMeleePA = 5;
            me.defenseRangedPA = 5;
            me.defenseMentalPA = 5;
        }
        // ========================================================
        // FAZA 3: REGULARNA WALKA (Ca³a reszta rund)
        // ========================================================
        else
        {
            Debug.Log($"<color=orange>{me.combatantName} (Runda {round}): Ogieñ i kontrola t³umu!</color>");

            // 1 potê¿na strza³a ognia do zadawania HP
            actions.Add(new CombatAction { actor = me, target = player, skill = fireArrow, paInvested = 3, originalIndex = actionCounter++ });

            // Oraz mieszanka debuffów do kradzie¿y PA i zatrucia
            actions.Add(new CombatAction { actor = me, target = player, skill = eyeArrow, paInvested = 3, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = iceArrow, paInvested = 3, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = poisonArrow, paInvested = 2, originalIndex = actionCounter++ });

            // Œrednia obrona - zachêca Gracza do ataków Melee (tylko 3 PA w obronie Melee)
            me.defenseMeleePA = 2;
            me.defenseRangedPA = 4;
            me.defenseMentalPA = 1;
        }

        return actions;
    }
}