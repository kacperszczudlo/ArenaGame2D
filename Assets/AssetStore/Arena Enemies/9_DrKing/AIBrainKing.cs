using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "King Brain", menuName = "ArenaRPG/AI/King Brain")]
public class AIBrain_King : EnemyAIBrain
{
    // Zmienna zapamiêtuj¹ca wylosowan¹ taktykê na ca³¹ walkê
    [System.NonSerialized] private int chosenTactic = -1;

    [Header("Debug / Testowanie")]
    [Tooltip("Wpisz numer od 0 do 3 ¿eby wymusiæ taktykê. Zostaw -1 ¿eby dzia³a³o losowo!")]
    public int forceTactic = -1;

    public override List<CombatAction> DecideTurn(Combatant me, Combatant player, ref int actionCounter)
    {
        List<CombatAction> actions = new List<CombatAction>();
        me.ResetDefensePA();

        if (me.mySkills.Count < 8)
        {
            return actions;
        }

        CharacterSkill shadowAttack = me.mySkills[0];
        CharacterSkill magicArmorDown = me.mySkills[1];
        CharacterSkill poisonAttack = me.mySkills[2];
        CharacterSkill accuracyDown = me.mySkills[3];
        CharacterSkill meleeAttack = me.mySkills[4];
        CharacterSkill mentalAttack = me.mySkills[5];
        CharacterSkill suddenDeath = me.mySkills[6];
        CharacterSkill charmBleed = me.mySkills[7];

        int round = BattleManager.Instance.currentRound;

        // RUNDA 1: LOSOWANIE TAKTYKI NA CA£¥ WALKÊ!
        if (round == 1)
        {
            if (forceTactic != -1)
            {
                chosenTactic = forceTactic;
            }
            else
            {
                int roll = Random.Range(1, 101);

                if (roll <= 10)
                {
                    chosenTactic = 0; // 10% Szans - TRYB ZABÓJCY (Nag³a Œmieræ)
                }
                else if (roll <= 40)
                {
                    chosenTactic = 1; // 30% Szans - TRYB WOJOWNIKA (Zwarcie + Cieñ)
                }
                else if (roll <= 70)
                {
                    chosenTactic = 2; // 30% Szans - TRYB W£ADCY UMYS£ÓW (Psychika + Debuffy)
                }
                else
                {
                    chosenTactic = 3;
                }
            }
        }


        // TAKTYKA 0: EGZEKUCJA (All-in w Rundzie 1)
        if (chosenTactic == 0)
        {
            if (round == 1)
            {
                actions.Add(new CombatAction { actor = me, target = player, skill = magicArmorDown, paInvested = 1, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = magicArmorDown, paInvested = 1, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = magicArmorDown, paInvested = 3, originalIndex = actionCounter++ });

                actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 3, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 3, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 3, originalIndex = actionCounter++ });

                actions.Add(new CombatAction { actor = me, target = player, skill = accuracyDown, paInvested = 3, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = accuracyDown, paInvested = 3, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = accuracyDown, paInvested = 3, originalIndex = actionCounter++ });

                actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });

                me.defenseMeleePA = 0; me.defenseRangedPA = 0; me.defenseMentalPA = 0;
            }
            else
            {
                // RUNDY 2+: SPRAWDZANIE STATUSÓW GRACZA
                bool hasPoison = player.activeStatuses.Exists(s => s.type == StatusType.Poison);
                bool hasMagicArmorDown = player.activeStatuses.Exists(s => s.type == StatusType.VoodooCurse); // Zmieñ na swój typ obni¿enia defa
                bool hasAccuracyDown = player.activeStatuses.Exists(s => s.type == StatusType.Blindness); // Zmieñ na swój typ utraty celnoœci

                if (hasPoison && hasMagicArmorDown && hasAccuracyDown)
                {

                    actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                    

                    me.defenseMeleePA = 0; me.defenseRangedPA = 0; me.defenseMentalPA = 0;
                }
                else
                {
                    int subTactic = Random.Range(0, 3); 

                    if (subTactic == 0)
                    {
                        actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 3, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 4, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 3, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = accuracyDown, paInvested = 3, originalIndex = actionCounter++ });

                        me.defenseMeleePA = 2; me.defenseRangedPA = 2; me.defenseMentalPA = 2;
                    }
                    else if (subTactic == 1)
                    {
                        actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 1, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 3, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 1, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = magicArmorDown, paInvested = 3, originalIndex = actionCounter++ });

                        me.defenseMeleePA = 2; me.defenseRangedPA = 2; me.defenseMentalPA = 2;
                    }
                    else
                    {
                        actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 3, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 3, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 2, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = charmBleed, paInvested = 5, originalIndex = actionCounter++ });

                        me.defenseMeleePA = 2; me.defenseRangedPA = 2; me.defenseMentalPA = 2;
                    }
                }
            }
        }
        // TAKTYKA 1: BRUTALNA SI£A 
        else if (chosenTactic == 1)
        {
            // 1. UNIVERSALNE SPRAWDZENIE EGZEKUCJI 
            bool hasPoison = player.activeStatuses.Exists(s => s.type == StatusType.Poison);
            bool hasMagicArmorDown = player.activeStatuses.Exists(s => s.type == StatusType.VoodooCurse);
            bool hasAccuracyDown = player.activeStatuses.Exists(s => s.type == StatusType.Blindness);

            if (hasPoison && hasMagicArmorDown && hasAccuracyDown)
            {

                actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 5, originalIndex = actionCounter++ });

                me.defenseMeleePA = 0; me.defenseRangedPA = 0; me.defenseMentalPA = 0;
            }
            else
            {
                // 2. NORMALNA WALKA WOJOWNIKA
                if (round == 1)
                {
                    // RUNDA 1: Oœlepienie i grad ciosów

                    actions.Add(new CombatAction { actor = me, target = player, skill = accuracyDown, paInvested = 4, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = accuracyDown, paInvested = 4, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 6, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 6, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 6, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 6, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 6, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 6, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 6, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 6, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 6, originalIndex = actionCounter++ });

                    me.defenseMeleePA = 1; me.defenseRangedPA = 1; me.defenseMentalPA = 1; 
                }
                else
                {
                    // RUNDY 2+: Losowe komba z debuffami
                    int subTactic = Random.Range(0, 3);

                    if (subTactic == 0)
                    {
                        actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 4, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 2, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 2, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 5, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 5, originalIndex = actionCounter++ });

                        me.defenseMeleePA = 3; me.defenseRangedPA = 2; me.defenseMentalPA = 2;
                    }
                    else if (subTactic == 1)
                    {
                        actions.Add(new CombatAction { actor = me, target = player, skill = accuracyDown, paInvested = 2, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = accuracyDown, paInvested = 2, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 6, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 6, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = charmBleed, paInvested = 3, originalIndex = actionCounter++ });

                        me.defenseMeleePA = 2; me.defenseRangedPA = 2; me.defenseMentalPA = 2;
                    }
                    else
                    {
                        actions.Add(new CombatAction { actor = me, target = player, skill = magicArmorDown, paInvested = 6, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = magicArmorDown, paInvested = 4, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 6, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 6, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 4, originalIndex = actionCounter++ });

                        me.defenseMeleePA = 1; me.defenseRangedPA = 1; me.defenseMentalPA = 1;
                    }
                }
            }
        }
        else if (chosenTactic == 2)
        {
            // 1. UNIVERSALNE SPRAWDZENIE EGZEKUCJI
            bool hasPoison = player.activeStatuses.Exists(s => s.type == StatusType.Poison);
            bool hasMagicArmorDown = player.activeStatuses.Exists(s => s.type == StatusType.VoodooCurse);
            bool hasAccuracyDown = player.activeStatuses.Exists(s => s.type == StatusType.Blindness);

            if (hasPoison && hasMagicArmorDown && hasAccuracyDown)
            {
                actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 5, originalIndex = actionCounter++ });

                me.defenseMeleePA = 0; me.defenseRangedPA = 0; me.defenseMentalPA = 0;
            }
            else
            {
                // 2. NORMALNA WALKA W£ADCY UMYS£ÓW
                if (round == 1)
                {
                    // RUNDA 1: Zdejmuje pancerz magiczny 

                    actions.Add(new CombatAction { actor = me, target = player, skill = magicArmorDown, paInvested = 4, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = magicArmorDown, paInvested = 6, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = magicArmorDown, paInvested = 6, originalIndex = actionCounter++ });


                    actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 5, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 5, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 5, originalIndex = actionCounter++ });
                    

                    me.defenseMeleePA = 1; me.defenseRangedPA = 1; me.defenseMentalPA = 1;
                }
                else
                {
                    // RUNDY 2+: Losowe komba umys³owe 
                    int subTactic = Random.Range(0, 3);

                    if (subTactic == 0)
                    {
                        actions.Add(new CombatAction { actor = me, target = player, skill = magicArmorDown, paInvested = 4, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 4, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = charmBleed, paInvested = 6, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 2, originalIndex = actionCounter++ });
                        

                        me.defenseMeleePA = 3; me.defenseRangedPA = 3; me.defenseMentalPA = 3;
                    }
                    else if (subTactic == 1)
                    {
                        actions.Add(new CombatAction { actor = me, target = player, skill = magicArmorDown, paInvested = 4, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 4, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 6, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 3, originalIndex = actionCounter++ });

                        me.defenseMeleePA = 2; me.defenseRangedPA = 2; me.defenseMentalPA = 2;
                    }
                    else
                    {
                        actions.Add(new CombatAction { actor = me, target = player, skill = accuracyDown, paInvested = 4, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 6, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 1, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 1, originalIndex = actionCounter++ });


                        me.defenseMeleePA = 1; me.defenseRangedPA = 1; me.defenseMentalPA = 1;
                    }
                }
            }
        }
        // TAKTYKA 3: DYSTANS
        // ------------------------------------------------
        else if (chosenTactic == 3)
        {
            // 1. UNIVERSALNE SPRAWDZENIE EGZEKUCJI
            bool hasPoison = player.activeStatuses.Exists(s => s.type == StatusType.Poison);
            bool hasMagicArmorDown = player.activeStatuses.Exists(s => s.type == StatusType.VoodooCurse);
            bool hasAccuracyDown = player.activeStatuses.Exists(s => s.type == StatusType.Blindness);

            if (hasPoison && hasMagicArmorDown && hasAccuracyDown)
            {

                actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 5, originalIndex = actionCounter++ });

                me.defenseMeleePA = 0; me.defenseRangedPA = 0; me.defenseMentalPA = 0;
            }
            else
            {
                // 2. NORMALNA WALKA Z CIENIA
                if (round == 1)
                {
                    // RUNDA 1: Zatruta strza³a i zasypanie atakami z cienia
                    Debug.Log($"<color=green>{me.combatantName} (Cieñ): Wrzuca truciznê i znika w ciemnoœciach!</color>");

                    actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 4, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = accuracyDown, paInvested = 4, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 5, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 5, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 6, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 6, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 6, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 6, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 6, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 6, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 6, originalIndex = actionCounter++ });

                    me.defenseMeleePA = 1; me.defenseRangedPA = 1; me.defenseMentalPA = 1;
                }
                else
                {
                    // RUNDY 2+: Losowe komba dystansowe
                    int subTactic = Random.Range(0, 3);

                    if (subTactic == 0)
                    {
                        actions.Add(new CombatAction { actor = me, target = player, skill = accuracyDown, paInvested = 4, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = accuracyDown, paInvested = 2, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 5, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 5, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 5, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 5, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 5, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 3, originalIndex = actionCounter++ });

                        me.defenseMeleePA = 2; me.defenseRangedPA = 2; me.defenseMentalPA = 2;
                    }
                    else if (subTactic == 1)
                    {
                        actions.Add(new CombatAction { actor = me, target = player, skill = magicArmorDown, paInvested = 4, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = magicArmorDown, paInvested = 2, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 6, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 5, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 5, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 5, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 3, originalIndex = actionCounter++ });

                        me.defenseMeleePA = 1; me.defenseRangedPA = 1; me.defenseMentalPA = 1;
                    }
                    else
                    {
                        
                        actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 4, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 2, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = charmBleed, paInvested = 4, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 5, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 5, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 5, originalIndex = actionCounter++ });

                        me.defenseMeleePA = 3; me.defenseRangedPA = 3; me.defenseMentalPA = 3;
                    }
                }
            }
        }

        return actions;
    }
}