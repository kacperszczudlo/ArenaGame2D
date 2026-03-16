public class LogicPoison : StatusLogic
{
    public override bool IsExpired(StatusEffect effect)
    {
        return effect.duration <= 0;
    }
}