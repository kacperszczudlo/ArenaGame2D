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
            Debug.LogError($"<color=red>UWAGA: {me.combatantName} potrzebuje 5 skilli (0: Blokada, 1: Furia, 2: Modlitwa, 3: Zatrucie, 4: Szybki Atak)!</color>");
            return actions;
        }

        CharacterSkill blockSkill = me.mySkills[0];
        CharacterSkill furySkill = me.mySkills[1];
        CharacterSkill blessingSkill = me.mySkills[2];
        CharacterSkill poisonAttack = me.mySkills[3];
        CharacterSkill fastAttack = me.mySkills[4];

        int round = BattleManager.Instance.currentRound;

        // ---  procent zdrowia Grzegorza (od 0.0 do 1.0) ---
        float hpPercentage = (float)me.currentHP / me.maxHP;

        // Sprawdzamy, czy Grzegorz rzuci³ ju¿ na siebie Furiê w tej walce
        bool hasFury = me.activeStatuses.Exists(s => s.type == StatusType.Fury);

        // ==========================================
        // FAZA 2: ZDROWIE PONI¯EJ 70%
        // ==========================================
        if (hpPercentage <= 0.70f)
        {
            if (!hasFury)
            {
                // --- TO JEST TWOJE "OKIENKO" (Odpali siê tylko raz, gdy spadnie do 70%) ---
                Debug.Log($"<color=red>{me.combatantName} (HP: {Mathf.RoundToInt(hpPercentage * 100)}%): SZA£ BITEWNY! Odrzuca tarczê i szar¿uje!</color>");

                // Rzuca na siebie Furiê (dziêki temu gra zapamiêta, ¿e okienko zosta³o otwarte)
                actions.Add(new CombatAction { actor = me, target = me, skill = furySkill, paInvested = 4, originalIndex = actionCounter++ });

                // Mordercze kombo
                actions.Add(new CombatAction { actor = me, target = player, skill = fastAttack, paInvested = 4, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = fastAttack, paInvested = 4, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = fastAttack, paInvested = 4, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = fastAttack, paInvested = 4, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = fastAttack, paInvested = 4, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = fastAttack, paInvested = 3, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 2, originalIndex = actionCounter++ });

                // --- ZERO OBRONY - GRACZ MUSI GO TERAZ ZABIÆ ALBO MOCNO OBIÆ ---
                me.defenseMeleePA = 0;
                me.defenseRangedPA = 0;
                me.defenseMentalPA = 0;
            }
            else
            {
                // --- FAZA 3: PO FURII (Walka o przetrwanie) ---
                // Grzegorz zu¿y³ ju¿ okienko Furii, wie, ¿e ma ma³o HP, wiêc walczy desperacko
                Debug.Log($"<color=orange>{me.combatantName}: Ostatnie tchnienie!</color>");
                actions.Add(new CombatAction { actor = me, target = player, skill = blockSkill, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = fastAttack, paInvested = 3, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 2, originalIndex = actionCounter++ });

                // Wraca do umiarkowanej obrony
                me.defenseMeleePA = 4;
                me.defenseRangedPA = 2;
                me.defenseMentalPA = 1;
            }
        }
        // ==========================================
        // FAZA 1: ZDROWIE POWY¯EJ 70% (Pocz¹tek walki)
        // ==========================================
        else
        {
            if (round == 1 || round == 2)
            {
                // MURY OBRONNE
                Debug.Log($"<color=blue>{me.combatantName} (Runda {round}): Tytanowa Garda!</color>");

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
                // ZMIÊKCZANIE (Czeka a¿ gracz zjedzie mu HP poni¿ej 70%)
                Debug.Log($"<color=yellow>{me.combatantName} (Runda {round}): Szuka otwarcia, opuszcza gardê.</color>");

                
                actions.Add(new CombatAction { actor = me, target = player, skill = fastAttack, paInvested = 3, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 2, originalIndex = actionCounter++ });

                // Obrona obni¿ona – tutaj gracz musi pchn¹æ go poni¿ej 70% HP!
                me.defenseMeleePA = 3;
                me.defenseRangedPA = 3;
                me.defenseMentalPA = 3;
            }
        }

        return actions;
    }
}