using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Mage Brain", menuName = "ArenaRPG/AI/Mage Brain")]
public class AIBrain_Mage : EnemyAIBrain
{
    public override List<CombatAction> DecideTurn(Combatant me, Combatant player, ref int actionCounter)
    {
        List<CombatAction> actions = new List<CombatAction>();
        me.ResetDefensePA();

        
        if (me.mySkills.Count < 2)
        {
            
            return actions;
        }

        
        CharacterSkill buffSkill = me.mySkills.Find(s => s.data.category == SkillCategory.PositiveCharm);

        // Mag przeszukuje ksiêgê i znajduje pierwszy skill, który NIE JEST buffem
        CharacterSkill attackSkill = me.mySkills.Find(s => s.data.category != SkillCategory.PositiveCharm);

        if (buffSkill == null || attackSkill == null)
        {
            
            return actions;
        }

        // Pobieramy numer rundy
        int round = BattleManager.Instance.currentRound;

        // Jeœli to runda 1, LUB runda w której tarcza w³aœnie wygas³a (np. runda 4, 7, 10...)
        if (round == 1 || (round - 1) % 6 == 0)
        {
            

            // 1. Akcja: Mag rzuca na SIEBIE buffa
      
            actions.Add(new CombatAction { actor = me, target = me, skill = buffSkill, paInvested = 6, originalIndex = actionCounter++ });

            // 2 i 3. Akcja: Mag poprawia dwoma atakami we wroga
            actions.Add(new CombatAction { actor = me, target = player, skill = attackSkill, paInvested = 5, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = attackSkill, paInvested = 5, originalIndex = actionCounter++ });

           
            me.defenseMeleePA = 0;
            me.defenseRangedPA = 0;
            me.defenseMentalPA = 0;
        }
        else
        {
            // Pozosta³e rundy 
           
            actions.Add(new CombatAction { actor = me, target = player, skill = attackSkill, paInvested = 2, originalIndex = actionCounter++ });
            actions.Add(new CombatAction { actor = me, target = player, skill = attackSkill, paInvested = 2, originalIndex = actionCounter++ });
          
            actions.Add(new CombatAction { actor = me, target = player, skill = attackSkill, paInvested = 3, originalIndex = actionCounter++ });

            me.defenseMeleePA = 0;
            me.defenseRangedPA = 0;
            me.defenseMentalPA = 0;
        }

        return actions;
    }
}