using System;

namespace KingsDamageMeter.Controls
{
    [Flags]
    public enum DisplayType
    {
        Damage = 1,
        DamagePerSecond = 2,
        Percent = 4,
        Experience = 8,
        Kinah = 16,
        AbyssPoints = 32
    }
}