using UnityEngine;
using System.Collections.Generic;

public abstract class EnemyAIBrain : ScriptableObject
{
    // Funkcja, która przyjmuje informacje o arenie i zwraca listê akcji do wykonania
    public abstract List<CombatAction> DecideTurn(Combatant me, Combatant player, ref int actionCounter);
}