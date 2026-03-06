using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // Potrzebne do wykrywania najechania (Hover)

public class SkillItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image iconImage;
    public TextMeshProUGUI nameText;

    private SkillData data;

    // Funkcja ustawiaj¹ca dane skilla w prefabie
    public void Setup(SkillData skillData)
    {
        data = skillData;
        iconImage.sprite = data.icon;
        nameText.text = data.skillName;
    }

    // Klikniêcie w ikonê
    public void OnClick()
    {
        SkillSelectionWindow.Instance.Select(data);
    }

    // Najechanie myszk¹ (Wyœwietlamy koszty)
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Wywo³ujemy funkcjê w oknie, która poka¿e koszty na dole
        SkillSelectionWindow.Instance.ShowDetails(data);
    }

    // Zjechanie myszk¹ (Przywracamy nazwê)
    public void OnPointerExit(PointerEventData eventData)
    {
        SkillSelectionWindow.Instance.HideDetails();
    }
}