public class LogicVoodooCurse : StatusLogic
{
    public override bool IsExpired(StatusEffect effect)
    {
        // Kl¹twa po prostu znika, gdy skoñcz¹ siê jej rundy
        return effect.duration <= 0;
    }
}