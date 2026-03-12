using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterUI : MonoBehaviour
{
    private Combatant target;

    [Header("Informacje G³ówne")]
    public Image avatarDisplay;
    public TextMeshProUGUI nameText;

    [Header("Paski Zasobów")]
    public ResourceBar hpBar;
    public ResourceBar manaBar;
    public ResourceBar staminaBar;

    public void Setup(Combatant combatant)
    {
        target = combatant;

        if (nameText != null) nameText.text = target.combatantName;
        if (avatarDisplay != null && target.avatarImage != null)
        {
            avatarDisplay.sprite = target.avatarImage;
        }

        // T³umaczymy Twoim paskom, jakie maj¹ byæ wartoœci MAKSYMALNE na start
        if (hpBar != null) hpBar.SetMaxResource(target.maxHP);
        if (manaBar != null) manaBar.SetMaxResource(target.maxMana);
        if (staminaBar != null) staminaBar.SetMaxResource(target.maxStamina);

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (target == null) return;

        // T³umaczymy paskom, jakie s¹ OBECNE wartoœci
        if (hpBar != null) hpBar.UpdateValue(target.currentHP);
        if (manaBar != null) manaBar.UpdateValue(target.currentMana);
        if (staminaBar != null) staminaBar.UpdateValue(target.currentStamina);
    }
}