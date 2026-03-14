using System.Collections.Generic;

public static class StatusRegistry
{
    // S³ownik paruje StatusType (z Twojego Enum) z jego logik¹
    private static Dictionary<StatusType, StatusLogic> logics = new Dictionary<StatusType, StatusLogic>()
    {
        { StatusType.Shield, new LogicShield() },
        { StatusType.DamageOverTime, new LogicDOT() },
        { StatusType.HealOverTime, new LogicHOT() },
        { StatusType.Blessing, new LogicBlessing() },
        { StatusType.Fury, new LogicFury() },
        { StatusType.Freeze, new LogicFreeze() },
        { StatusType.Blindness, new LogicBlindness() },
        { StatusType.Poison, new LogicPoison() }
    };

    public static StatusLogic GetLogic(StatusType type)
    {
        if (logics.ContainsKey(type))
            return logics[type];
        return null;
    }
}