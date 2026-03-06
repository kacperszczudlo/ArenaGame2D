using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Potrzebne do List<>

public class BattleManager : MonoBehaviour
{
    [Header("Aktorzy")]
    public Combatant player;
    public Combatant enemy;

    [Header("UI Gracza")]
    public List<SkillAPHandler> attackSlots; // Lista WSZYSTKICH 5 kó³ek ataku

    [Header("Pozycje na arenie")]
    public Transform playerStartPos; // Gdzie Rycerz stoi domyœlnie
    public Transform enemyMeleePos;  // Punkt tu¿ przed przeciwnikiem, sk¹d bijemy

    public void TestEndTurn()
    {
        // Uruchamiamy sekwencjê rundy w tle, ¿eby gra mog³a "czekaæ" na animacje
        StartCoroutine(ExecuteTurnRoutine());
    }

    IEnumerator ExecuteTurnRoutine()
    {
        Debug.Log("<b>--- START RUNDY ---</b>");

        // Sprawdzamy po kolei ka¿de z 5 kó³ek
        foreach (SkillAPHandler slot in attackSlots)
        {
            // Warunek: Kó³ko ma przypisany skill ORAZ gracz da³ min. 1 PA
            if (slot.currentSkill != null && slot.currentPA > 0)
            {
                Debug.Log($"Rycerz u¿ywa: {slot.currentSkill.skillName} (PA: {slot.currentPA})");

                // 1. Podbiegamy do wroga (zajmie to 0.3 sekundy)
                yield return StartCoroutine(MoveCharacter(player.transform, enemyMeleePos.position, 0.3f));

                // 2. Odpalamy animacjê z karty umiejêtnoœci
                player.PlayAttackAnimation(slot.currentSkill.animTriggerName);

                // CZEKAMY pó³ sekundy, ¿eby miecz "trafi³" (zanim zadamy obra¿enia)
                yield return new WaitForSeconds(0.5f);

                // 3. Zadajemy testowe obra¿enia
                enemy.TakeDamage(100);

                // CZEKAMY kolejn¹ sekundê, ¿eby Rycerz dokoñczy³ wymach mieczem
                yield return new WaitForSeconds(1.0f);

                // 4. Wracamy na pozycjê startow¹
                yield return StartCoroutine(MoveCharacter(player.transform, playerStartPos.position, 0.3f));

                // Ma³a pauza przed nastêpnym kó³kiem
                yield return new WaitForSeconds(0.2f);
            }
        }

        Debug.Log("<b>--- KONIEC RUNDY ---</b>");
    }

    // P³ynne przesuwanie postaci (z punktu A do B w czasie)
    IEnumerator MoveCharacter(Transform character, Vector3 targetPos, float duration)
    {
        Vector3 startPos = character.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            character.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null; // Czekamy do nastêpnej klatki gry
        }
        character.position = targetPos; // Upewniamy siê, ¿e dotar³ równo na miejsce
    }
}