using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Rico Brain", menuName = "ArenaRPG/AI/Rico Brain")]
public class AIBrain_TankKnight : EnemyAIBrain
{
    public override List<CombatAction> DecideTurn(Combatant me, Combatant player, ref int actionCounter)
    {
        List<CombatAction> actions = new List<CombatAction>();
        me.ResetDefensePA();

        if (me.mySkills.Count < 6)
        {
            Debug.LogError($"<color=red>UWAGA: {me.combatantName} potrzebuje 6 skilli (0: Blok, 1: Atak, 2: Furia, 3: Leczenie, 4: Modlitwa, 5: Trucizna)!</color>");
            return actions;
        }

        CharacterSkill blockSkill = me.mySkills[0];
        CharacterSkill heavyAttack = me.mySkills[1];
        CharacterSkill furySkill = me.mySkills[2];
        CharacterSkill healSkill = me.mySkills[3];
        CharacterSkill blessingSkill = me.mySkills[4];
        CharacterSkill poisonAttack = me.mySkills[5];

        int round = BattleManager.Instance.currentRound;

        // --- PRZELICZAMY SUROWE HP Z ZESZ£EJ RUNDY NA PROCENT ---
        // U¿ywamy tego do decydowania o taktyce, by zignorowaæ obra¿enia z trucizny na starcie aktualnej rundy!
        float lastRoundHPPercentage = (round == 1) ? 1.0f : ((float)me.hpAtRoundEnd / me.maxHP);

        // ==========================================
        // FAZA 4: EGZEKUCJA - HP <= 20% Z POPRZEDNIEJ RUNDY (KARA DLA GRACZA)
        // ==========================================
        if (lastRoundHPPercentage > 0f && lastRoundHPPercentage <= 0.20f)
        {
            Debug.Log($"<color=darkred>{me.combatantName} (Zesz³e HP: {Mathf.RoundToInt(lastRoundHPPercentage * 100)}%): GOD MODE! Kara za brak dobicia!</color>");

            // Pe³ne leczenie i mordercze buffy
            for (int i = 0; i < 12; i++) actions.Add(new CombatAction { actor = me, target = me, skill = healSkill, paInvested = 6, originalIndex = actionCounter++ });
            for (int i = 0; i < 3; i++) actions.Add(new CombatAction { actor = me, target = me, skill = furySkill, paInvested = 6, originalIndex = actionCounter++ });

            actions.Add(new CombatAction { actor = me, target = me, skill = blessingSkill, paInvested = 6, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = me, skill = blockSkill, paInvested = 6, originalIndex = actionCounter++ });

            for (int i = 0; i < 14; i++) actions.Add(new CombatAction { actor = me, target = player, skill = heavyAttack, paInvested = 6, originalIndex = actionCounter++ });

            me.defenseMeleePA = 6;
            me.defenseRangedPA = 6;
            me.defenseMentalPA = 6;
        }
        // ==========================================
        // FAZA 3: "Z£OTE OKIENKO" - HP > 20% i <= 30% 
        // ==========================================
        else if (lastRoundHPPercentage > 0.20f && lastRoundHPPercentage <= 0.30f)
        {
            Debug.Log($"<color=magenta>{me.combatantName} (Zesz³e HP: {Mathf.RoundToInt(lastRoundHPPercentage * 100)}%): Z£OTE OKIENKO! Szok bojowy, rycerz rezygnuje z leczenia i szykuje siê na œmieræ!</color>");

            // Brak leczenia! Tylko ataki - idealny moment dla gracza na uderzenie z pe³n¹ si³¹!
            actions.Add(new CombatAction { actor = me, target = player, skill = heavyAttack, paInvested = 6, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = heavyAttack, paInvested = 5, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 1, originalIndex = actionCounter++ });

            // Doœæ s³aba obrona, u³atwia dobicie
            me.defenseMeleePA = 1;
            me.defenseRangedPA = 2;
            me.defenseMentalPA = 1;
        }
        // ==========================================
        // FAZA 2: ZMÊCZENIE MATERIA£U - HP > 30% i <= 50% 
        // ==========================================
        else if (lastRoundHPPercentage > 0.30f && lastRoundHPPercentage <= 0.50f)
        {
            Debug.Log($"<color=orange>{me.combatantName} (Zesz³e HP: {Mathf.RoundToInt(lastRoundHPPercentage * 100)}%): Pancerz pêka! Leczy siê i twardo stoi!</color>");

            actions.Add(new CombatAction { actor = me, target = me, skill = healSkill, paInvested = 6, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = heavyAttack, paInvested = 1, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = heavyAttack, paInvested = 2, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 1, originalIndex = actionCounter++ });

            me.defenseMeleePA = 2;
            me.defenseRangedPA = 3;
            me.defenseMentalPA = 2;
        }
        // ==========================================
        // FAZA 1: PANCERNY CZO£G - HP Powy¿ej 50%
        // ==========================================
        else
        {
            if (round % 4 == 1)
            {
                Debug.Log($"<color=blue>{me.combatantName} (Runda {round}): Nie do ruszenia. Blok, Modlitwa i Trucizna.</color>");
                actions.Add(new CombatAction { actor = me, target = me, skill = blockSkill, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = me, skill = blessingSkill, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 2, originalIndex = actionCounter++ });

                me.defenseMeleePA = 5;
                me.defenseRangedPA = 5;
                me.defenseMentalPA = 3;
            }
            else if (round % 4 == 2)
            {
                actions.Add(new CombatAction { actor = me, target = me, skill = blockSkill, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = heavyAttack, paInvested = 2, originalIndex = actionCounter++ });

                me.defenseMeleePA = 3;
                me.defenseRangedPA = 3;
                me.defenseMentalPA = 3;
            }
            else if (round % 4 == 3)
            {
                Debug.Log($"<color=yellow>{me.combatantName} (Runda {round}): Wœciek³y kontratak zza tarczy!</color>");
                actions.Add(new CombatAction { actor = me, target = me, skill = blockSkill, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = heavyAttack, paInvested = 4, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = heavyAttack, paInvested = 4, originalIndex = actionCounter++ });

                me.defenseMeleePA = 3;
                me.defenseRangedPA = 3;
                me.defenseMentalPA = 3;
            }
            else
            {
                Debug.Log($"<color=orange>{me.combatantName} (Runda {round}): Rozdaje mocarne ciosy z wci¹¿ podniesion¹ tarcz¹!</color>");
                actions.Add(new CombatAction { actor = me, target = me, skill = blockSkill, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = heavyAttack, paInvested = 3, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = heavyAttack, paInvested = 2, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = heavyAttack, paInvested = 5, originalIndex = actionCounter++ });

                me.defenseMeleePA = 2;
                me.defenseRangedPA = 2;
                me.defenseMentalPA = 2;
            }
        }

        return actions;
    }
}