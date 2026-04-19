using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    [Header("Znaczniki (Miejsca na arenie)")]
    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;

    [Header("Panele UI (Do podpiêcia postaciom)")]
    public CharacterUI playerUI;
    public CharacterUI enemyUI;

    [Header("Kontenery Statusów (UI)")]
    public Transform playerStatusContainer;
    public Transform enemyStatusContainer;

    [Header("Aktorzy (Przypisz¹ siê sami!)")]
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
    public int currentRound = 1;
    public static BattleManager Instance;

    private Vector3 playerOriginalPos;
    private Vector3 enemyOriginalPos;

    public UnityEngine.UI.Button startRoundButton;
    private bool isExecutingRound = false;

    [Header("Ekran Podsumowania (Koniec Walki)")]
    public GameObject summaryPanel;
    public TMPro.TextMeshProUGUI summaryTitleText;
    public TMPro.TextMeshProUGUI summaryRewardsText;

    void Start()
    {
        currentRound = 1;
        UpdateRoundUI();

        SpawnCombatants();
    }
    void Awake()
    {
        // Przypisz instancjê przy starcie
        Instance = this;
    }

    void SpawnCombatants()
    {
        // 1. TWORZYMY GRACZA
        GameObject pPrefab = GameManager.Instance.currentPlayerPrefab;
        if (pPrefab != null && playerSpawnPoint != null)
        {
            GameObject pGo = Instantiate(pPrefab, playerSpawnPoint.position, Quaternion.identity);
            player = pGo.GetComponent<Combatant>();
            playerOriginalPos = playerSpawnPoint.position;

            // Podpinamy UI
            if (playerUI != null) player.myUI = playerUI;
            if (playerStatusContainer != null) player.statusIconsContainer = playerStatusContainer;

            // Upewniamy siê, ¿e za³adowa³ swoje statystyki z PlayerDataManagera
            player.LoadDataFromManager();
        }
        else
        {
            Debug.LogError("B³¹d! GameManager nie przekaza³ prefaba gracza!");
        }

        // 2. TWORZYMY WROGA (Pobieramy z GameManagera!)
        EnemyData enemyData = GameManager.Instance.currentEnemyToFight;
        if (enemyData != null && enemyData.enemyVisualPrefab != null && enemySpawnPoint != null)
        {
            // Klonujemy UNIKALNE cia³o wroga (zapisane w jego pliku)
            GameObject eGo = Instantiate(enemyData.enemyVisualPrefab, enemySpawnPoint.position, Quaternion.identity);
            enemy = eGo.GetComponent<Combatant>();
            enemyOriginalPos = enemySpawnPoint.position;

            // Podpinamy UI
            if (enemyUI != null) enemy.myUI = enemyUI;
            if (enemyStatusContainer != null) enemy.statusIconsContainer = enemyStatusContainer;

            // WSTRZYKUJEMY  (Statystyki, poziom, skille i Mózg AI)
            enemy.LoadEnemyData(enemyData);
        }
        else
        {
            Debug.LogError("B³¹d! GameManager nie przekaza³ wroga do walki, albo wróg nie ma przypisanego 'Enemy Visual Prefab'!");
        }
    }

    // Funkcja podpiêta pod przycisk "Rozpocznij rundê"
    public void OnStartRoundClicked()
    {
        // ZABEZPIECZENIE: Jeœli runda ju¿ trwa, zignoruj klikniêcie
        if (isExecutingRound) return;

        isExecutingRound = true; // Zaznaczamy, ¿e weszliœmy do walki

        // Blokujemy przycisk fizycznie, ¿eby zszarza³
        if (startRoundButton != null) startRoundButton.interactable = false;

        // Odpalamy korutynê walki
        StartCoroutine(ExecuteTurnRoutine());
    }

    void UpdateRoundUI()
    {
        if (roundText != null) roundText.text = "Runda: " + currentRound;
    }

    IEnumerator ExecuteTurnRoutine()
    {


        yield return StartCoroutine(player.ProcessStatusesRoutine());
        yield return StartCoroutine(enemy.ProcessStatusesRoutine());
        yield return new WaitForSeconds(0.4f);

        if (defenseMeleeUI != null) player.defenseMeleePA = defenseMeleeUI.currentPA;
        if (defenseRangedUI != null) player.defenseRangedPA = defenseRangedUI.currentPA;
        if (defenseMentalUI != null) player.defenseMentalPA = defenseMentalUI.currentPA;


        List<CombatAction> roundActions = new List<CombatAction>();
        int actionCounter = 0;

        // ZBIERANIE DECYZJI GRACZA
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

        // MÓZG WROGA
        if (enemy.currentHP > 0)
        {
            if (enemy.myBrain != null)
            {
                List<CombatAction> enemyActions = enemy.myBrain.DecideTurn(enemy, player, ref actionCounter);
                roundActions.AddRange(enemyActions);
            }
            else
            {
                Debug.LogWarning($"<color=yellow>UWAGA: {enemy.combatantName} nie ma podpiêtego Mózgu w pliku EnemyData!</color>");
            }
        }

        // SORTOWANIE 
        roundActions.Sort((a, b) => {
            int categoryComparison = a.skill.data.category.CompareTo(b.skill.data.category);
            if (categoryComparison != 0) return categoryComparison;

            if (a.actor.isPlayer != b.actor.isPlayer)
                return a.actor.isPlayer ? -1 : 1;

            return a.originalIndex.CompareTo(b.originalIndex);
        });

        // WYKONYWANIE AKCJI
        bool playerAtMelee = false;
        bool enemyAtMelee = false;

        for (int i = 0; i < roundActions.Count; i++)
        {
            var action = roundActions[i];

            if (action.actor.currentHP <= 0 || action.target.currentHP <= 0) break;

            StatusEffect freezeStatus = action.actor.activeStatuses.Find(s => s.type == StatusType.DeepFreeze);
            if (freezeStatus != null)
            {
                action.actor.ShowFloatingText("ZAMRO¯ONY!", DamagePopup.PopupType.Miss);
                yield return new WaitForSeconds(0.4f);
                continue;
            }

            SkillData skillData = action.skill.data;
            SkillLevelData levelData = skillData.GetLevelData(action.skill.currentLevel);

            int mCost = levelData?.manaCost ?? 0;
            int sCost = levelData?.staminaCost ?? 0;

            //TRUCIZNY: SPRAWDZAMY KARÊ PRZED ATAKIEM ---
            int poisonPenalty = 0;
            StatusEffect poison = action.actor.activeStatuses.Find(s => s.type == StatusType.Poison);
            if (poison != null)
            {
                poisonPenalty = poison.value; // Pobieramy karê z trucizny
            }

            //sprawdzamy, czy gracz ma zasoby na koszt bazowy + karê z trucizny
            bool hasResources = action.actor.currentMana >= (mCost + poisonPenalty) && action.actor.currentStamina >= (sCost + poisonPenalty);

            if (!hasResources)
            {
                action.actor.ShowFloatingText("Brak zasobów!", DamagePopup.PopupType.Miss);
                // Jeœli to gracz nie ma many, kó³ko w UI robi siê czerwone/szare - zablokowanie tury
                yield return new WaitForSeconds(0.8f);
            }
            else
            {
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

                //OBS£UGA STRZA£ I POCISKÓW
                // Sprawdzamy, czy skill ma przypisan¹ strza³ê (nie jest puste okienko w Inspektorze)
                if (skillData.projectilePrefab != null)
                {
                    //DELAY: Jeœli postaæ leczy/buffuje sam¹ siebie, nie czekamy
                    float prepDelay = (action.actor == action.target) ? 0.05f : 0.4f;
                    yield return new WaitForSeconds(prepDelay);



                    // Tworzymy strza³ê na scenie
                    GameObject projGo = Instantiate(skillData.projectilePrefab);
                    Projectile projScript = projGo.GetComponent<Projectile>();

                    if (projScript != null)
                    {
                        // Mened¿er PAUZUJE bitwê, dopóki strza³a nie doleci do klatki piersiowej wroga
                        float arrowSpeed = 25f; // Szybkoœæ strza³y
                        yield return StartCoroutine(projScript.FlyToTarget(action.actor.centerSpawnPoint.position, action.target.centerSpawnPoint.position, skillData.projectileColor, arrowSpeed));
                    }
                }
                else
                {
                    // Zwyk³y atak  - stara, sztywna pauza na animacjê
                    // FIX NA DELAY: B³yskawiczne efekty, gdy rzucamy buffa/leczenie na siebie!
                    float impactDelay = (action.actor == action.target) ? 0.05f : 0.5f;
                    yield return new WaitForSeconds(impactDelay);
                }

                // OBLICZANIE OBRA¯EÑ 
                AttackResult result = DamageCalculator.ProcessAttack(action.actor, action.target, action.skill, action.paInvested);

                if (result.isHit)
                {
                    Combatant mainTarget = (skillData.category == SkillCategory.PositiveCharm) ? action.actor : action.target;
                    if (skillData.showCenterVFX) mainTarget.PlaySkillEffect(skillData.icon);

                    if (skillData.category == SkillCategory.PositiveCharm)
                    {
                        action.actor.Heal(result.damageDealt, result.chanceText);
                    }
                    else
                    {
                        // G£ÓWNY CEL DOSTAJE OBRA¯ENIA
                        action.target.TakeDamage(result.damageDealt, result.isCritical, result.chanceText, false, skillData.category);

                        // TARCZA OGNIA: ODBICIE ---
                        // Sprawdzamy czy cel uderzenia mia³ na sobie Tarczê Ognia
                        StatusEffect fireShield = action.target.activeStatuses.Find(s => s.type == StatusType.FireShield);

                        // Jeœli ma tarczê i w ogóle dosta³ jakieœ obra¿enia
                        if (fireShield != null && result.damageDealt > 0)
                        {
                            // Liczymy odbicie
                            int reflectedDamage = Mathf.RoundToInt(result.damageDealt * fireShield.multiplier);
                            if (reflectedDamage > 0)
                            {
                                // ATAKUJ¥CY DOSTAJE RYKOSZETEM
                                action.actor.TakeDamage(reflectedDamage, false, "", true, SkillCategory.RangedMagic);
                                action.actor.ShowFloatingText($"Odbicie: {reflectedDamage}", DamagePopup.PopupType.CriticalDamage);
                            }
                        }
                    }

                    // ODPALANIE EFEKTÓW
                    float baseChance = levelData != null ? levelData.statusEffectChance : 100f;
                    float finalChance = (skillData.category == SkillCategory.PositiveCharm)
                        ? (100f * result.hitChanceMultiplier)
                        : baseChance;

                    foreach (SkillEffect effect in skillData.effects)
                    {
                        if (effect != null)
                        {
                            // Dziêki temu buffy trafiaj¹ na rzucaj¹cego, a kl¹twy na wroga.
                            effect.Execute(action.actor, mainTarget, result, finalChance, levelData, skillData.icon);
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

            //INTELIGENTNY POWRÓT NA MIEJSCE
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

        // CZYSZCZENIE RUNDY
        if (player.currentHP <= 0)
        {
            StartCoroutine(EndBattleRoutine(false)); // Gracz przegra³
            yield break; // Zatrzymujemy ca³kowicie tê korutynê
        }
        else if (enemy.currentHP <= 0)
        {
            StartCoroutine(EndBattleRoutine(true)); // Gracz wygra³
            yield break; // Zatrzymujemy ca³kowicie tê korutynê
        }
        player.ResetDefensePA();
        enemy.ResetDefensePA();
        // ZAPISYWANIE HP NA KONIEC RUNDY 
        player.hpAtRoundEnd = player.currentHP;
        enemy.hpAtRoundEnd = enemy.currentHP;

        // Odzyskiwanie zasobów na pocz¹tku rundy (5%)
        player.RegenerateResources();
        enemy.RegenerateResources();
        RefreshPlayerAP();

        currentRound++;
        UpdateRoundUI();

        isExecutingRound = false;
        if (startRoundButton != null) startRoundButton.interactable = true;
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



    //FUNKCJA ZAKOÑCZENIA BITWY
    IEnumerator EndBattleRoutine(bool playerWon)
    {
        yield return new WaitForSeconds(1.5f);

        EnemyData defeatedEnemy = GameManager.Instance.currentEnemyToFight;

        // W³¹czamy czarny ekran podsumowania
        if (summaryPanel != null) summaryPanel.SetActive(true);

        if (playerWon)
        {
            if (summaryTitleText != null) { summaryTitleText.text = "ZWYCIÊSTWO!"; summaryTitleText.color = Color.green; }

            if (GameManager.Instance.isTournamentBattle)
            {
                // ZASADY TURNIEJU: £upy id¹ do worka ("pending")
                int droppedGems = 0;

                if (defeatedEnemy != null)
                {
                    GameManager.Instance.pendingGold += defeatedEnemy.goldReward;

                    if (defeatedEnemy.level > PlayerDataManager.Instance.currentLevel)
                    {
                        GameManager.Instance.pendingXP += defeatedEnemy.expReward;
                    }

                    // Losowanie Gemów Turniejowych
                    if (defeatedEnemy.gemRewardAmount > 0 && Random.Range(0f, 100f) <= defeatedEnemy.gemRewardChance)
                    {
                        droppedGems = defeatedEnemy.gemRewardAmount;
                        GameManager.Instance.pendingGems += droppedGems;
                    }
                }
                GameManager.Instance.currentTournamentIndex++;

                int gainedExp = (defeatedEnemy != null && defeatedEnemy.level > PlayerDataManager.Instance.currentLevel) ? defeatedEnemy.expReward : 0;
                string gemsText = droppedGems > 0 ? $"\nGemy turniejowe: {droppedGems}" : ""; // Dopisek, jeœli wypad³y

                if (summaryRewardsText != null)
                    summaryRewardsText.text = $"Do puli nagród za walki w turnieju zdobywasz:\nZ³oto: {defeatedEnemy?.goldReward}\nDoœwiadczenie: {gainedExp}{gemsText}";
            }
            else
            {
                // ZASADY MAPY: £upy id¹ od razu do kieszeni! (TWARDY ZAPIS)
                if (defeatedEnemy != null)
                {
                    // U¿ywamy AddGold ¿eby natychmiast zapisaæ do pliku!
                    GameManager.Instance.AddGold(defeatedEnemy.goldReward);

                    // Opcjonalnie: Jeœli na mapie te¿ chcesz dawaæ graczowi XP za zwyk³e moby, zrób to tak:
                    // if (defeatedEnemy.level > PlayerDataManager.Instance.currentLevel)
                    // {
                    //     PlayerDataManager.Instance.AddExperience(defeatedEnemy.expReward);
                    //     PlayerDataManager.Instance.SavePlayerData();
                    // }
                }
                if (summaryRewardsText != null) summaryRewardsText.text = $"Zdobywasz:\nZ³oto: {defeatedEnemy?.goldReward}";
            }
        }
        else
        {
            if (summaryTitleText != null) { summaryTitleText.text = "PORA¯KA!"; summaryTitleText.color = Color.red; }

            if (GameManager.Instance.isTournamentBattle)
            {
                // ZASADY TURNIEJU: Wrzucamy to co mieliœmy w worku do g³ównej puli, zapisujemy i resetujemy!
                GameManager.Instance.AddGold(GameManager.Instance.pendingGold);
                GameManager.Instance.AddGems(GameManager.Instance.pendingGems);

                if (GameManager.Instance.pendingXP > 0)
                {
                    PlayerDataManager.Instance.AddExperience(GameManager.Instance.pendingXP);
                }

                // Dodajemy zgon i robimy twardy zapis wszystkiego!
                PlayerDataManager.Instance.deathCount++;
                PlayerDataManager.Instance.SavePlayerData();

                // Wyœwietlamy ocala³e przedmioty
                if (GameManager.Instance.pendingGold > 0 || GameManager.Instance.pendingXP > 0 || GameManager.Instance.pendingGems > 0)
                {
                    string gemsString = GameManager.Instance.pendingGems > 0 ? $"\nGemy turniejowe: {GameManager.Instance.pendingGems}" : "";
                    if (summaryRewardsText != null) summaryRewardsText.text = $"Z poprzednich walk uda³o Ci siê ocaliæ:\nZ³oto: {GameManager.Instance.pendingGold}\nDoœwiadczenie: {GameManager.Instance.pendingXP}{gemsString}";
                }
                else
                {
                    if (summaryRewardsText != null) summaryRewardsText.text = "Niestety, wracasz z pustymi rêkami...";
                }

                GameManager.Instance.pendingGold = 0;
                GameManager.Instance.pendingXP = 0;
                GameManager.Instance.pendingGems = 0;
                GameManager.Instance.currentTournamentIndex = 0;
            }
            else
            {
                // Zwyk³a œmieræ na mapie (te¿ zapisujemy zgon!)
                PlayerDataManager.Instance.deathCount++;
                PlayerDataManager.Instance.SavePlayerData();

                if (summaryRewardsText != null) summaryRewardsText.text = "Zgin¹³eœ na mapie! Zostajesz przeniesiony do bezpiecznego miejsca.";
            }

            GameManager.Instance.sceneToLoadAfterBattle = "MainScene";
        }


    }

    public void OnClick_LeaveBattle()
    {
        SceneManager.LoadScene(GameManager.Instance.sceneToLoadAfterBattle);
    }
    public void RefreshPlayerAP()
    {
        if (CombatAPManager.Instance == null) return;

        // 1. Obliczamy nowy, maksymalny limit PA (Bazowe - Kara z debuffów)
        int apPenalty = player.GetCombatAPReduction();
        int baseMaxAP = CombatAPManager.Instance.maxAP; // Pobieramy oryginalne 15
        int effectiveMaxAP = Mathf.Max(0, baseMaxAP - apPenalty);

        // 2. Liczymy, ile gracz ma obecnie naklikane w UI (Ataki + Obrony)
        int allocatedAP = 0;
        foreach (var slot in attackSlots) allocatedAP += slot.currentPA;
        if (defenseMeleeUI != null) allocatedAP += defenseMeleeUI.currentPA;
        if (defenseRangedUI != null) allocatedAP += defenseRangedUI.currentPA;
        if (defenseMentalUI != null) allocatedAP += defenseMentalUI.currentPA;

        // 3. Jeœli masz rozdane wiêcej ni¿ wynosi nowy limit to odejmujemy
        int toRemove = allocatedAP - effectiveMaxAP;
        if (toRemove > 0)
        {
            // A) Kradniemy ze skilli ataku (od prawego do lewego)
            for (int j = attackSlots.Count - 1; j >= 0; j--)
            {
                var slot = attackSlots[j];
                while (slot.currentPA > 0 && toRemove > 0)
                {
                    slot.currentPA--;
                    toRemove--;
                    slot.UpdateVisuals(); // Gasi kó³ko na ekranie
                }
                if (toRemove <= 0) break;
            }

            // B) Jeœli dalej trzeba ukraœæ (kó³ka ataku puste), kradniemy z obron (Umys³ -> Dystans -> Zwarcie)
            if (toRemove > 0 && defenseMentalUI != null) { int drain = Mathf.Min(defenseMentalUI.currentPA, toRemove); defenseMentalUI.currentPA -= drain; toRemove -= drain; defenseMentalUI.UpdateVisuals(); }
            if (toRemove > 0 && defenseRangedUI != null) { int drain = Mathf.Min(defenseRangedUI.currentPA, toRemove); defenseRangedUI.currentPA -= drain; toRemove -= drain; defenseRangedUI.UpdateVisuals(); }
            if (toRemove > 0 && defenseMeleeUI != null) { int drain = Mathf.Min(defenseMeleeUI.currentPA, toRemove); defenseMeleeUI.currentPA -= drain; toRemove -= drain; defenseMeleeUI.UpdateVisuals(); }

            allocatedAP = effectiveMaxAP;
        }

        // 4. Przypisujemy wolne punkty z powrotem do puli Gracza 
        CombatAPManager.Instance.currentAP = effectiveMaxAP - allocatedAP;

        // 5. Aktualizujemy tekst na œrodku ekranu
        CombatAPManager.Instance.UpdateAPText(effectiveMaxAP);
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