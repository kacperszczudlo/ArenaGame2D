using UnityEngine;

public class Combatant : MonoBehaviour
{
    [Header("Podstawowe Informacje")]
    public string combatantName;
    public bool isPlayer; // Zaznaczysz to u Rycerza, a odznaczysz u potwora

    [Header("Statystyki")]
    public int maxHP = 1000;
    public int currentHP;
    public int maxMana = 300;
    public int currentMana;
    public int maxStamina;
    public int currentStamina = 300;

    [Header("Atrybuty (Do wzorów)")]
    public int strength = 20;
    public int knowledge = 15;
    public int agility = 18;

    [Header("Komponenty Wizualne")]
    public Animator animator; // Tu podepniemy system animacji Unity

    void Start()
    {
        // Na start walki odnawiamy ¿ycie i manê
        currentHP = maxHP;
        currentMana = maxMana;
        currentStamina = maxStamina;
    }

    // Funkcja wywo³ywana przez Kalkulator Walki, gdy postaæ obrywa
    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        Debug.Log($"<color=red>{combatantName} otrzymuje {amount} obra¿eñ!</color> Zosta³o HP: {currentHP}");

        if (animator != null)
        {
            animator.SetTrigger("Hit"); // Odtwarza animacjê otrzymania ciosu
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    // Funkcja wywo³ywana, gdy postaæ u¿ywa skilla
    public void PlayAttackAnimation(string animationTriggerName)
    {
        if (animator != null)
        {
            animator.SetTrigger(animationTriggerName);
            Debug.Log($"{combatantName} wykonuje atak: {animationTriggerName}!");
        }
    }

    void Die()
    {
        Debug.Log($"<b>{combatantName} pada na ziemiê!</b>");
        if (animator != null) animator.SetTrigger("Die");
    }
}