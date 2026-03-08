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
        player.ProcessStatuses();
        enemy.ProcessStatuses();
        yield return new WaitForSeconds(0.8f);

        List<SkillAPHandler> usedSkills = new List<SkillAPHandler>();
        foreach (var slot in attackSlots)
            if (slot.currentSkill?.data != null && slot.currentPA > 0) usedSkills.Add(slot);

        usedSkills.Sort((a, b) => a.currentSkill.data.category.CompareTo(b.currentSkill.data.category));

        bool isAtMeleeRange = false;

        foreach (var slot in usedSkills)
        {
            SkillData skillData = slot.currentSkill.data;
            SkillLevelData levelData = skillData.GetLevelData(slot.currentSkill.currentLevel);
            player.ConsumeResources(levelData?.manaCost ?? 0, levelData?.staminaCost ?? 0);

            if (skillData.category == SkillCategory.MeleePhysical && !isAtMeleeRange)
            {
                yield return StartCoroutine(MoveCharacter(player.transform, enemyOriginalPos + new Vector3(-enemy.meleeStoppingDistance, 0, 0), 0.3f));
                isAtMeleeRange = true;
            }

            player.PlayAttackAnimation(skillData.animTriggerName);
            yield return new WaitForSeconds(0.5f);

            AttackResult result = DamageCalculator.ProcessAttack(player, enemy, slot.currentSkill, slot.currentPA, 0);

            if (result.isHit)
            {
                // Ustalamy g³ówny cel (Wróg dla ataku, Gracz dla buffa)
                Combatant mainTarget = (skillData.category == SkillCategory.PositiveCharm) ? player : enemy;

                // 1. Wizualizacja Center VFX (Kó³ka)
                if (skillData.showCenterVFX)
                {
                    mainTarget.PlaySkillEffect(skillData.icon);
                }

                // 2. Obra¿enia / Leczenie
                if (skillData.category == SkillCategory.PositiveCharm)
                    player.Heal(result.damageDealt, result.chanceText);
                else
                    enemy.TakeDamage(result.damageDealt, result.isCritical, result.chanceText);

                // 3. Odpalanie modularnych efektów (Z przekazaniem ikonki!)
                SkillLevelData currentLevelData = skillData.GetLevelData(slot.currentSkill.currentLevel);

                // Pobieramy Twoje np. 9% z tabelki (jeœli nie znajdzie, daje 100)
                float baseChance = currentLevelData != null ? currentLevelData.statusEffectChance : 100f;
                float finalChance = 0f;

                if (skillData.category == SkillCategory.PositiveCharm)
                {
                    // LOGIKA DLA BUFFÓW (Tarcza)
                    // Trudnoœæ zale¿na od wpakowanych PA (np. 120 trudnoœci vs 6 PA)
                    finalChance = 100f * result.hitChanceMultiplier;
                }
                else
                {
                    // LOGIKA DLA ATAKÓW (Magiczny Cios + Krwawienie)
                    // Bierzemy szansê z tabeli poziomu (Twoje 9%). 
                    // Mno¿ymy przez hitChanceMultiplier, ¿eby mniejsza iloœæ PA zmniejsza³a te¿ szansê na efekt.
                    //finalChance = baseChance * result.hitChanceMultiplier;
                    finalChance = baseChance;
                }

                foreach (SkillEffect effect in skillData.effects)
                {
                    if (effect != null)
                    {
                        // Przekazujemy odpowiedni¹ szansê!
                        effect.Execute(player, enemy, result, finalChance, skillData.icon);
                    }
                }
            }
            else
            {
                Combatant target = (skillData.category == SkillCategory.PositiveCharm) ? player : enemy;
                target.ShowFloatingText("Pud³o", DamagePopup.PopupType.Miss, null, result.chanceText);
            }
            yield return new WaitForSeconds(1.0f);
        }

        if (isAtMeleeRange) yield return StartCoroutine(MoveCharacter(player.transform, playerOriginalPos, 0.4f));
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