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

        // Pobieramy aktualny numer rundy prosto z Mened¿era Walki!
        int round = BattleManager.Instance.currentRound;

        Debug.Log($"<color=orange>{me.combatantName} analizuje taktykê dla Rundy {round}!</color>");

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
            // --- RUNDA 3, 6, 9, 12... (Co trzeci¹ rundê) ---
            // £ucznik opuszcza gardê i ³aduje potê¿ny, morderczy strza³ za 5 PA!
            Debug.Log($"<color=red>UWAGA! {me.combatantName} ³aduje potê¿ny strza³!</color>");
            actions.Add(new CombatAction { actor = me, target = player, skill = normalArrow, paInvested = 5, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = normalArrow, paInvested = 5, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = normalArrow, paInvested = 5, originalIndex = actionCounter++ });



            me.defenseMeleePA = 1;
            me.defenseRangedPA = 0;
            me.defenseMentalPA = 0;
        }
        else
        {
            // --- POZOSTA£E RUNDY (np. 2, 4, 5, 7...) ---
            // Standardowy, powtarzalny ostrza³. Dwie szybkie strza³y i bezpieczna obrona.
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