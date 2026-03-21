using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Dr Pepper Brain", menuName = "ArenaRPG/AI/Dr Pepper Brain")]
public class AIBrain_DrPepper : EnemyAIBrain
{
    public override List<CombatAction> DecideTurn(Combatant me, Combatant player, ref int actionCounter)
    {
        List<CombatAction> actions = new List<CombatAction>();
        me.ResetDefensePA();

        if (me.mySkills.Count < 4)
        {
            Debug.LogError($"<color=red>UWAGA: {me.combatantName} potrzebuje 4 skilli (0: Tarcza, 1: Inkantacja, 2: Podpalenie, 3: Kula Ognia)!</color>");
            return actions;
        }

        CharacterSkill fireShield = me.mySkills[0];
        CharacterSkill incantation = me.mySkills[1];
        CharacterSkill burnAttack = me.mySkills[2];
        CharacterSkill fireball = me.mySkills[3];

        int round = BattleManager.Instance.currentRound;

        // --- IDEALNY CYKL ZGRANY Z 3-RUNDOW¥ TARCZ¥ ---
        // Okienka w rundach: 4, 9, 14, 19...
        bool isShieldDropRound = (round == 4) || (round > 4 && (round - 4) % 5 == 0);

        // Armagedon w rundach: 5, 10, 15, 20...
        bool isArmageddonRound = (round == 5) || (round > 5 && (round - 5) % 5 == 0);


        if (round == 1)
        {
            // --- RUNDA 1: LOSOWANIE TAKTYKI ---
            int tactic = Random.Range(0, 2);
            Debug.Log($"<color=magenta>{me.combatantName} (Runda 1): Rzut monet¹... Wybrano Taktykê {(tactic == 0 ? "Fizyczn¹ (Podpalenie)" : "Magiczn¹ (Kule Ognia)")}!</color>");

            actions.Add(new CombatAction { actor = me, target = me, skill = fireShield, paInvested = 3, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = me, skill = incantation, paInvested = 5, originalIndex = actionCounter++ });

            if (tactic == 0)
            {
                actions.Add(new CombatAction { actor = me, target = player, skill = burnAttack, paInvested = 3, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = burnAttack, paInvested = 3, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = burnAttack, paInvested = 1, originalIndex = actionCounter++ });
            }
            else
            {
                actions.Add(new CombatAction { actor = me, target = player, skill = fireball, paInvested = 3, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = fireball, paInvested = 3, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = fireball, paInvested = 1, originalIndex = actionCounter++ });
            }

            me.defenseMeleePA = 0;
            me.defenseRangedPA = 0;
            me.defenseMentalPA = 0;
        }
        else if (isArmageddonRound)
        {
            // --- ARMAGEDON (Gracz musi u¿yæ modlitwy!) ---
            Debug.Log($"<color=red>{me.combatantName} (Runda {round}): ARMAGEDON! £aduje 2 potê¿ne Kule Ognia!</color>");

            actions.Add(new CombatAction { actor = me, target = player, skill = fireball, paInvested = 6, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = fireball, paInvested = 6, originalIndex = actionCounter++ });

            me.defenseMeleePA = 1;
            me.defenseRangedPA = 0;
            me.defenseMentalPA = 0;
        }
        else if (isShieldDropRound)
        {
            // --- OKIENKO DLA GRACZA (Tarcza spada naturalnie!) ---
            Debug.Log($"<color=yellow>{me.combatantName} (Runda {round}): Tarcza opad³a! Mag szykuje siê do Armagedonu!</color>");

            actions.Add(new CombatAction { actor = me, target = me, skill = incantation, paInvested = 4, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = burnAttack, paInvested = 2, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = fireball, paInvested = 2, originalIndex = actionCounter++ });

            me.defenseMeleePA = 2;
            me.defenseRangedPA = 2;
            me.defenseMentalPA = 2;
        }
        else
        {
            // --- STANDARDOWE RUNDY (INTELIGENTNE SPRAWDZANIE TARCZY) ---
            bool hasShield = me.activeStatuses.Exists(s => s.type == StatusType.FireShield);

            if (!hasShield)
            {
                // Jeœli nie ma tarczy (np. Runda 6 po Armagedonie), rzuca j¹!
                Debug.Log($"<color=orange>{me.combatantName} (Runda {round}): Nak³ada now¹ Tarczê Ognia!</color>");
                actions.Add(new CombatAction { actor = me, target = me, skill = fireShield, paInvested = 4, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = burnAttack, paInvested = 3, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = fireball, paInvested = 3, originalIndex = actionCounter++ });

                me.defenseMeleePA = 5;
                me.defenseRangedPA = 3;
                me.defenseMentalPA = 3;
            }
            else
            {
                // Jeœli tarcza ju¿ na nim wisi (np. Rundy 2, 3, 7, 8), bije znacznie mocniej!
                Debug.Log($"<color=orange>{me.combatantName} (Runda {round}): Tarcza wci¹¿ aktywna, skupia siê na ataku!</color>");
                actions.Add(new CombatAction { actor = me, target = player, skill = burnAttack, paInvested = 4, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = burnAttack, paInvested = 2, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = fireball, paInvested = 4, originalIndex = actionCounter++ });

                me.defenseMeleePA = 4;
                me.defenseRangedPA = 3;
                me.defenseMentalPA = 3;
            }
        }

        return actions;
    }
}