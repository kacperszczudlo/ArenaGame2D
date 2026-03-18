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

        // King to prawdziwy arsena³ - potrzebuje 8 umiejêtnoœci!
        if (me.mySkills.Count < 8)
        {
            Debug.LogError($"<color=red>UWAGA: {me.combatantName} potrzebuje 8 skilli (0: Cieñ, 1: Magiczny Pancerz W Dó³, 2: Zatrucie, 3: Œlepota/Celnoœæ, 4: Atak Fizyczny, 5: Atak Psychiczny, 6: NAG£A ŒMIERÆ, 7: Krwawienie z Uroku)!</color>");
            return actions;
        }

        // Przypisanie dla wygody i czytelnoœci
        CharacterSkill shadowAttack = me.mySkills[0];
        CharacterSkill magicArmorDown = me.mySkills[1];
        CharacterSkill poisonAttack = me.mySkills[2];
        CharacterSkill accuracyDown = me.mySkills[3];
        CharacterSkill meleeAttack = me.mySkills[4];
        CharacterSkill mentalAttack = me.mySkills[5];
        CharacterSkill suddenDeath = me.mySkills[6];
        CharacterSkill charmBleed = me.mySkills[7];

        int round = BattleManager.Instance.currentRound;

        // ==========================================
        // RUNDA 1: LOSOWANIE TAKTYKI NA CA£¥ WALKÊ!
        // ==========================================
        if (round == 1)
        {
            // Opcja dla Ciebie do testowania z poziomu Inspektora:
            if (forceTactic != -1)
            {
                chosenTactic = forceTactic;
                Debug.Log($"<color=cyan><b>[TRYB TESTOWY]</b> Wymuszono Taktykê nr: {chosenTactic}</color>");
            }
            else
            {
                // Zwyk³a, losowa gra na arenie:
                int roll = Random.Range(1, 101);

                if (roll <= 10)
                {
                    chosenTactic = 0; // 10% Szans - TRYB ZABÓJCY (Nag³a Œmieræ)
                    Debug.Log($"<color=darkred>{me.combatantName}: Oczy mu b³yszcz¹ na czerwono... Wylosowa³ Tryb Egzekucji!</color>");
                }
                else if (roll <= 40)
                {
                    chosenTactic = 1; // 30% Szans - TRYB WOJOWNIKA (Zwarcie + Cieñ)
                    Debug.Log($"<color=gray>{me.combatantName}: Wyci¹ga ostrze. Tryb brutalnej si³y.</color>");
                }
                else if (roll <= 70)
                {
                    chosenTactic = 2; // 30% Szans - TRYB W£ADCY UMYS£ÓW (Psychika + Debuffy)
                    Debug.Log($"<color=magenta>{me.combatantName}: Zaczyna szeptaæ zaklêcia... Tryb Psychiczny.</color>");
                }
                else
                {
                    chosenTactic = 3;
                    Debug.Log($"<color=green>{me.combatantName}: Zatrute sztylety w d³oniach. Tryb DoT.</color>");
                }
            }
        }

        // ==========================================
        // WYKONYWANIE WYLOSOWANEJ TAKTYKI
        // ==========================================

        // ------------------------------------------------
        // TAKTYKA 0: EGZEKUCJA (All-in w Rundzie 1)
        // ------------------------------------------------
        if (chosenTactic == 0)
        {
            if (round == 1)
            {
                // Rundy przygotowawcze: Spamuje wszystkim co brudne, by upewniæ siê, ¿e gracz ma wymagane debuffy!
                actions.Add(new CombatAction { actor = me, target = player, skill = magicArmorDown, paInvested = 3, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = magicArmorDown, paInvested = 3, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = magicArmorDown, paInvested = 3, originalIndex = actionCounter++ });

                actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 3, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 3, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 3, originalIndex = actionCounter++ });

                actions.Add(new CombatAction { actor = me, target = player, skill = accuracyDown, paInvested = 3, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = accuracyDown, paInvested = 3, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = accuracyDown, paInvested = 3, originalIndex = actionCounter++ });

                // Próba natychmiastowego mordu!
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

                // King ca³kowicie ods³ania gardê rzucaj¹c wszystko na jedn¹ szalê
                me.defenseMeleePA = 0; me.defenseRangedPA = 0; me.defenseMentalPA = 0;
            }
            else
            {
                // RUNDY 2+: SPRAWDZANIE STATUSÓW GRACZA
                // UWAGA: Upewnij siê, ¿e te typy statusów (StatusType) idealnie pasuj¹ do Twoich enumów!
                bool hasPoison = player.activeStatuses.Exists(s => s.type == StatusType.Poison);
                bool hasMagicArmorDown = player.activeStatuses.Exists(s => s.type == StatusType.VoodooCurse); // Zmieñ na swój typ obni¿enia defa
                bool hasAccuracyDown = player.activeStatuses.Exists(s => s.type == StatusType.Blindness); // Zmieñ na swój typ utraty celnoœci

                if (hasPoison && hasMagicArmorDown && hasAccuracyDown)
                {
                    // Debuffy wci¹¿ s¹ aktywne! Rzuca mniejsz¹ porcjê Nag³ej Œmierci i normalne ataki.
                    Debug.Log($"<color=darkred>{me.combatantName}: Gracz wci¹¿ posiada 3 wymagane debuffy! U¿ywa Nag³ej Œmierci!</color>");

                    actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                    

                    me.defenseMeleePA = 0; me.defenseRangedPA = 0; me.defenseMentalPA = 0;
                }
                else
                {
                    // Debuffy wygas³y, King improwizuje! Losuje jedn¹ z 3 potê¿nych rotacji na tê rundê.
                    int subTactic = Random.Range(0, 3); // Zwróci 0, 1 albo 2

                    if (subTactic == 0)
                    {
                        Debug.Log($"<color=gray>{me.combatantName}: Brak debuffów. Przechodzi do potê¿nej furii fizycznej!</color>");
                        // Potê¿ne ciosy wrêcz i próba ponownego oœlepienia
                        actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 3, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 4, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 3, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = accuracyDown, paInvested = 3, originalIndex = actionCounter++ });

                        me.defenseMeleePA = 2; me.defenseRangedPA = 2; me.defenseMentalPA = 2;
                    }
                    else if (subTactic == 1)
                    {
                        Debug.Log($"<color=magenta>{me.combatantName}: Brak debuffów. Otwiera umys³ gracza magi¹!</color>");
                        // Ataki psychiczne i próba zniszczenia pancerza magicznego
                        actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 1, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 3, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 1, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = magicArmorDown, paInvested = 3, originalIndex = actionCounter++ });

                        me.defenseMeleePA = 2; me.defenseRangedPA = 2; me.defenseMentalPA = 2;
                    }
                    else
                    {
                        Debug.Log($"<color=green>{me.combatantName}: Brak debuffów. Znika w cieniach i truje!</color>");
                        // Mieszanka cienia, trucizny i krwawienia
                        actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 3, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 3, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 2, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = charmBleed, paInvested = 5, originalIndex = actionCounter++ });

                        // Bardzo trudny do trafienia dystansowo (bo jest "w cieniu")
                        me.defenseMeleePA = 2; me.defenseRangedPA = 2; me.defenseMentalPA = 2;
                    }
                }
            }
        }
        // ------------------------------------------------
        // TAKTYKA 1: BRUTALNA SI£A (Wojownik czujny na egzekucjê)
        // ------------------------------------------------
        else if (chosenTactic == 1)
        {
            // 1. UNIVERSALNE SPRAWDZENIE EGZEKUCJI (Dzia³a w ka¿dej rundzie!)
            bool hasPoison = player.activeStatuses.Exists(s => s.type == StatusType.Poison);
            bool hasMagicArmorDown = player.activeStatuses.Exists(s => s.type == StatusType.VoodooCurse);
            bool hasAccuracyDown = player.activeStatuses.Exists(s => s.type == StatusType.Blindness);

            if (hasPoison && hasMagicArmorDown && hasAccuracyDown)
            {
                // Gracz ma pecha i zebra³ 3 debuffy! Wojownik porzuca taktykê i œcina g³owê!
                Debug.Log($"<color=darkred>{me.combatantName}: Gwiazdy siê u³o¿y³y! King zauwa¿a s³aboœæ i odpala Nag³¹ Œmieræ!</color>");

                actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = suddenDeath, paInvested = 6, originalIndex = actionCounter++ });
                actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 5, originalIndex = actionCounter++ });

                // Opuszcza gardê
                me.defenseMeleePA = 0; me.defenseRangedPA = 0; me.defenseMentalPA = 0;
            }
            else
            {
                // 2. NORMALNA WALKA WOJOWNIKA
                if (round == 1)
                {
                    // RUNDA 1: Oœlepienie i grad ciosów
                    Debug.Log($"<color=gray>{me.combatantName} (Wojownik): Oœlepia gracza i szar¿uje z mieczem!</color>");

                    actions.Add(new CombatAction { actor = me, target = player, skill = accuracyDown, paInvested = 4, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = accuracyDown, paInvested = 4, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 6, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 5, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 5, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 5, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 5, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 5, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 5, originalIndex = actionCounter++ });

                    me.defenseMeleePA = 1; me.defenseRangedPA = 1; me.defenseMentalPA = 1; 
                }
                else
                {
                    // RUNDY 2+: Losowe komba z debuffami
                    int subTactic = Random.Range(0, 3);

                    if (subTactic == 0)
                    {
                        Debug.Log($"<color=gray>{me.combatantName} (Wojownik): Uderza z cienia, truje i poprawia mieczem!</color>");
                        actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 4, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 3, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 5, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 5, originalIndex = actionCounter++ });

                        me.defenseMeleePA = 3; me.defenseRangedPA = 2; me.defenseMentalPA = 2;
                    }
                    else if (subTactic == 1)
                    {
                        Debug.Log($"<color=gray>{me.combatantName} (Wojownik): Próbuje oœlepiæ i wyprowadza mordercze ciosy!</color>");
                        actions.Add(new CombatAction { actor = me, target = player, skill = accuracyDown, paInvested = 4, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 6, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 6, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = charmBleed, paInvested = 3, originalIndex = actionCounter++ });

                        me.defenseMeleePA = 2; me.defenseRangedPA = 2; me.defenseMentalPA = 2;
                    }
                    else
                    {
                        Debug.Log($"<color=gray>{me.combatantName} (Wojownik): Kruszy pancerz magiczny i mia¿d¿y fizycznie!</color>");
                        actions.Add(new CombatAction { actor = me, target = player, skill = magicArmorDown, paInvested = 6, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 5, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 5, originalIndex = actionCounter++ });
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
                Debug.Log($"<color=darkred>{me.combatantName}: Gwiazdy siê u³o¿y³y! King zauwa¿a s³aboœæ i odpala Nag³¹ Œmieræ!</color>");
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
                    // RUNDA 1: Zdejmuje pancerz magiczny i wypala umys³!
                    Debug.Log($"<color=magenta>{me.combatantName} (W³adca): £amie magiczn¹ barierê i wdziera siê do umys³u!</color>");

                    actions.Add(new CombatAction { actor = me, target = player, skill = magicArmorDown, paInvested = 4, originalIndex = actionCounter++ });
                    
                    
                    actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 5, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 5, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 5, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 5, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 5, originalIndex = actionCounter++ });

                    // Os³oniêty przed magi¹, ignoruje uderzenia fizyczne
                    me.defenseMeleePA = 1; me.defenseRangedPA = 1; me.defenseMentalPA = 1;
                }
                else
                {
                    // RUNDY 2+: Losowe komba umys³owe z wtr¹ceniami
                    int subTactic = Random.Range(0, 3);

                    if (subTactic == 0)
                    {
                        Debug.Log($"<color=magenta>{me.combatantName} (W³adca): Szepty cienia i magia krwi!</color>");
                        actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 4, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = charmBleed, paInvested = 4, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 1, originalIndex = actionCounter++ });
                        

                        me.defenseMeleePA = 3; me.defenseRangedPA = 3; me.defenseMentalPA = 3;
                    }
                    else if (subTactic == 1)
                    {
                        Debug.Log($"<color=magenta>{me.combatantName} (W³adca): Truje umys³ i kruszy bariery!</color>");
                        actions.Add(new CombatAction { actor = me, target = player, skill = magicArmorDown, paInvested = 4, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 4, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 6, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 3, originalIndex = actionCounter++ });

                        me.defenseMeleePA = 2; me.defenseRangedPA = 2; me.defenseMentalPA = 2;
                    }
                    else
                    {
                        Debug.Log($"<color=magenta>{me.combatantName} (W³adca): Oœlepia iluzj¹, po czym atakuje umys³!</color>");
                        actions.Add(new CombatAction { actor = me, target = player, skill = accuracyDown, paInvested = 4, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 6, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 5, originalIndex = actionCounter++ });
                        

                        me.defenseMeleePA = 1; me.defenseRangedPA = 1; me.defenseMentalPA = 1;
                    }
                }
            }
        }
        // ------------------------------------------------
        // TAKTYKA 3: TRUCICIEL (DoT / Wyniszczenie)
        // ------------------------------------------------
        else if (chosenTactic == 3)
        {
            // 1. UNIVERSALNE SPRAWDZENIE EGZEKUCJI
            bool hasPoison = player.activeStatuses.Exists(s => s.type == StatusType.Poison);
            bool hasMagicArmorDown = player.activeStatuses.Exists(s => s.type == StatusType.VoodooCurse);
            bool hasAccuracyDown = player.activeStatuses.Exists(s => s.type == StatusType.Blindness);

            if (hasPoison && hasMagicArmorDown && hasAccuracyDown)
            {
                Debug.Log($"<color=darkred>{me.combatantName}: Gwiazdy siê u³o¿y³y! King zauwa¿a s³aboœæ i odpala Nag³¹ Œmieræ!</color>");

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
                    actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 5, originalIndex = actionCounter++ });
                    actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 5, originalIndex = actionCounter++ });

                    // Ukryty w cieniu, trudny do trafienia z ³uku/magii dystansowej
                    me.defenseMeleePA = 1; me.defenseRangedPA = 1; me.defenseMentalPA = 1;
                }
                else
                {
                    // RUNDY 2+: Losowe komba dystansowe
                    int subTactic = Random.Range(0, 3);

                    if (subTactic == 0)
                    {
                        Debug.Log($"<color=green>{me.combatantName} (Cieñ): Oœlepia z ukrycia i rzuca sztyletami!</color>");
                        actions.Add(new CombatAction { actor = me, target = player, skill = accuracyDown, paInvested = 4, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 5, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 5, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = meleeAttack, paInvested = 3, originalIndex = actionCounter++ });

                        me.defenseMeleePA = 2; me.defenseRangedPA = 2; me.defenseMentalPA = 2;
                    }
                    else if (subTactic == 1)
                    {
                        Debug.Log($"<color=green>{me.combatantName} (Cieñ): Kl¹twa na pancerz i morderczy deszcz cieni!</color>");
                        actions.Add(new CombatAction { actor = me, target = player, skill = magicArmorDown, paInvested = 4, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 6, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = shadowAttack, paInvested = 5, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = mentalAttack, paInvested = 3, originalIndex = actionCounter++ });

                        me.defenseMeleePA = 1; me.defenseRangedPA = 1; me.defenseMentalPA = 1;
                    }
                    else
                    {
                        Debug.Log($"<color=green>{me.combatantName} (Cieñ): Rozprowadza krwawienie i truciznê!</color>");
                        actions.Add(new CombatAction { actor = me, target = player, skill = charmBleed, paInvested = 4, originalIndex = actionCounter++ });
                        actions.Add(new CombatAction { actor = me, target = player, skill = poisonAttack, paInvested = 4, originalIndex = actionCounter++ });
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