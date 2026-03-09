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

    // Przyjmuje List<CharacterSkill> zamiast List<SkillData>
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
        SkillData data = cSkill.data;

        if (data.progression != null && data.progression.Count > 0)
        {
            // Bierzemy poziom od POSTACI (cSkill), a nie z pliku danych
            int levelIndex = cSkill.currentLevel - 1;
            if (levelIndex >= data.progression.Count) levelIndex = data.progression.Count - 1;
            else if (levelIndex < 0) levelIndex = 0;

            int mana = data.progression[levelIndex].manaCost;
            int stamina = data.progression[levelIndex].staminaCost;

            List<string> costs = new List<string>();

            if (mana > 0) costs.Add($"{mana} Many");
            if (stamina > 0) costs.Add($"{stamina} Kondycji");

            // Informacja, czy skill jest zablokowany
            string lockStatus = cSkill.isUnlocked ? "" : "<color=red>[ZABLOKOWANE]</color>\n";

            if (costs.Count > 0)
            {
                detailsText.text = $"{lockStatus}Wymagania (Poz. {cSkill.currentLevel}):\n" + string.Join("\n", costs);
                detailsText.color = cSkill.isUnlocked ? Color.yellow : new Color(0.7f, 0.7f, 0.7f);
            }
            else
            {
                detailsText.text = $"{lockStatus}Brak kosztów (Poz. {cSkill.currentLevel})";
                detailsText.color = cSkill.isUnlocked ? Color.green : new Color(0.7f, 0.7f, 0.7f);
            }
        }
        else
        {
            string lockStatus = cSkill.isUnlocked ? "" : "<color=red>[ZABLOKOWANE]</color>\n";
            detailsText.text = $"{lockStatus}{data.skillName}\n(Brak wpisanych kosztów!)";
            detailsText.color = Color.white;
        }
    }

    public void HideDetails()
    {
        detailsText.text = "Wybierz umiejêtnoœæ...";
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