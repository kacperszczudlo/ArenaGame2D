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
        // 1. START RUNDY I TICKI STATUSÓW (Twoja zasada "Efekty z poprzedniej rundy")
        player.ProcessStatuses();
        enemy.ProcessStatuses();

        // Czekamy chwilê, ¿eby gracz zobaczy³ cyferki z trucizny/podpalenia
        yield return new WaitForSeconds(0.8f);

        // 2. ZBIERAMY I SORTUJEMY AKCJE GRACZA
        List<SkillAPHandler> usedSkills = new List<SkillAPHandler>();
        foreach (SkillAPHandler slot in attackSlots)
        {
            if (slot.currentSkill != null && slot.currentSkill.data != null && slot.currentPA > 0)
                usedSkills.Add(slot);
        }

        // Sortowanie: Uroki -> Dystans -> Zwarcie
        usedSkills.Sort((a, b) => a.currentSkill.data.category.CompareTo(b.currentSkill.data.category));

        bool isAtMeleeRange = false;

        // 3. WYKONANIE AKCJI
        foreach (SkillAPHandler slot in usedSkills)
        {
            CharacterSkill charSkill = slot.currentSkill;
            SkillData skillData = charSkill.data;
            SkillLevelData levelData = skillData.GetLevelData(charSkill.currentLevel);

            player.ConsumeResources(levelData?.manaCost ?? 0, levelData?.staminaCost ?? 0);

            // Podbieganie
            if (skillData.category == SkillCategory.MeleePhysical && !isAtMeleeRange)
            {
                float distanceToStop = enemy.meleeStoppingDistance;
                Vector3 targetMeleePos = enemyOriginalPos + new Vector3(-distanceToStop, 0, 0);
                yield return StartCoroutine(MoveCharacter(player.transform, targetMeleePos, 0.3f));
                isAtMeleeRange = true;
            }

            player.PlayAttackAnimation(skillData.animTriggerName);
            yield return new WaitForSeconds(0.5f);

            // Obliczenia
            AttackResult result = DamageCalculator.ProcessAttack(player, enemy, charSkill, slot.currentPA, 0);

            if (result.isHit)
            {
                if (skillData.category == SkillCategory.PositiveCharm)
                {
                    // UROK POZYTYWNY: Efekt na Graczu
                    if (skillData.showCenterVFX) player.PlaySkillEffect(skillData.icon);
                    player.Heal(result.damageDealt, result.chanceText);
                }
                else
                {
                    // WSZYSTKO INNE: Efekt na Wrogu
                    if (skillData.showCenterVFX) enemy.PlaySkillEffect(skillData.icon);

                    if (skillData.category == SkillCategory.NegativeCharm && result.damageDealt <= 0)
                        enemy.ShowFloatingText(skillData.skillName, DamagePopup.PopupType.Buff, null, result.chanceText);
                    else
                        enemy.TakeDamage(result.damageDealt, result.isCritical, result.chanceText);
                }
            }
            else
            {
                Combatant target = (skillData.category == SkillCategory.PositiveCharm) ? player : enemy;
                target.ShowFloatingText("Pud³o", DamagePopup.PopupType.Miss, null, result.chanceText);
            }

            yield return new WaitForSeconds(1.0f);
        }

        // 4. POWRÓT I KONIEC RUNDY
        if (isAtMeleeRange)
        {
            yield return StartCoroutine(MoveCharacter(player.transform, playerOriginalPos, 0.4f));
        }

        currentRound++;
        UpdateRoundUI();
        yield return new WaitForSeconds(0.2f);
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