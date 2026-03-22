using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillSelectionWindow : MonoBehaviour
{
    public static SkillSelectionWindow Instance;
    public Transform gridContainer;
    public GameObject skillItemPrefab;
    public TextMeshProUGUI detailsText;

    private SkillAPHandler activeSlot;

    void Awake() { Instance = this; gameObject.SetActive(false); }

    // Przyjmuje List<CharacterSkill>
    public void Open(SkillAPHandler caller, List<CharacterSkill> availableSkills)
    {
        activeSlot = caller;
        gameObject.SetActive(true);
        detailsText.text = "Wybierz umiejêtnoœæ";

        foreach (Transform child in gridContainer) Destroy(child.gameObject);

        foreach (CharacterSkill skill in availableSkills)
        {
            // Zabezpieczenie przed pustymi polami w Inspektorze
            if (skill == null || skill.data == null) continue;

            GameObject item = Instantiate(skillItemPrefab, gridContainer);
            SkillItemUI ui = item.GetComponent<SkillItemUI>();
            ui.Setup(skill);
        }
    }

    // Odbiera CharacterSkill zamiast SkillData
    public void ShowDetails(CharacterSkill cSkill)
    {
        if (!cSkill.isUnlocked)
        {
            detailsText.text = "<color=red>[ZABLOKOWANE]</color>";
            detailsText.color = Color.white; 
            return;
        }

        SkillData data = cSkill.data;

        if (data.progression != null && data.progression.Count > 0)
        {
            // Bierzemy poziom od POSTACI (cSkill)
            int levelIndex = cSkill.currentLevel - 1;
            if (levelIndex >= data.progression.Count) levelIndex = data.progression.Count - 1;
            else if (levelIndex < 0) levelIndex = 0;

            int mana = data.progression[levelIndex].manaCost;
            int stamina = data.progression[levelIndex].staminaCost;

            List<string> costs = new List<string>();

            if (mana > 0) costs.Add($"{mana} Many");
            if (stamina > 0) costs.Add($"{stamina} Kondycji");

            if (costs.Count > 0)
            {
                detailsText.text = $"Wymagania (Poz. {cSkill.currentLevel}):\n" + string.Join("\n", costs);
                detailsText.color = Color.yellow;
            }
            else
            {
                detailsText.text = $"Brak kosztów (Poz. {cSkill.currentLevel})";
                detailsText.color = Color.green;
            }
        }
        else
        {
            detailsText.text = $"{data.skillName}\n(Brak wpisanych kosztów!)";
            detailsText.color = Color.white;
        }
    }

    public void HideDetails()
    {
        detailsText.text = "Wybierz umiejêtnoœæ";
        detailsText.color = Color.white;
    }

    // Wybiera CharacterSkill
    public void Select(CharacterSkill skill)
    {
        activeSlot.AssignSkill(skill);
        gameObject.SetActive(false);
    }

    public void CloseWindow()
    {
        gameObject.SetActive(false);
    }
}