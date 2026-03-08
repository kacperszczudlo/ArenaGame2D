using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    [Header("Aktorzy")]
    public Combatant player;
    public Combatant enemy;

    [Header("UI Gracza")]
    public List<SkillAPHandler> attackSlots;

    [Header("UI Obrony Gracza")]
    public DefenseAPHandler defenseMeleeUI;
    public DefenseAPHandler defenseRangedUI;
    public DefenseAPHandler defenseMentalUI;

    [Header("UI Walki")]
    public TMPro.TextMeshProUGUI roundText;
    private int currentRound = 1;

    private Vector3 playerOriginalPos;
    private Vector3 enemyOriginalPos;

    void Start()
    {
        currentRound = 1;
        UpdateRoundUI();
        if (player != null) playerOriginalPos = player.transform.position;
        if (enemy != null) enemyOriginalPos = enemy.transform.position;
    }

    public void TestEndTurn()
    {
        // Teraz ca³a logika rundy siedzi w Routine
        StartCoroutine(ExecuteTurnRoutine());
    }

    void UpdateRoundUI()
    {
        if (roundText != null) roundText.text = "Runda: " + currentRound;
    }

    IEnumerator ExecuteTurnRoutine()
    {
        // Odzyskiwanie zasobów na pocz¹tku rundy (5%)
        player.RegenerateResources();
        enemy.RegenerateResources();

        player.ProcessStatuses();
        enemy.ProcessStatuses();
        yield return new WaitForSeconds(0.8f);

        if (defenseMeleeUI != null) player.defenseMeleePA = defenseMeleeUI.currentPA;
        if (defenseRangedUI != null) player.defenseRangedPA = defenseRangedUI.currentPA;
        if (defenseMentalUI != null) player.defenseMentalPA = defenseMentalUI.currentPA;
        Debug.Log($"<color=green>Gracz pobra³ z UI obronê: Zwarcie {player.defenseMeleePA}, Dystans {player.defenseRangedPA}, Umys³ {player.defenseMentalPA}</color>");
        // ----------------------------------------------------

        List<CombatAction> roundActions = new List<CombatAction>();
        int actionCounter = 0;

        

        // 1. ZBIERANIE DECYZJI GRACZA
        foreach (var slot in attackSlots)
        {
            if (slot.currentSkill?.data != null && slot.currentPA > 0)
            {
                roundActions.Add(new CombatAction
                {
                    actor = player,
                    target = enemy,
                    skill = slot.currentSkill,
                    paInvested = slot.currentPA,
                    originalIndex = actionCounter++ // Nadajemy numerek
                });
            }
        }

        // 2. MÓZG WROGA (3 TAKTYKI JANUSZA)
        if (enemy.currentHP > 0)
        {
            enemy.ResetDefensePA();
            int tacticRoll = Random.Range(1, 4);
            Debug.Log($"<color=orange>Wróg wybiera Taktykê nr: {tacticRoll}</color>");

            if (tacticRoll == 1)
            {
                if (enemy.mySkills.Count >= 2)
                {
                    roundActions.Add(new CombatAction { actor = enemy, target = player, skill = enemy.mySkills[0], paInvested = 3, originalIndex = actionCounter++ });
                    roundActions.Add(new CombatAction { actor = enemy, target = player, skill = enemy.mySkills[1], paInvested = 3, originalIndex = actionCounter++ });
                }
                enemy.defenseMeleePA = 5; enemy.defenseRangedPA = 0; enemy.defenseMentalPA = 0;
            }
            else if (tacticRoll == 2)
            {
                if (enemy.mySkills.Count >= 3)
                    roundActions.Add(new CombatAction { actor = enemy, target = player, skill = enemy.mySkills[2], paInvested = 2, originalIndex = actionCounter++ });

                enemy.defenseMeleePA = 3; enemy.defenseRangedPA = 0; enemy.defenseMentalPA = 1;
            }
            else
            {
                if (enemy.mySkills.Count >= 2)
                    roundActions.Add(new CombatAction { actor = enemy, target = player, skill = enemy.mySkills[1], paInvested = 4, originalIndex = actionCounter++ });

                enemy.defenseMeleePA = 5; enemy.defenseRangedPA = 1; enemy.defenseMentalPA = 0;
            }
            Debug.Log($"<color=red>Wróg ustawi³ obronê: Zwarcie {enemy.defenseMeleePA}, Dystans {enemy.defenseRangedPA}, Umys³ {enemy.defenseMentalPA}</color>");
        }

        // 3. SORTOWANIE (Teraz z poszanowaniem kolejki gracza!)
        roundActions.Sort((a, b) => {
            int categoryComparison = a.skill.data.category.CompareTo(b.skill.data.category);
            if (categoryComparison != 0) return categoryComparison;

            if (a.actor.isPlayer != b.actor.isPlayer)
                return a.actor.isPlayer ? -1 : 1;

            // NOWOŒÆ: Jeœli to ta sama kategoria ataków (np. 5 uderzeñ mieczem pod rz¹d), 
            // sortuj po oryginalnym indeksie z Twoich slotów!
            return a.originalIndex.CompareTo(b.originalIndex);
        });

        // 4. WYKONYWANIE AKCJI
        bool playerAtMelee = false;
        bool enemyAtMelee = false;

        // Zmieniamy foreach na pêtlê for, ¿eby móc podgl¹daæ nastêpn¹ akcjê!
        // Zmieniamy foreach na pêtlê for, ¿eby móc podgl¹daæ nastêpn¹ akcjê!
        for (int i = 0; i < roundActions.Count; i++)
        {
            var action = roundActions[i];

            if (action.actor.currentHP <= 0 || action.target.currentHP <= 0) break;

            SkillData skillData = action.skill.data;
            SkillLevelData levelData = skillData.GetLevelData(action.skill.currentLevel);

            int mCost = levelData?.manaCost ?? 0;
            int sCost = levelData?.staminaCost ?? 0;

            // Zmienna sprawdzaj¹ca, czy mamy na to hajs
            bool hasResources = action.actor.currentMana >= mCost && action.actor.currentStamina >= sCost;

            if (!hasResources)
            {
                // BRAK ZASOBÓW: Wyœwietlamy napis i czekamy chwilê (nie u¿ywamy ju¿ 'continue!')
                action.actor.ShowFloatingText("Brak zasobów!", DamagePopup.PopupType.Miss);
                yield return new WaitForSeconds(0.8f);
            }
            else
            {
                // S¥ ZASOBY: Odpalamy pe³n¹ sekwencjê ataku
                action.actor.ConsumeResources(mCost, sCost);

                // --- INTELIGENTNE PODBIEGANIE ---
                if (skillData.category == SkillCategory.MeleePhysical)
                {
                    if (action.actor.isPlayer && !playerAtMelee)
                    {
                        float dir = (action.target.transform.position.x > action.actor.transform.position.x) ? -1f : 1f;
                        Vector3 targetMeleePos = action.target.transform.position + new Vector3(action.target.meleeStoppingDistance * dir, 0, 0);
                        yield return StartCoroutine(MoveCharacter(action.actor.transform, targetMeleePos, 0.3f));
                        playerAtMelee = true;
                    }
                    else if (!action.actor.isPlayer && !enemyAtMelee)
                    {
                        float dir = (action.target.transform.position.x > action.actor.transform.position.x) ? -1f : 1f;
                        Vector3 targetMeleePos = action.target.transform.position + new Vector3(action.target.meleeStoppingDistance * dir, 0, 0);
                        yield return StartCoroutine(MoveCharacter(action.actor.transform, targetMeleePos, 0.3f));
                        enemyAtMelee = true;
                    }
                }

                // ATAK
                action.actor.PlayAttackAnimation(skillData.animTriggerName);
                yield return new WaitForSeconds(0.5f);

                AttackResult result = DamageCalculator.ProcessAttack(action.actor, action.target, action.skill, action.paInvested);

                if (result.isHit)
                {
                    Combatant mainTarget = (skillData.category == SkillCategory.PositiveCharm) ? action.actor : action.target;
                    if (skillData.showCenterVFX) mainTarget.PlaySkillEffect(skillData.icon);

                    if (skillData.category == SkillCategory.PositiveCharm)
                        action.actor.Heal(result.damageDealt, result.chanceText);
                    else
                        action.target.TakeDamage(result.damageDealt, result.isCritical, result.chanceText, false, skillData.category);

                    float baseChance = levelData != null ? levelData.statusEffectChance : 100f;

                    // NAPRAWA: Jeœli to normalny atak (nie urok na siebie), bierzemy CZYST¥ szansê z Inspektora!
                    float finalChance = (skillData.category == SkillCategory.PositiveCharm)
                        ? (100f * result.hitChanceMultiplier)
                        : baseChance;

                    foreach (SkillEffect effect in skillData.effects)
                    {
                        if (effect != null)
                        {
                            // --- ZMIANA: Dodaliœmy 'levelData' do wysy³anych informacji! ---
                            effect.Execute(action.actor, action.target, result, finalChance, levelData, skillData.icon);
                        }
                    }
                }
                else
                {
                    Combatant missedTarget = (skillData.category == SkillCategory.PositiveCharm) ? action.actor : action.target;
                    missedTarget.ShowFloatingText("Pud³o", DamagePopup.PopupType.Miss, null, result.chanceText);
                }

                yield return new WaitForSeconds(0.6f);
            }

            // --- INTELIGENTNY POWRÓT NA MIEJSCE ---
            // (Teraz jest POZA blokiem if-else, wiêc zawsze siê wykona, odsy³aj¹c goœcia do bazy!)
            if (skillData.category == SkillCategory.MeleePhysical)
            {
                bool shouldReturn = false;

                if (i == roundActions.Count - 1)
                {
                    shouldReturn = true;
                }
                else
                {
                    var nextAction = roundActions[i + 1];
                    if (nextAction.actor != action.actor || nextAction.skill.data.category != SkillCategory.MeleePhysical)
                    {
                        shouldReturn = true;
                    }
                }

                if (shouldReturn)
                {
                    // Upewniamy siê, ¿e wraca TYLKO wtedy, kiedy faktycznie jest na œrodku (flaga == true)
                    if (action.actor.isPlayer && playerAtMelee)
                    {
                        yield return StartCoroutine(MoveCharacter(player.transform, playerOriginalPos, 0.4f));
                        playerAtMelee = false;
                    }
                    else if (!action.actor.isPlayer && enemyAtMelee)
                    {
                        yield return StartCoroutine(MoveCharacter(enemy.transform, enemyOriginalPos, 0.4f));
                        enemyAtMelee = false;
                    }
                }
            }
        }

        // 5. CZYSZCZENIE RUNDY
        player.ResetDefensePA();
        enemy.ResetDefensePA();

        // Zwróæ uwagê, ¿e nie ma tu ju¿ ClearAll() dla UI, tak jak ustaliliœmy!

        currentRound++;
        UpdateRoundUI();
    }

        IEnumerator MoveCharacter(Transform character, Vector3 targetPos, float duration)
    {
        Vector3 startPos = character.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            character.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        character.position = targetPos;
    }

    
}

public class CombatAction
{
    public Combatant actor;
    public Combatant target;
    public CharacterSkill skill;
    public int paInvested;
    public int originalIndex;
}