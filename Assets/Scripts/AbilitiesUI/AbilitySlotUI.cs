using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilitySlotUI : MonoBehaviour
{
    public Image skillIcon;
    public Image lockMask;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI skillNameText; 
    public Button slotButton;

    private CharacterSkill mySkill;
    private AbilitiesWindow myWindow;

    public void Setup(CharacterSkill skill, AbilitiesWindow window)
    {
        mySkill = skill;
        myWindow = window;

        // Podmieniamy Ikonê
        if (skillIcon != null && skill.data != null)
            skillIcon.sprite = skill.data.icon;

        // Podmieniamy Nazwê pod kó³kiem! 
        if (skillNameText != null && skill.data != null)
            skillNameText.text = skill.data.skillName;

        RefreshVisuals();

        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(() => myWindow.SelectSkill(mySkill, this));
    }

    public void RefreshVisuals()
    {
        if (mySkill.currentLevel == 0)
        {
            if (lockMask != null) lockMask.gameObject.SetActive(true);
            if (levelText != null) levelText.text = "0";
            skillIcon.color = new Color(0.5f, 0.5f, 0.5f, 1f); // Szary, zablokowany
        }
        else
        {
            if (lockMask != null) lockMask.gameObject.SetActive(false);
            if (levelText != null) levelText.text = mySkill.currentLevel.ToString();
            skillIcon.color = Color.white; // Odblokowany
        }
    }
}