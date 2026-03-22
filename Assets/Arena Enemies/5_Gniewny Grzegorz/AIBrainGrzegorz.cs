using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Grzegorz Brain", menuName = "ArenaRPG/AI/Grzegorz Brain")]
public class AIBrain_Grzegorz : EnemyAIBrain
{
    public override List<CombatAction> DecideTurn(Combatant me, Combatant player, ref int actionCounter)
    {
        List<CombatAction> actions = new List<CombatAction>();
        me.ResetDefensePA();

        if (me.mySkills.Count < 5)
        {
            return actions;
        }

        CharacterSkill blockSkill = me.mySkills[0];
        CharacterSkill furySkill = me.mySkills[1];
        CharacterSkill blessingSkill = me.mySkills[2];
        CharacterSkill poisonAttack = me.mySkills[3];
        CharacterSkill fastAttack = me.mySkills[4];

        int round = BattleManager.Instance.currentRound;

        // ---  procent zdrowia  (od 0.0 do 1.0) ---
        float hpPercentage = (float)me.currentHP / me.maxHP;

        bool hasFury = me.activeStatuses.Exists(s => s.type == StatusType.Fury);

        // FAZA 2: ZDROWIE PONI¯EJ 70%
        if (hpPercentage <= 0.70f)
        {
            if (!hasFury)
            {
                // --- (Odpali siê tylko raz, gdy spadnie do 70%) ---

                actions.Add(new CombatAction { actor = me, target = me, skill = furySkill, paInvested = 4, originalIndex = actionCounter++ });

                // Mordercze kombo
                actions.Add(new CombatAction { actor = me, target = player, skill = fastAttack, paInvested = 4, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = fastAttack, paInvested = 4, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = fastAttack, paInvested = 4, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = fastAttack, paInvested = 4, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = fastAttack, paInvested = 4, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = fastAttack, paInvested = 3, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 2, originalIndex = actionCounter++ });

                me.defenseMeleePA = 0;
                me.defenseRangedPA = 0;
                me.defenseMentalPA = 0;
            }
            else
            {
                // --- FAZA 3: PO FURII  ---
                actions.Add(new CombatAction { actor = me, target = player, skill = blockSkill, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = fastAttack, paInvested = 3, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 2, originalIndex = actionCounter++ });

                me.defenseMeleePA = 4;
                me.defenseRangedPA = 2;
                me.defenseMentalPA = 1;
            }
        }
        // FAZA 1: ZDROWIE POWY¯EJ 70% (Pocz¹tek walki)
        else
        {
            if (round == 1 || round == 2)
            {

                actions.Add(new CombatAction { actor = me, target = me, skill = blockSkill, paInvested = 6, originalIndex = actionCounter++ });

                if (round == 1)
                    actions.Add(new CombatAction { actor = me, target = me, skill = blessingSkill, paInvested = 6, originalIndex = actionCounter++ });

                actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 2, originalIndex = actionCounter++ });

                if (round == 1)
                    actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 2, originalIndex = actionCounter++ });

                me.defenseMeleePA = 5;
                me.defenseRangedPA = 4;
                me.defenseMentalPA = 2;
            }
            else
            {

                actions.Add(new CombatAction { actor = me, target = player, skill = fastAttack, paInvested = 3, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 2, originalIndex = actionCounter++ });

                me.defenseMeleePA = 3;
                me.defenseRangedPA = 3;
                me.defenseMentalPA = 3;
            }
        }

        return actions;
    }
}