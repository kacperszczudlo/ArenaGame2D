using UnityEngine;

public class CharacterUI : MonoBehaviour
{
    [Header("Statystyki ¯ycia")]
    public float maxHP = 1000f;
    public float currentHP = 1000f;

    [Header("Statystyki Zasobów")]
    public float maxMana = 500f;
    public float currentMana = 500f;
    public float maxStamina = 300f;
    public float currentStamina = 300f;

    [Header("Referencje do pasków")]
    public ResourceBar hpBar;
    public ResourceBar manaBar;
    public ResourceBar staminaBar;

    void Start()
    {
        // Inicjalizacja pasków na 100% na starcie walki
        if (hpBar != null) hpBar.SetMaxResource(maxHP);
        if (manaBar != null) manaBar.SetMaxResource(maxMana);
        if (staminaBar != null) staminaBar.SetMaxResource(maxStamina);

        RefreshAllBars();
    }

    public void RefreshAllBars()
    {
        if (hpBar != null) hpBar.UpdateValue(currentHP);
        if (manaBar != null) manaBar.UpdateValue(currentMana);
        if (staminaBar != null) staminaBar.UpdateValue(currentStamina);
    }

    // Funkcja do testowania - zadaje 100 obra¿eñ
    [ContextMenu("Test Damage")]
    public void TakeTestDamage()
    {
        currentHP -= 100;
        RefreshAllBars();
    }
}