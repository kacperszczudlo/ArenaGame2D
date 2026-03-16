using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Voodoo Brain", menuName = "ArenaRPG/AI/Voodoo Brain")]
public class AIBrain_Voodoo : EnemyAIBrain
{
    public override List<CombatAction> DecideTurn(Combatant me, Combatant player, ref int actionCounter)
    {
        List<CombatAction> actions = new List<CombatAction>();
        me.ResetDefensePA();

        // Zabezpieczenie: Sprawdzamy, czy Kap³an ma w³o¿one 3 skille w Inspektorze
        if (me.mySkills.Count < 3)
        {
            Debug.LogError($"<color=red>UWAGA: {me.combatantName} potrzebuje 3 skilli (0: Zatrucie, 1: Szpila, 2: Atak Mentalny)!</color>");
            return actions;
        }

        // Pobieramy skille do wygodnych zmiennych
        CharacterSkill poisonSkill = me.mySkills[0];
        CharacterSkill clumsinessSkill = me.mySkills[1]; // Twoja "Szpila" / Niezdarnoœæ
        CharacterSkill mentalAttack = me.mySkills[2];

        int round = BattleManager.Instance.currentRound;

        if (round == 1)
        {
            // --- RUNDA 1: PE£EN PAKIET POWITALNY ---
            Debug.Log($"<color=purple>{me.combatantName} (Runda 1): Zestaw powitalny - Trucizna, Szpila");

            actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 6, originalIndex = actionCounter++ });

            actions.Add(new CombatAction { actor = me, target = player, skill = poisonSkill, paInvested = 2, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = poisonSkill, paInvested = 3, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = poisonSkill, paInvested = 4, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = poisonSkill, paInvested = 6, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = poisonSkill, paInvested = 6, originalIndex = actionCounter++ });



            actions.Add(new CombatAction { actor = me, target = player, skill = clumsinessSkill, paInvested = 2, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = clumsinessSkill, paInvested = 2, originalIndex = actionCounter++ });





            me.defenseMeleePA = 5;
            me.defenseRangedPA = 5;
            me.defenseMentalPA = 5;
        }
        else if (round == 2 || round == 3)
        {
            actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 6, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = clumsinessSkill, paInvested = 3, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = clumsinessSkill, paInvested = 1, originalIndex = actionCounter++ });

            me.defenseMeleePA = 3;
            me.defenseRangedPA = 3;
            me.defenseMentalPA = 3;

        }
        else if (round >= 4 && round <=6)
        {
            actions.Add(new CombatAction { actor = me, target = player, skill = clumsinessSkill, paInvested = 3, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = clumsinessSkill, paInvested = 1, originalIndex = actionCounter++ });
            me.defenseMeleePA = 2;
            me.defenseRangedPA = 2;
            me.defenseMentalPA = 2;
        }

        else if (round >= 7 && round <= 9)
        {
            

            me.defenseMeleePA = 3;
            me.defenseRangedPA = 1;
            me.defenseMentalPA = 1;
        }
        else
        {



            // Utrzymuje debuff na celnoœæ
            actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 6, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = clumsinessSkill, paInvested = 6, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = poisonSkill, paInvested = 6, originalIndex = actionCounter++ });

            // Uderza z ca³ej si³y w umys³ gracza
            
            

            
            me.defenseMeleePA = 1;
            me.defenseRangedPA = 0;
            me.defenseMentalPA = 0;
        }

        return actions;
    }
}