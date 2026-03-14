using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TestAllSkillsBrain", menuName = "RPG System/AI/Test All Skills Brain")]
public class TestAllSkillsAIBrain : EnemyAIBrain
{
    public override List<CombatAction> DecideTurn(Combatant me, Combatant target, ref int actionCounter)
    {
        List<CombatAction> actions = new List<CombatAction>();

        // Strzelamy ze WSZYSTKICH skilli, jakie ma dodane w pliku!
        for (int i = 0; i < me.mySkills.Count; i++)
        {
            // Omijamy puste okienka
            if (me.mySkills[i] == null || me.mySkills[i].data == null) continue;

            actions.Add(new CombatAction
            {
                actor = me,
                target = target,
                skill = me.mySkills[i],
                paInvested = 1, // Inwestujemy 1 PA w ka¿dy atak
                originalIndex = actionCounter++
            });
        }

        return actions;
    }
}