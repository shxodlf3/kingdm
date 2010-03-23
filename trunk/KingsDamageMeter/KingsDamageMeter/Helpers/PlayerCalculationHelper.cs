using System.Collections.Generic;
using System.Linq;
using KingsDamageMeter.Controls;

namespace KingsDamageMeter.Helpers
{
    public static class PlayerCalculationHelper
    {
        public static void CalculateTopDamagePercents(IEnumerable<Player> players)
        {
            if (players.Count() == 0)
            {
                return;
            }

            var topDamagePlayer = players.Where(o => o.Damage == players.Max(x => x.Damage)).First();
            if (topDamagePlayer.Damage > 0)
            {
                topDamagePlayer.PercentFromTopDamage = 1;
                foreach (var player in players)
                {
                    if (topDamagePlayer != player)
                    {
                        player.PercentFromTopDamage = (double)player.Damage / topDamagePlayer.Damage;
                    }
                }
            }
        }

        public static void CalculateGroupDamagePercents(IEnumerable<Player> players)
        {
            long total = players.Sum(o => o.Damage);
            if (total > 0)
            {
                foreach (Player p in players)
                {
                    p.PercentFromGroupDamages = (double)p.Damage / total;
                }
            }
        }

        public static void CalculateDps(IEnumerable<Player> players, double totalTime)
        {
            foreach (Player p in players)
            {
                p.DamagePerSecond = (int)(p.Damage / totalTime);
            }
        }

    }
}
