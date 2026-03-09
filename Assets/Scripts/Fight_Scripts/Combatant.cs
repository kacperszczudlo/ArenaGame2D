using UnityEngine;
using System.Collections.Generic;

public class Combatant : MonoBehaviour
{
    [Header("Podstawowe Informacje")]
    public string combatantName;
    public bool isPlayer;
    public Sprite avatarImage; // ZDJÊCIE TWARZY

    [Header("Leveling")]
    public int currentLevel = 1;

    [Header("Tylko dla Wroga (Zignoruj dla Gracza)")]
    public EnemyData enemyTemplate;

    [Header("Po³¹czenie z UI")]
    public CharacterUI myUI; // Referencja do panelu na dole ekranu

    [Header("Ustawienia Areny")]
    [Tooltip("W jakiej odleg³oœci od œrodka tej postaci ma stan¹æ wróg, ¿eby zadaæ cios w zwarciu?")]
    public float meleeStoppingDistance = 1.5f; // Dla smoka zrobisz np. 4.0, dla szczura 0.8

    [Header("Efekty Trafienia (VFX)")]
    public GameObject skillEffectPrefab; // Przeci¹gniesz tu nowy prefab
    public Transform centerSpawnPoint;   // Œrodek postaci (klatka piersiowa)

    [Header("Efekty")]
    public GameObject damagePopupPrefab; // Tu wrzucisz nowy prefab
    public Transform popupSpawnPoint;    // Miejsce nad g³ow¹
    public List<StatusEffect> activeStatuses = new List<StatusEffect>();

    [Header("Statystyki (Dla Gracza nadpisze je Mened¿er)")]
    public int maxHP = 1000;
    public int currentHP;
    public int maxMana = 300;
    public int currentMana;
    public int maxStamina = 300;
    public int currentStamina;

    [Header("Odpornoœci (Dla Gracza nadpisze je Mened¿er)")]
    public int physicalArmor = 10;
    public int magicResistance = 5;

    [Header("Ksiêga Umiejêtnoœci Postaci")]
    public List<CharacterSkill> mySkills;

    [Header("Atrybuty (Dla Gracza nadpisze je Mened¿er)")]
    public int strength = 20;
    public int knowledge = 15;
    public int agility = 18;
    public int power = 10; // DODANE: Moc

    public int health = 200; // Zostawiam z Twojego starego skryptu

    [Header("Walka (Dla Gracza nadpisze je Mened¿er)")]
    public int weaponDamage = 50;
    public int critChance = 5; // DODANE: Szansa na trafienie krytyczne

    [Header("Komponenty Wizualne")]
    public Animator animator;

    [Header("Nazwy Animacji (Triggery)")]
    [Tooltip("Wpisz tu nazwy triggerów z Animatora. Zostaw puste, jeœli postaæ nie ma takich animacji.")]
    public string hitAnimTrigger = "Hit";
    public string deathAnimTrigger = "Death";

    [Header("Status UI")]
    public Transform statusIconsContainer; // Tu przeci¹gnij Horizontal Layout Group
    public GameObject statusIconPrefab;

    [Header("Punkty Obrony w Bie¿¹cej Rundzie")]
    public int defenseMeleePA = 0;    // Obrona w zwarciu (fizyczna)
    public int defenseRangedPA = 0;   // Obrona przed dystansem (fiz/mag)
    public int defenseMentalPA = 0;   // Obrona przed urokami (psychiczna)
    //flaga dla animacji smierci
    public bool isDead = false;

    // Funkcja do czyszczenia obrony po zakoñczeniu rundy
    public void ResetDefensePA()
    {
        defenseMeleePA = 0;
        defenseRangedPA = 0;
        defenseMentalPA = 0;
    }

    void Start()
    {
        // 1. Najpierw decydujemy sk¹d bierzemy dane
        if (isPlayer && PlayerDataManager.Instance != null)
        {
            // Gracz pobiera wszystko z Globalnego Mened¿era
            LoadDataFromManager();
        }
        else if (!isPlayer && enemyTemplate != null)
        {
            // Wróg pobiera wszystko ze swojego Szablonu (EnemyData)!
            LoadEnemyData(enemyTemplate);
        }

        // 2. DOPIERO TERAZ wysy³amy dane do UI
        if (myUI != null)
        {
            myUI.Setup(this);
            myUI.UpdateUI();
        }
    }

    // --- FUNKCJA POBIERAJ¥CA DANE Z SERWERA (PLAYER DATA MANAGER) ---
    public void LoadDataFromManager()
    {
        var data = PlayerDataManager.Instance;

        // POBIERANIE ZSUMOWANYCH STATYSTYK
        currentLevel = data.currentLevel;
        maxHP = data.TotalMaxHP;
        currentHP = maxHP;

        maxMana = data.baseMaxMana;
        currentMana = maxMana;

        maxStamina = data.baseMaxStamina;
        currentStamina = maxStamina;

        strength = data.TotalStrength;
        agility = data.TotalAgility;
        knowledge = data.TotalKnowledge;
        power = data.TotalPower;

        physicalArmor = data.TotalPhysicalArmor;
        magicResistance = data.TotalMagicResistance;
        critChance = data.TotalCritChance;
        weaponDamage = data.weaponDamage;


        // --- POBIERANIE UMIEJÊTNOŒCI Z SERWERA ---
        mySkills.Clear();

        foreach (var savedSkill in data.unlockedSkills)
        {
            if (savedSkill.skill != null)
            {
                CharacterSkill newSkill = new CharacterSkill();
                newSkill.data = savedSkill.skill;
                newSkill.currentLevel = savedSkill.currentLevel;

                // ZMIANA: Sprawdzamy, czy skill ma poziom 0!
                if (savedSkill.currentLevel == 0)
                {
                    newSkill.isUnlocked = false; // Przyszarzony, zablokowany
                }
                else
                {
                    newSkill.isUnlocked = true;  // Aktywny, mo¿na klikaæ
                }

                mySkills.Add(newSkill);
            }
        }


        Debug.Log($"<color=cyan>Za³adowano gracza {combatantName} z Mened¿era! Max HP: {maxHP}, Crit: {critChance}%</color>");
    }

    // --- FUNKCJA POBIERAJ¥CA DANE Z PLIKU WROGA ---
    public void LoadEnemyData(EnemyData data)
    {
        combatantName = data.enemyName;
        avatarImage = data.avatarImage;
        meleeStoppingDistance = data.meleeStoppingDistance;
        currentLevel = data.level;

        maxHP = data.maxHP;
        currentHP = maxHP;
        maxMana = data.maxMana;
        currentMana = maxMana;
        maxStamina = data.maxStamina;
        currentStamina = maxStamina;

        strength = data.strength;
        agility = data.agility;
        knowledge = data.knowledge;
        power = data.power;

        physicalArmor = data.physicalArmor;
        magicResistance = data.magicResistance;
        critChance = data.critChance;
        weaponDamage = data.weaponDamage;

        // Kopiujemy skille
        mySkills.Clear();
        foreach (var skill in data.enemySkills)
        {
            mySkills.Add(new CharacterSkill { data = skill.data, currentLevel = skill.currentLevel, isUnlocked = true });
        }

        Debug.Log($"<color=red>Za³adowano Wroga: {combatantName} z pliku (Szablonu)!</color>");
    }

    public void ConsumeResources(int manaAmount, int staminaAmount)
    {
        currentMana -= manaAmount;
        if (currentMana < 0) currentMana = 0;

        currentStamina -= staminaAmount;
        if (currentStamina < 0) currentStamina = 0;

        Debug.Log($"<color=blue>{combatantName} zu¿ywa {manaAmount} Many i {staminaAmount} Kondycji.</color>");

        if (myUI != null) myUI.UpdateUI();
    }

    public void RegenerateResources()
    {
        // Liczymy 5% z maksymalnej wartoœci
        int manaRegen = Mathf.RoundToInt(maxMana * 0.05f);
        int staminaRegen = Mathf.RoundToInt(maxStamina * 0.05f);

        // Dodajemy, ale upewniamy siê, ¿e nie przekroczymy maksimum
        currentMana = Mathf.Min(maxMana, currentMana + manaRegen);
        currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegen);

        if (myUI != null) myUI.UpdateUI();
    }

    public void PlayAttackAnimation(string animationTriggerName)
    {
        if (animator != null) animator.SetTrigger(animationTriggerName);
    }

    void Die()
    {
        if (isDead) return; // Jeœli ju¿ umar³, ignorujemy kolejne komendy!
        isDead = true;

        Debug.Log($"<b>{combatantName} pada na ziemiê!</b>");
        if (animator != null && !string.IsNullOrEmpty(deathAnimTrigger))
        {
            animator.SetTrigger(deathAnimTrigger);
        }
    }

    public void Heal(int amount, string chanceText = "", Sprite icon = null)
    {
        // Jeœli faktycznie jest jakieœ leczenie (np. mikstura albo skill lecz¹cy)
        if (amount > 0)
        {
            currentHP += amount;
            if (currentHP > maxHP) currentHP = maxHP;

            if (myUI != null) myUI.UpdateUI();

            ShowFloatingText("+" + amount.ToString(), DamagePopup.PopupType.Heal, icon, chanceText);
        }
        else
        {
            // Jeœli leczenie wynosi 0 (czyli rzucamy czysty Buff, np. Tarcza, Furia, Modlitwa)
            // Zamiast g³upiego "+0", wyœwietlamy fajny tekst!
            ShowFloatingText("Wzmocnienie!", DamagePopup.PopupType.Heal, icon, chanceText);
        }
    }

    // Zmieniamy sygnaturê, by przyjmowa³a isDot i category
    public void TakeDamage(int damage, bool isCritical = false, string chanceText = "", bool isDot = false, SkillCategory category = SkillCategory.MeleePhysical)
    {
        int finalDamage = damage;

        // 1. Przepuszczamy obra¿enia przez statusy (Tarcza, Modlitwa)
        // U¿ywamy .ToArray(), ¿eby móc bezpiecznie modyfikowaæ listê w trakcie pêtli
        foreach (var s in activeStatuses.ToArray())
        {
            StatusLogic logic = StatusRegistry.GetLogic(s.type);
            if (logic != null)
            {
                // Wysy³amy nasze nowe informacje do Tarczy/Modlitwy!
                finalDamage = logic.OnTakeDamage(this, s, finalDamage, isDot, category);
            }
        }

        // 2. ZU¯YTE TARCZE DO KOSZA
        activeStatuses.RemoveAll(s => StatusRegistry.GetLogic(s.type)?.IsExpired(s) ?? true);
        RefreshStatusUI();

        // Jeœli po przejœciu przez tarcze zosta³o 0 obra¿eñ (zablokowane/unikniête), wychodzimy!
        if (finalDamage <= 0) return;

        // 3. OBLICZAMY RESZTÊ JAK ZWYKLE
        currentHP -= finalDamage;
        if (currentHP < 0) currentHP = 0;

        if (animator != null) animator.SetTrigger("Hit");
        if (myUI != null) myUI.UpdateUI();

        DamagePopup.PopupType pType = isCritical ? DamagePopup.PopupType.CriticalDamage : DamagePopup.PopupType.NormalDamage;
        ShowFloatingText("-" + finalDamage, pType, null, chanceText);

        if (currentHP <= 0)
        {
            Die(); // Jeœli umar³, zagraj TYLKO œmieræ
        }
        else
        {
            // Zamiast sztywnego "Hit", u¿ywamy naszej zmiennej!
            if (animator != null && !string.IsNullOrEmpty(hitAnimTrigger))
            {
                animator.SetTrigger(hitAnimTrigger);
            }
        }
    }

    public void ShowFloatingText(string text, DamagePopup.PopupType type, Sprite icon = null, string chanceText = "")
    {
        if (damagePopupPrefab != null && popupSpawnPoint != null)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0f, 0.5f), 0);
            GameObject popup = Instantiate(damagePopupPrefab, popupSpawnPoint.position + randomOffset, Quaternion.identity);

            popup.GetComponent<DamagePopup>().Setup(text, type, icon, chanceText);
        }
    }

    public void PlaySkillEffect(Sprite icon)
    {
        if (skillEffectPrefab != null && centerSpawnPoint != null && icon != null)
        {
            GameObject vfx = Instantiate(skillEffectPrefab, centerSpawnPoint.position, Quaternion.identity);
            vfx.GetComponent<SkillEffectVFX>().Setup(icon);
        }
    }
    public void ProcessStatuses()
    {
        // 1. Odpalamy logikê na pocz¹tku tury (np. Krwawienie zadaje ból)
        for (int i = activeStatuses.Count - 1; i >= 0; i--)
        {
            var status = activeStatuses[i];
            StatusLogic logic = StatusRegistry.GetLogic(status.type);

            if (logic != null) logic.OnTurnStart(this, status);

            status.duration--; // Odejmujemy rundê
        }

        // 2. Usuwamy statusy, które wygas³y (Koniec rund, albo Tarcza straci³a ³adunki)
        activeStatuses.RemoveAll(s => StatusRegistry.GetLogic(s.type)?.IsExpired(s) ?? true);
        RefreshStatusUI();
    }

    public void RefreshStatusUI()
    {
        if (statusIconsContainer == null) return;
        foreach (Transform child in statusIconsContainer) Destroy(child.gameObject);
        foreach (var status in activeStatuses)
        {
            if (statusIconPrefab != null)
            {
                GameObject iconGo = Instantiate(statusIconPrefab, statusIconsContainer);
                iconGo.GetComponent<StatusIconUI>().Setup(status);
            }
        }
    }

    public void AddStatusEffect(StatusEffect newEffect)
    {
        StatusEffect existing = activeStatuses.Find(s => s.effectName == newEffect.effectName);

        if (existing != null)
        {
            if (newEffect.type == StatusType.DamageOverTime)
            {
                existing.remainingHits += newEffect.remainingHits;

                // NAPRAWA UI: Aktualizujemy cyferkê na ikonce po do³o¿eniu stacków!
                existing.duration = existing.remainingHits;

                if (newEffect.value > existing.value) existing.value = newEffect.value;
            }
            else if (newEffect.type == StatusType.Shield || newEffect.type == StatusType.Blessing)
            {
                existing.remainingHits = newEffect.remainingHits;
                existing.duration = newEffect.duration;
            }
        }
        else
        {
            activeStatuses.Add(newEffect);
        }
        RefreshStatusUI();
    }

    public int GetCombatPhysicalArmor()
    {
        int arm = physicalArmor;
        foreach (var s in activeStatuses)
        {
            if (s.type == StatusType.Fury) arm -= s.value;
            // Modlitwa tu NIE DZIA£A. Pancerz fizyczny zostaje bez zmian (czyli ³ucznik bije normalnie).
        }
        return arm;
    }

    public int GetCombatMagicResistance()
    {
        int res = magicResistance;
        foreach (var s in activeStatuses)
        {
            if (s.type == StatusType.Blessing) res += s.value; // Modlitwa dodaje Obronê Magiczn¹!
            if (s.type == StatusType.Fury) res -= s.value;
        }
        return res;
    }

    public float GetCombatDamageMultiplier()
    {
        float mult = 1.0f;
        foreach (var s in activeStatuses)
        {
            // ZAMIAST na sztywno "+1.5f", dodajemy to, co wpisa³eœ w 'Effect Multiplier'!
            if (s.type == StatusType.Fury) mult += s.multiplier;
        }
        return mult;
    }

    public float GetCombatHitChanceMultiplier()
    {
        float mult = 1.0f;
        foreach (var s in activeStatuses)
        {
            // Przerabiamy wpisane -10 na u³amek. 
            // Wzór: 1 + (-10 / 100) = 0.9.  Zatem celnoœæ zostanie pomno¿ona przez 0.9!
            if (s.type == StatusType.Fury)
            {
                mult *= (1f + (s.hitChanceMod / 100f));
            }
        }
        return mult;
    }
}