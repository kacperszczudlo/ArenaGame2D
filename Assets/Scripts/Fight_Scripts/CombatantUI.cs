using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatantUI : MonoBehaviour
{
    private Combatant target;

    [Header("Informacje G³ówne")]
    public Image avatarDisplay;
    public TextMeshProUGUI nameText;

    [Header("Paski Wype³nienia (Image: Filled)")]
    public Image hpBarFill;
    public Image manaBarFill;
    public Image staminaBarFill;

    [Header("Teksty Wartoœci (np. 700 / 1000)")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI staminaText;

    // Funkcja wywo³ywana przez Combatant.cs na starcie walki
    public void Setup(Combatant combatant)
    {
        target = combatant;

        if (nameText != null) nameText.text = target.combatantName;
        if (avatarDisplay != null && target.avatarImage != null)
        {
            avatarDisplay.sprite = target.avatarImage;
        }

        UpdateUI();
    }

    // Aktualizuje paski i teksty (wywo³ywana np. po otrzymaniu ciosu lub zu¿yciu many)
    public void UpdateUI()
    {
        if (target == null) return;

        // --- ZDROWIE ---
        if (hpText != null) hpText.text = $"{target.currentHP} / {target.maxHP}";
        if (hpBarFill != null) hpBarFill.fillAmount = (float)target.currentHP / target.maxHP;

        // --- MANA ---
        if (manaText != null) manaText.text = $"{target.currentMana} / {target.maxMana}";
        if (manaBarFill != null) manaBarFill.fillAmount = (float)target.currentMana / target.maxMana;

        // --- KONDYCJA ---
        if (staminaText != null) staminaText.text = $"{target.currentStamina} / {target.maxStamina}";
        if (staminaBarFill != null) staminaBarFill.fillAmount = (float)target.currentStamina / target.maxStamina;
    }
}