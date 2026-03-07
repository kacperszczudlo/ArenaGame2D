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

    [Header("Status UI")]
    public Transform statusIconsContainer; // Tu przeci¹gnij Horizontal Layout Group
    public GameObject statusIconPrefab;

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

        // POBIERANIE UMIEJÊTNOŒCI (Na razie ukryte, do pod³¹czenia w nastêpnym kroku z kó³kami na ekranie!)

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

    public void PlayAttackAnimation(string animationTriggerName)
    {
        if (animator != null) animator.SetTrigger(animationTriggerName);
    }

    void Die()
    {
        Debug.Log($"<b>{combatantName} pada na ziemiê!</b>");
        if (animator != null) animator.SetTrigger("Die");
    }

    public void Heal(int amount, string chanceText = "", Sprite icon = null)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;

        if (myUI != null) myUI.UpdateUI();

        ShowFloatingText("+" + amount.ToString(), DamagePopup.PopupType.Heal, icon, chanceText);
    }

    public void TakeDamage(int amount, bool isCritical = false, string chanceText = "")
    {
        currentHP -= amount;
        if (currentHP < 0) currentHP = 0;

        if (animator != null) animator.SetTrigger("Hit");
        if (myUI != null) myUI.UpdateUI();

        DamagePopup.PopupType type = isCritical ? DamagePopup.PopupType.CriticalDamage : DamagePopup.PopupType.NormalDamage;

        ShowFloatingText("-" + amount.ToString(), type, null, chanceText);

        if (currentHP <= 0) Die();
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
        if (activeStatuses.Count == 0) return;

        for (int i = activeStatuses.Count - 1; i >= 0; i--)
        {
            var status = activeStatuses[i];

            if (status.isDamage)
                TakeDamage(status.value, false, status.effectName);
            else
                Heal(status.value, status.effectName);

            status.duration--;
            if (status.duration <= 0) activeStatuses.RemoveAt(i);
        }

        RefreshStatusUI();
    }

    public void RefreshStatusUI()
    {
        if (statusIconsContainer == null) return;

        // Czyœcimy stare ikonki
        foreach (Transform child in statusIconsContainer) Destroy(child.gameObject);

        // Tworzymy nowe ikonki dla aktywnych statusów
        foreach (var status in activeStatuses)
        {
            if (statusIconPrefab != null)
            {
                GameObject iconGo = Instantiate(statusIconPrefab, statusIconsContainer);
                // Tutaj powinieneœ mieæ skrypt na prefabie, który ustawia Sprite i tekst duration
                // Przyk³ad: iconGo.GetComponent<StatusIconUI>().Setup(status);
            }
        }
    }

    public void AddStatusEffect(StatusEffect newEffect)
    {
        // Mo¿esz dodaæ logikê sprawdzaj¹c¹, czy status ju¿ istnieje, by go odœwie¿yæ
        activeStatuses.Add(newEffect);
        RefreshStatusUI();
    }
}