using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillSelectionWindow : MonoBehaviour
{
    public static SkillSelectionWindow Instance;
    public Transform gridContainer;
    public GameObject skillItemPrefab;
    public TextMeshProUGUI detailsText; // Tu przeci¹gnij tekst z do³u okna

    private SkillAPHandler activeSlot;

    void Awake() { Instance = this; gameObject.SetActive(false); }

    public void Open(SkillAPHandler caller, List<SkillData> availableSkills)
    {
        activeSlot = caller;
        gameObject.SetActive(true);
        detailsText.text = "Wybierz umiejêtnoœæ"; // Tekst domyœlny

        foreach (Transform child in gridContainer) Destroy(child.gameObject);

        foreach (SkillData skill in availableSkills)
        {
            GameObject item = Instantiate(skillItemPrefab, gridContainer);
            SkillItemUI ui = item.GetComponent<SkillItemUI>();
            ui.Setup(skill);
        }
    }

    // Wywo³ywane przez SkillItemUI podczas najechania myszk¹
    public void ShowDetails(SkillData data)
    {
        if (data.progression != null && data.progression.Count > 0)
        {
            int mana = data.progression[0].manaCost;
            int stamina = data.progression[0].staminaCost;

            System.Collections.Generic.List<string> costs = new System.Collections.Generic.List<string>();

            // Mana dodawana jako pierwsza (bêdzie wy¿ej)
            if (mana > 0) costs.Add($"{mana} Many");

            // Kondycja dodawana jako druga (bêdzie ni¿ej)
            if (stamina > 0) costs.Add($"{stamina} Kondycji");

            // Jeœli s¹ jakiekolwiek koszty, ³¹czymy je znakiem nowej linii (\n)
            if (costs.Count > 0)
            {
                detailsText.text = "Wymagania:\n" + string.Join("\n", costs);
                detailsText.color = Color.yellow;
            }
            else
            {
                detailsText.text = "Brak kosztów (Darmowe)";
                detailsText.color = Color.green;
            }
        }
        else
        {
            detailsText.text = data.skillName + "\n(Brak wpisanych kosztów!)";
            detailsText.color = Color.white;
        }
    }

    public void HideDetails()
    {
        detailsText.text = "Wybierz umiejêtnoœæ...";
        detailsText.color = Color.white;
    }

    public void Select(SkillData skill)
    {
        activeSlot.AssignSkill(skill);
        gameObject.SetActive(false);
    }
    public void CloseWindow()
    {
        gameObject.SetActive(false);
    }
}