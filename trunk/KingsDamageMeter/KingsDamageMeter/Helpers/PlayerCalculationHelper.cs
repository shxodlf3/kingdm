using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public static IEnumerable<Player> CalculatePlayersData(IEnumerable<Encounter> encounters)
        {
            var players = new Dictionary<string, Player>();
            double totalTime = 0;
            foreach (var encounter in encounters)
            {
                totalTime += encounter.Time;
                foreach (var player in encounter.Players)
                {
                    IncrementPlayerData(players, player);
                }
            }

            CalculateTopDamagePercents(players.Values);
            CalculateGroupDamagePercents(players.Values);
            if (totalTime > 0)
            {
                CalculateDps(players.Values, totalTime);
            }

            return players.Values;
        }

        public static IEnumerable<Player> CalculatePlayersData(ObservableCollection<Region> regions)
        {
            var players = new Dictionary<string, Player>();
            double totalTime = 0;
            foreach (var region in regions)
            {
                foreach (var encounter in region.Encounters)
                {
                    totalTime += encounter.Time;
                    foreach (var player in encounter.Players)
                    {
                        IncrementPlayerData(players, player);
                    }
                }
            }

            CalculateTopDamagePercents(players.Values);
            CalculateGroupDamagePercents(players.Values);
            if (totalTime > 0)
            {
                CalculateDps(players.Values, totalTime);
            }

            return players.Values;
        }
        
        private static void IncrementPlayerData(IDictionary<string, Player> players, Player player)
        {
            Player findedPlayer;
            if (!players.TryGetValue(player.PlayerName, out findedPlayer))
            {
                findedPlayer = new Player
                {
                    PlayerName = player.PlayerName,
                    PlayerClass = player.PlayerClass,
                    IsGroupMember = player.IsGroupMember,
                    IsFriend = player.IsFriend,
                };
                players.Add(player.PlayerName, findedPlayer);
            }
            //Save BiggestHit, because increment damage will increment biggest hit
            int biggestHit = findedPlayer.BiggestHit;

            findedPlayer.Damage += player.Damage;
            findedPlayer.DamageTaken += player.DamageTaken;
            foreach (var skill in player.Skills)
            {
                findedPlayer.Skills.Incriment(skill);
            }


            //Restore currect biggest hit
            if (player.BiggestHit > biggestHit)
            {
                findedPlayer.BiggestHit = player.BiggestHit;
            }
            else
            {
                findedPlayer.BiggestHit = biggestHit;
            }
        }
    }
}
