using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image iconImage;
    public TextMeshProUGUI nameText;

    private CharacterSkill mySkill;

    // Funkcja ustawiaj¹ca dane skilla w prefabie
    public void Setup(CharacterSkill cSkill)
    {
        mySkill = cSkill;
        iconImage.sprite = mySkill.data.icon;

        if (nameText != null)
            nameText.text = mySkill.data.skillName;

        Button btn = GetComponent<Button>();

        // WYSZARZANIE I BLOKOWANIE SKILLA
        if (!mySkill.isUnlocked)
        {
            iconImage.color = new Color(0.3f, 0.3f, 0.3f, 1f); // Ciemnoszary
            if (btn != null) btn.interactable = false; // Wy³¹cza klikanie
        }
        else
        {
            iconImage.color = Color.white; // Normalny kolor
            if (btn != null) btn.interactable = true; // W³¹cza klikanie
        }
    }

    // Klikniêcie w ikonê
    public void OnClick()
    {
        if (mySkill.isUnlocked)
        {
            SkillSelectionWindow.Instance.Select(mySkill);
        }
    }

    // Najechanie myszk¹ (Wyœwietlamy koszty)
    public void OnPointerEnter(PointerEventData eventData)
    {
        SkillSelectionWindow.Instance.ShowDetails(mySkill);
    }

    // Zjechanie myszk¹ (Przywracamy nazwê)
    public void OnPointerExit(PointerEventData eventData)
    {
        SkillSelectionWindow.Instance.HideDetails();
    }
}