using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Final Boss Brain", menuName = "ArenaRPG/AI/Final Boss Brain")]
public class FinalBossBrain : EnemyAIBrain
{
    public override List<CombatAction> DecideTurn(Combatant me, Combatant player, ref int actionCounter)
    {
        List<CombatAction> actions = new List<CombatAction>();
        me.ResetDefensePA();

        if (me.mySkills.Count < 9)
        {
            return actions;
        }

        CharacterSkill heal = me.mySkills[0];
        CharacterSkill freeze = me.mySkills[1];
        CharacterSkill deepFreeze = me.mySkills[2];
        CharacterSkill wadaWzroku = me.mySkills[3];
        CharacterSkill fireball = me.mySkills[4];

        CharacterSkill poison = me.mySkills[5];
        CharacterSkill madness = me.mySkills[6];    
        CharacterSkill clumsy = me.mySkills[7];

        CharacterSkill suddenDeath = me.mySkills[8];

        int round = BattleManager.Instance.currentRound;

        // PAMIÊÆ HP  (Ignoruje obra¿enia z DoT na starcie rundy)
        float lastRoundHPPercentage = (round == 1) ? 1.0f : ((float)me.hpAtRoundEnd / me.maxHP);

        // SKANY STATUSÓW GRACZA 
        bool hasRegularFreeze = player.activeStatuses.Exists(s => s.type == StatusType.Freeze);
        bool hasDeepFreeze = player.activeStatuses.Exists(s => s.type == StatusType.DeepFreeze);

        bool hasPoison = player.activeStatuses.Exists(s => s.type == StatusType.Poison);
        bool hasMagicArmorDown = player.activeStatuses.Exists(s => s.type == StatusType.VoodooCurse);
        bool hasAccuracyDown = player.activeStatuses.Exists(s => s.type == StatusType.Blindness);

        // GLOBALNY PRIORYTET: UTRZYMANIE ¯YCIA
        // 1. Sprawdzamy, czy w poprzedniej rundzie  (<35% HP)
        // Jeœli tak, dorzuca leczenie na sam pocz¹tek listy, 
        if (lastRoundHPPercentage <= 0.35f)
        {
            actions.Add(new CombatAction { actor = me, target = me, skill = heal, paInvested = 6, originalIndex = actionCounter++ });
        }

        // INSTYNKT 1: EGZEKUCJA (Najwy¿szy priorytet ataku)
        if (hasPoison && hasMagicArmorDown && hasAccuracyDown)
        {

            actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = fireball, paInvested = 6, originalIndex = actionCounter++ });

            me.defenseMeleePA = 0; me.defenseRangedPA = 0; me.defenseMentalPA = 0;
            return actions; 
        }

        // INSTYNKT 2: KOMBOS Lodu (Jeœli gracz ma Freeze, ZAWSZE aktywuje sie Deep Freeze)
        if (hasRegularFreeze && !hasDeepFreeze)
        {

            actions.Add(new CombatAction { actor = me, target = player, skill = deepFreeze, paInvested = 6, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = deepFreeze, paInvested = 6, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = deepFreeze, paInvested = 6, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = wadaWzroku, paInvested = 4, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = clumsy, paInvested = 4, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = fireball, paInvested = 4, originalIndex = actionCounter++ });

            me.defenseMeleePA = 2; me.defenseRangedPA = 2; me.defenseMentalPA = 2;
            return actions;
        }

        // INSTYNKT 3: DARMOWE BICIE (Gracz ma DeepFreeze) -> LOSOWANIE TAKTYKI
        if (hasDeepFreeze)
        {
            int frozenTactic = Random.Range(0, 2);

            if (frozenTactic == 0)
            {
                actions.Add(new CombatAction { actor = me, target = player, skill = madness, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = poison, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = clumsy, paInvested = 6, originalIndex = actionCounter++ });
            }
            else
            {
                actions.Add(new CombatAction { actor = me, target = player, skill = fireball, paInvested = 3, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = fireball, paInvested = 3, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = fireball, paInvested = 2, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = fireball, paInvested = 1, originalIndex = actionCounter++ });
            }

            me.defenseMeleePA = 0; me.defenseRangedPA = 0; me.defenseMentalPA = 0;
            return actions;
        }

        // RUNDA 1: PRZYWITANIE (Wyssanie PA + Próba zamro¿enia)
        if (round == 1)
        {
            Debug.Log($"<color=cyan>{me.combatantName}: Runda 1! Rozpoczyna wysysanie zasobów! Ch³ód i Wada Wzroku!</color>");
            actions.Add(new CombatAction { actor = me, target = player, skill = freeze, paInvested = 5, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = freeze, paInvested = 4, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = freeze, paInvested = 1, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = wadaWzroku, paInvested = 3, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = wadaWzroku, paInvested = 2, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = fireball, paInvested = 4, originalIndex = actionCounter++ });

            me.defenseMeleePA = 3; me.defenseRangedPA = 3; me.defenseMentalPA = 3;
            return actions;
        }

        // NORMALNA WALKA: DYNAMICZNE LOSOWANIE 

        // Losujemy 1 z 4 wrednych taktyk na tê rundê
        int randomTactic = Random.Range(0, 4);

        if (randomTactic == 0)
        {
            // Twardy atak
            actions.Add(new CombatAction { actor = me, target = player, skill = fireball, paInvested = 5, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = fireball, paInvested = 5, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = fireball, paInvested = 5, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = wadaWzroku, paInvested = 3, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = clumsy, paInvested = 3, originalIndex = actionCounter++ });
            me.defenseMeleePA = 4; me.defenseRangedPA = 4; me.defenseMentalPA = 2;
        }
        else if (randomTactic == 1)
        {
            // Debuffy pod Egzekucjê
            actions.Add(new CombatAction { actor = me, target = player, skill = poison, paInvested = 1, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = clumsy, paInvested = 1, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = madness, paInvested = 1, originalIndex = actionCounter++ });
            me.defenseMeleePA = 3; me.defenseRangedPA = 3; me.defenseMentalPA = 4;
        }
        else if (randomTactic == 2)
        {
            // Próba zamro¿enia
            actions.Add(new CombatAction { actor = me, target = player, skill = freeze, paInvested = 6, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = freeze, paInvested = 5, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = fireball, paInvested = 3, originalIndex = actionCounter++ });
            me.defenseMeleePA = 4; me.defenseRangedPA = 3; me.defenseMentalPA = 3;
        }
        else
        {
            // Kradzie¿ PA i utrudnianie ¿ycia
            actions.Add(new CombatAction { actor = me, target = player, skill = wadaWzroku, paInvested = 5, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = clumsy, paInvested = 5, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = madness, paInvested = 3, originalIndex = actionCounter++ });
            me.defenseMeleePA = 5; me.defenseRangedPA = 2; me.defenseMentalPA = 5;
        }

        return actions;
    }
}