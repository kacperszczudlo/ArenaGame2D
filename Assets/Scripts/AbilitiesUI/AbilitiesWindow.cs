using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class AbilitiesWindow : MonoBehaviour
{
    [Header("Lewa Strona - Kó³ka")]
    public Transform slotsContainer;
    public GameObject abilitySlotPrefab;
    private List<AbilitySlotUI> spawnedSlots = new List<AbilitySlotUI>();

    [Header("Góra - Punkty")]
    public TextMeshProUGUI availablePointsText;

    [Header("Prawa Strona - Informacje")]
    public GameObject rightPanel;
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI formulaText;

    [Header("Prawa Strona - Statystyki i Preview")]
    public TextMeshProUGUI statsPreviewText;
    public TextMeshProUGUI effectsPreviewText;
    public TextMeshProUGUI requiredLevelText;
    public TextMeshProUGUI upgradeCostText;
    public Button upgradeButton;

    [Header("Prawa Strona - Przyciski Poziomów (1-7)")]
    [Tooltip("Przeci¹gnij tu wszystkie 7 przycisków z cyferkami!")]
    public Button[] levelButtons;
    [Tooltip("Grafika dla Wbitego Poziomu (Zielona)")]
    public Sprite unlockedLevelSprite;
    [Tooltip("Grafika dla Zablokowanego Poziomu (Szara)")]
    public Sprite lockedLevelSprite;
    [Tooltip("Grafika dla Podgl¹danego Poziomu (¯ó³ta)")]
    public Sprite previewLevelSprite;

    private CharacterSkill selectedSkill;
    private AbilitySlotUI selectedSlotUI;
    private int currentPreviewLevel = 1;

    void Start()
    {
        RefreshWindow();
    }

    void OnEnable()
    {
        RefreshWindow();
    }

    public void RefreshWindow()
    {
        if (PlayerDataManager.Instance == null) return;

        if (availablePointsText != null)
            availablePointsText.text = PlayerDataManager.Instance.availableSkillPoints.ToString();

        if (spawnedSlots.Count == 0)
        {
            AbilitySlotUI[] prePlacedSlots = slotsContainer.GetComponentsInChildren<AbilitySlotUI>();

            for (int i = 0; i < PlayerDataManager.Instance.unlockedSkills.Count; i++)
            {
                if (i >= prePlacedSlots.Length) break;

                var savedSkill = PlayerDataManager.Instance.unlockedSkills[i];
                if (savedSkill == null || savedSkill.skill == null) continue;

                CharacterSkill charSkill = new CharacterSkill();
                charSkill.data = savedSkill.skill;
                charSkill.currentLevel = savedSkill.currentLevel;
                charSkill.isUnlocked = (savedSkill.currentLevel > 0);

                prePlacedSlots[i].Setup(charSkill, this);
                spawnedSlots.Add(prePlacedSlots[i]);
            }

            for (int i = PlayerDataManager.Instance.unlockedSkills.Count; i < prePlacedSlots.Length; i++)
            {
                prePlacedSlots[i].gameObject.SetActive(false);
            }

            if (rightPanel != null) rightPanel.SetActive(false);
        }
        else
        {
            foreach (var slot in spawnedSlots) slot.RefreshVisuals();
        }

        if (selectedSkill != null)
        {
            UpdateRightPanel(currentPreviewLevel);
        }
    }

    public void SelectSkill(CharacterSkill skill, AbilitySlotUI slotUI)
    {
        selectedSkill = skill;
        selectedSlotUI = slotUI;
        if (rightPanel != null) rightPanel.SetActive(true);

        if (skillNameText != null) skillNameText.text = skill.data.skillName;
        if (descriptionText != null) descriptionText.text = skill.data.skillDescription;
        if (formulaText != null) formulaText.text = GenerateFormulaText(skill.data);

        // Domyœlnie pokazujemy statystyki NASTÊPNEGO poziomu (lub 1, jeœli skill zablokowany)
        currentPreviewLevel = skill.currentLevel == 0 ? 1 : Mathf.Min(skill.currentLevel + 1, skill.data.progression.Count);

        UpdateRightPanel(currentPreviewLevel);
    }

    public void ClickPreviewLevel(int level)
    {
        currentPreviewLevel = level;
        UpdateRightPanel(currentPreviewLevel);
    }

    private void UpdateRightPanel(int previewLvl)
    {
        if (selectedSkill == null || selectedSkill.data == null) return;

        SkillData data = selectedSkill.data;
        SkillLevelData previewData = data.GetLevelData(previewLvl);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] == null) continue;
            int lvl = i + 1;
            Image btnImage = levelButtons[i].GetComponent<Image>();

            if (lvl == previewLvl) btnImage.sprite = previewLevelSprite;
            else if (lvl <= selectedSkill.currentLevel) btnImage.sprite = unlockedLevelSprite;
            else btnImage.sprite = lockedLevelSprite;

            levelButtons[i].gameObject.SetActive(lvl <= data.progression.Count);
        }

        if (previewData != null)
        {
            // 1. G£ÓWNE STATYSTYKI
            string stats = $"Poziom Umiejêtnoœci: {previewLvl}\n";
            if (data.category != SkillCategory.PositiveCharm || previewData.damageMultiplier > 0)
                stats += $"Obra¿enia: {Mathf.RoundToInt(previewData.damageMultiplier * 100)}%\n";

            if (previewData.staminaCost > 0) stats += $"Kondycja: {previewData.staminaCost}\n";
            if (previewData.manaCost > 0) stats += $"Mana: {previewData.manaCost}\n";
            if (previewData.hitChanceBonus != 0) stats += $"Szansa trafienia: {(previewData.hitChanceBonus > 0 ? "+" : "")}{previewData.hitChanceBonus}%\n";

            if (statsPreviewText != null) statsPreviewText.text = stats;

            // 2. ODDZIELNE EFEKTY SPECJALNE
            string effectsStats = "";

            if (data.category == SkillCategory.PositiveCharm)
                effectsStats += $"Trudnoœæ rzucenia (Buff): {previewData.selfCastDifficulty}\n";

            string sName = data.skillName.ToLower();

            // ---  ZAKA¯ENIA / TRUCIZNY ---
            if (sName.Contains("zatrut") || sName.Contains("trucizna") || sName.Contains("zakazenie") || sName.Contains("zaka¿enie"))
            {
                effectsStats += $"Szansa na zaka¿enie: {previewData.statusEffectChance}%\n";
                effectsStats += $"Nak³adane ³adunki (stacki): {previewData.effectCharges}\n";
                effectsStats += $"Obra¿enia z ³adunku: {Mathf.RoundToInt(previewData.effectMultiplier * 100)}% si³y ciosu\n";
            }
            // ------------------------------------------------
            else if (sName.Contains("furia"))
            {
                effectsStats += $"Czas trwania: {previewData.effectDuration} rund(y)\n";
                effectsStats += $"Bonus do obra¿eñ: +{Mathf.RoundToInt(previewData.effectMultiplier * 100)}%\n";
                effectsStats += $"Kara do pancerza: -{previewData.effectValue}\n";
                if (previewData.effectHitChanceMod != 0)
                    effectsStats += $"Kara do celnoœci: {previewData.effectHitChanceMod}%\n";
            }
            else if (sName.Contains("blok") || sName.Contains("tarcza"))
            {
                effectsStats += $"Czas trwania: {previewData.effectDuration} rund(y)\n";
                effectsStats += $"Iloœæ bloków (³adunki): {previewData.effectCharges}\n";
                effectsStats += $"Redukcja obra¿eñ: {previewData.effectValue}%\n";
            }
            else if (sName.Contains("modlitwa") || sName.Contains("b³ogos³awieñstwo"))
            {
                effectsStats += $"Czas trwania: {previewData.effectDuration} rund(y)\n";
                effectsStats += $"Bonus do odpornoœci magicznej: +{previewData.effectValue}\n";
                effectsStats += $"Boskie uniki (³adunki): {previewData.effectCharges}\n";
            }
            else if (previewData.statusEffectChance > 0 && previewData.statusEffectChance < 100)
            {
                effectsStats += $"Szansa na efekt specjalny: {previewData.statusEffectChance}%\n";
            }

            if (!string.IsNullOrEmpty(effectsStats))
            {
                effectsStats = "<color=#ffcc00>Efekty Umiejêtnoœci:</color>\n" + effectsStats;
            }

            if (effectsPreviewText != null) effectsPreviewText.text = effectsStats;

            // 3. LOGIKA ZBIORCZYCH KOSZTÓW I AWANSU
            if (previewLvl > selectedSkill.currentLevel && previewLvl <= data.progression.Count)
            {
                int totalCost = 0;
                for (int i = selectedSkill.currentLevel + 1; i <= previewLvl; i++) totalCost += i;

                if (requiredLevelText != null) requiredLevelText.text = $"Wymagany poziom postaci: {previewData.requiredCharacterLevel}";
                if (upgradeCostText != null) upgradeCostText.text = $"Wymagane punkty umiejêtnoœci: {totalCost}";

                bool hasPoints = PlayerDataManager.Instance.availableSkillPoints >= totalCost;
                bool hasLevel = PlayerDataManager.Instance.currentLevel >= previewData.requiredCharacterLevel;

                if (requiredLevelText != null) requiredLevelText.color = hasLevel ? Color.white : Color.red;
                if (upgradeCostText != null) upgradeCostText.color = hasPoints ? Color.white : Color.red;

                if (upgradeButton != null) upgradeButton.interactable = hasPoints && hasLevel;
            }
            else
            {
                if (requiredLevelText != null)
                {
                    requiredLevelText.text = previewLvl <= selectedSkill.currentLevel ? "Poziom Odblokowany" : "Maksymalny poziom!";
                    requiredLevelText.color = Color.gray;
                }
                if (upgradeCostText != null)
                {
                    upgradeCostText.text = "Wymagane punkty umiejêtnoœci: 0";
                    upgradeCostText.color = Color.gray;
                }

                if (upgradeButton != null) upgradeButton.interactable = false;
            }
        }
    }

    public void UpgradeSkill()
    {
        // Upewniamy siê, ¿e próbujemy wbiæ wy¿szy level ni¿ mamy
        if (currentPreviewLevel > selectedSkill.currentLevel)
        {
            // Ponownie liczymy zbiorczy koszt dla bezpieczeñstwa
            int totalCost = 0;
            for (int i = selectedSkill.currentLevel + 1; i <= currentPreviewLevel; i++)
            {
                totalCost += i;
            }

            SkillLevelData previewData = selectedSkill.data.GetLevelData(currentPreviewLevel);
            bool hasLevel = PlayerDataManager.Instance.currentLevel >= previewData.requiredCharacterLevel;

            if (PlayerDataManager.Instance.availableSkillPoints >= totalCost && hasLevel)
            {
                // Odejmujemy wszystkie potrzebne pnkty z puli
                PlayerDataManager.Instance.availableSkillPoints -= totalCost;

                // Awansujemy skill OD RAZU na docelowy poziom!
                selectedSkill.currentLevel = currentPreviewLevel;
                selectedSkill.isUnlocked = true;

                // Zapisujemy nowy poziom do g³ównego sejwaa gry
                var savedData = PlayerDataManager.Instance.unlockedSkills.Find(s => s.skill == selectedSkill.data);
                if (savedData != null) savedData.currentLevel = selectedSkill.currentLevel;

                // Po awansie automatycznie podgl¹damy kolejny level (jeœli nie dobiliœmy do maxa)
                if (currentPreviewLevel < selectedSkill.data.progression.Count)
                {
                    currentPreviewLevel++;
                }

                RefreshWindow();
            }
        }
    }

    private string GenerateFormulaText(SkillData data)
    {
        List<string> parts = new List<string>();
        if (data.strengthWeight > 0) parts.Add($"{data.strengthWeight}*Si³a");
        if (data.agilityWeight > 0) parts.Add($"{data.agilityWeight}*Zrêcznoœæ");
        if (data.knowledgeWeight > 0) parts.Add($"{data.knowledgeWeight}*Wiedza");
        if (data.powerWeight > 0) parts.Add($"{data.powerWeight}*Moc");
        if (data.weaponDamageWeight > 0) parts.Add("Broñ");

        if (parts.Count == 0) return "";

        return $"OBR.: {string.Join(" + ", parts)}";
    }

    public void CloseWindow()
    {
        gameObject.SetActive(false);
    }
}