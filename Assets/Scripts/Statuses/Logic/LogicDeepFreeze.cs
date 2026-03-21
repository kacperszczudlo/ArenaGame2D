using UnityEngine;

public class LogicDeepFreeze : StatusLogic
{
    // Zamro¿enie nie modyfikuje otrzymywanych obra¿eñ, przyjmujemy wszystko na klatê (w lód)
    public override int OnTakeDamage(Combatant owner, StatusEffect status, int incomingDamage, bool isDot, SkillCategory category)
    {
        return incomingDamage;
    }

    // Nie zadajemy obra¿eñ co turê, wiêc tu jest pusto
    public override void OnTurnStart(Combatant owner, StatusEffect status)
    {
        // Cisza, postaæ jest zamro¿ona :)
    }

    // Standardowe sprawdzanie, czy lód ju¿ stopnia³ (czas dobieg³ koñca)
    public override bool IsExpired(StatusEffect status)
    {
        return status.duration <= 0;
    }
}