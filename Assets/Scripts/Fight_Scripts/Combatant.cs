using UnityEngine;

public class Combatant : MonoBehaviour
{
    [Header("Podstawowe Informacje")]
    public string combatantName;
    public bool isPlayer;
    public Sprite avatarImage; // ZDJÊCIE TWARZY

    [Header("Po³¹czenie z UI")]
    public CharacterUI myUI; // Referencja do panelu na dole ekranu

    [Header("Statystyki")]
    public int maxHP = 1000;
    public int currentHP;
    public int maxMana = 300;
    public int currentMana;
    public int maxStamina = 300; // DODANE: Maksymalna kondycja
    public int currentStamina;

    [Header("Atrybuty (Do wzorów)")]
    public int strength = 20;
    public int knowledge = 15;
    public int agility = 18;

    [Header("Komponenty Wizualne")]
    public Animator animator;

    void Start()
    {
        // Na start walki odnawiamy zasoby
        currentHP = maxHP;
        currentMana = maxMana;
        currentStamina = maxStamina;

        // Jeœli postaæ ma przypisany panel UI, ka¿emy mu siê ustawiæ
        if (myUI != null)
        {
            myUI.Setup(this);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP < 0) currentHP = 0; // ¯eby HP nie spad³o poni¿ej zera

        Debug.Log($"<color=red>{combatantName} otrzymuje {amount} obra¿eñ!</color>");

        if (animator != null) animator.SetTrigger("Hit");

        // Aktualizujemy pasek zdrowia po otrzymaniu ciosu!
        if (myUI != null) myUI.UpdateUI();

        if (currentHP <= 0) Die();
    }

    public void PlayAttackAnimation(string animationTriggerName)
    {
        if (animator != null) animator.SetTrigger(animationTriggerName);
    }

    void Die()
    {
        Debug.Log($"<b>{combatantName} pada na ziemiê!</b>");
        if (animator != null) animator.SetTrigger("Die");
    }
}