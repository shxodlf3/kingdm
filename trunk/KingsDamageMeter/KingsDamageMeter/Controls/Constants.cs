using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KingsDamageMeter.Controls
{
    public enum PlayerSortType
    {
        None,
        Name,
        Damage
    }

    public enum DisplayType
    {
        None,
        Damage,
        DamagePerSecond,
        Percent,
        Experience,
        Kinah
    }

    public enum PlayerFormatOptions
    {
        Short,
        Long
    }
}
