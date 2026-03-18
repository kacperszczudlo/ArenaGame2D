using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Mage Brain", menuName = "ArenaRPG/AI/Mage Brain")]
public class AIBrain_Mage : EnemyAIBrain
{
    public override List<CombatAction> DecideTurn(Combatant me, Combatant player, ref int actionCounter)
    {
        List<CombatAction> actions = new List<CombatAction>();
        me.ResetDefensePA();

        // Zabezpieczenie: Mag potrzebuje Tarczy (index 0) i Ataku (index 1)
        if (me.mySkills.Count < 2)
        {
            Debug.LogError($"<color=red>UWAGA: {me.combatantName} nie ma przypisanych 2 skilli!</color>");
            return actions;
        }

        // Pobieramy skille z listy Gracza (w Unity upewnij siê, ¿e Tarcza jest pierwsza, a Fireball drugi!)
        // Mag przeszukuje swoj¹ ksiêgê i znajduje pierwszy skill, który jest POZYTYWNY (Buff)
        CharacterSkill buffSkill = me.mySkills.Find(s => s.data.category == SkillCategory.PositiveCharm);

        // Mag przeszukuje ksiêgê i znajduje pierwszy skill, który NIE JEST buffem (czyli Atak)
        CharacterSkill attackSkill = me.mySkills.Find(s => s.data.category != SkillCategory.PositiveCharm);

        // Zabezpieczenie, jakby jednak czegoœ brakowa³o
        if (buffSkill == null || attackSkill == null)
        {
            Debug.LogError($"<color=red>{me.combatantName} nie mo¿e znaleŸæ swoich czarów! Potrzebuje 1 buffa i 1 ataku.</color>");
            return actions;
        }

        // Pobieramy numer rundy
        int round = BattleManager.Instance.currentRound;

        // Jeœli to runda 1, LUB runda w której tarcza w³aœnie wygas³a (np. runda 4, 7, 10...)
        if (round == 1 || (round - 1) % 6 == 0)
        {
            Debug.Log($"<color=orange>{me.combatantName} (Runda {round}) Taktyka 1: Tarcza i 2x Atak!</color>");

            // 1. Akcja: Mag rzuca na SIEBIE buffa
            // Zauwa¿, ¿e da³em target = me, choæ BattleManager i tak by to sam poprawi³!
            actions.Add(new CombatAction { actor = me, target = me, skill = buffSkill, paInvested = 6, originalIndex = actionCounter++ });

            // 2 i 3. Akcja: Mag poprawia dwoma atakami we wroga
            actions.Add(new CombatAction { actor = me, target = player, skill = attackSkill, paInvested = 5, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = attackSkill, paInvested = 5, originalIndex = actionCounter++ });

            // Magiczna klasa = mocniejsza obrona magiczna (Mental), s³absza fizyczna (Melee)
            me.defenseMeleePA = 0;
            me.defenseRangedPA = 0;
            me.defenseMentalPA = 0;
        }
        else
        {
            // Pozosta³e rundy (kiedy tarcza na nim bezpiecznie wisi)
            Debug.Log($"<color=red>{me.combatantName} (Runda {round}) Taktyka 2: Furia Ognia (3x Atak)!</color>");

            // Zasypuje wroga czarami
            actions.Add(new CombatAction { actor = me, target = player, skill = attackSkill, paInvested = 2, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = attackSkill, paInvested = 2, originalIndex = actionCounter++ });
            // Trzeci atak jest potê¿niejszy (3 PA)
            actions.Add(new CombatAction { actor = me, target = player, skill = attackSkill, paInvested = 3, originalIndex = actionCounter++ });

            // Opuszcza gardê, ¿eby wiêcej atakowaæ
            me.defenseMeleePA = 0;
            me.defenseRangedPA = 0;
            me.defenseMentalPA = 0;
        }

        return actions;
    }
}