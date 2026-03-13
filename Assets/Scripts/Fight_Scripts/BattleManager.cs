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
    private int currentRound = 1;

    private Vector3 playerOriginalPos;
    private Vector3 enemyOriginalPos;

    void Start()
    {
        currentRound = 1;
        UpdateRoundUI();

        // --- NOWOŒÆ: Wywo³ujemy zrzucenie postaci na ring! ---
        SpawnCombatants();
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

        // 2. TWORZYMY WROGA (Pobieramy duszê i cia³o z GameManagera!)
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

            // WSTRZYKUJEMY DUSZÊ! (Statystyki, poziom, skille i Mózg AI)
            enemy.LoadEnemyData(enemyData);
        }
        else
        {
            Debug.LogError("B³¹d! GameManager nie przekaza³ wroga do walki, albo wróg nie ma przypisanego 'Enemy Visual Prefab'!");
        }
    }

    public void TestEndTurn()
    {
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

        // 2. MÓZG WROGA (Teraz u¿ywamy wpiêtego modu³u AI!)
        if (enemy.currentHP > 0)
        {
            if (enemy.myBrain != null)
            {
                // Mened¿er po prostu mówi: "Masz tu dane, pomyœl sam i daj mi listê ataków!"
                List<CombatAction> enemyActions = enemy.myBrain.DecideTurn(enemy, player, ref actionCounter);
                roundActions.AddRange(enemyActions);
            }
            else
            {
                Debug.LogWarning($"<color=yellow>UWAGA: {enemy.combatantName} nie ma podpiêtego Mózgu (AI Brain) w pliku EnemyData! Stoi i gapi siê w niebo.</color>");
            }
        }

        // 3. SORTOWANIE (Teraz z poszanowaniem kolejki gracza!)
        roundActions.Sort((a, b) => {
            int categoryComparison = a.skill.data.category.CompareTo(b.skill.data.category);
            if (categoryComparison != 0) return categoryComparison;

            if (a.actor.isPlayer != b.actor.isPlayer)
                return a.actor.isPlayer ? -1 : 1;

            return a.originalIndex.CompareTo(b.originalIndex);
        });

        // 4. WYKONYWANIE AKCJI
        bool playerAtMelee = false;
        bool enemyAtMelee = false;

        for (int i = 0; i < roundActions.Count; i++)
        {
            var action = roundActions[i];

            if (action.actor.currentHP <= 0 || action.target.currentHP <= 0) break;

            SkillData skillData = action.skill.data;
            SkillLevelData levelData = skillData.GetLevelData(action.skill.currentLevel);

            int mCost = levelData?.manaCost ?? 0;
            int sCost = levelData?.staminaCost ?? 0;

            bool hasResources = action.actor.currentMana >= mCost && action.actor.currentStamina >= sCost;

            if (!hasResources)
            {
                action.actor.ShowFloatingText("Brak zasobów!", DamagePopup.PopupType.Miss);
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
                yield return new WaitForSeconds(0.5f); // <--- ZWIÊKSZ TO, JEŒLI POTRZEBUJESZ D£U¯SZEJ PAUZY NA ANIMACJE

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
                    float finalChance = (skillData.category == SkillCategory.PositiveCharm)
                        ? (100f * result.hitChanceMultiplier)
                        : baseChance;

                    foreach (SkillEffect effect in skillData.effects)
                    {
                        if (effect != null)
                        {
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

        // 5. CZYSZCZENIE RUNDY
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

        // Odzyskiwanie zasobów na pocz¹tku rundy (5%)
        player.RegenerateResources();
        enemy.RegenerateResources();

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
    // --- FUNKCJA ZAKOÑCZENIA BITWY ---
    IEnumerator EndBattleRoutine(bool playerWon)
    {
        yield return new WaitForSeconds(2.0f);
        EnemyData defeatedEnemy = GameManager.Instance.currentEnemyToFight;

        if (playerWon)
        {
            Debug.Log("<color=green>ZWYCIÊSTWO!</color>");

            // SPRAWDZAMY CZY TO TURNIEJ CZY MAPA:
            if (GameManager.Instance.isTournamentBattle)
            {
                // ZASADY TURNIEJU: £upy id¹ do worka (ryzykujemy dalej)
                if (defeatedEnemy != null)
                {
                    GameManager.Instance.pendingGold += defeatedEnemy.goldReward;
                    GameManager.Instance.pendingXP += defeatedEnemy.expReward;
                }
                GameManager.Instance.currentTournamentIndex++;
            }
            else
            {
                // ZASADY MAPY (Random Encounter): £upy id¹ od razu na sta³e do kieszeni!
                if (defeatedEnemy != null)
                {
                    GameManager.Instance.globalGold += defeatedEnemy.goldReward;
                    PlayerDataManager.Instance.currentExperience += defeatedEnemy.expReward;
                }
            }
        }
        else
        {
            Debug.Log("<color=red>PORA¯KA!</color>");

            if (GameManager.Instance.isTournamentBattle)
            {
                // ZASADY TURNIEJU: Wrzucamy to co mieliœmy w worku do g³ównej puli i resetujemy turniej
                GameManager.Instance.globalGold += GameManager.Instance.pendingGold;
                PlayerDataManager.Instance.currentExperience += GameManager.Instance.pendingXP;

                GameManager.Instance.pendingGold = 0;
                GameManager.Instance.pendingXP = 0;
                GameManager.Instance.currentTournamentIndex = 0;
            }
            else
            {
                // ZASADY MAPY: Gracz zgin¹³ w lesie. 
                // Mo¿esz tu w przysz³oœci dodaæ karê (np. utrata 10% z³ota) albo po prostu go odrodziæ.
                Debug.Log("Zgin¹³eœ na mapie! Zostajesz przeniesiony do bezpiecznego miejsca.");
            }
        }

        // --- MAGIA: Wracamy tam, sk¹d przyszliœmy! ---
        SceneManager.LoadScene(GameManager.Instance.sceneToLoadAfterBattle);
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