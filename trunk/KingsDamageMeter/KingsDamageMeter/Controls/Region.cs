using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using KingsDamageMeter.Helpers;

namespace KingsDamageMeter.Controls
{
    public class Region : NotifyPropertyChangedBase, IEncounter
    {
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }

        private bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                if (isExpanded != value)
                {
                    isExpanded = value;
                    NotifyPropertyChanged("IsExpanded");
                }
            }
        }
        
        public string Name { get; set; }
        public ObservableCollection<Encounter> Encounters { get; private set; }
        public ICollection<Player> Players
        {
            get
            {
                var players = new Dictionary<string, Player>();
                double totalTime = 0;
                foreach (var encounter in Encounters)
                {
                    totalTime += encounter.Time;
                    foreach (var player in encounter.Players)
                    {
                        Player findedPlayer;
                        if(!players.TryGetValue(player.PlayerName, out findedPlayer))
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
                        foreach (var skill in player.Skills)
                        {
                            findedPlayer.Skills.Incriment(skill);
                        }


                        //Restore currect biggest hit
                        if(player.BiggestHit > biggestHit)
                        {
                            findedPlayer.BiggestHit = player.BiggestHit;
                        }
                        else
                        {
                            findedPlayer.BiggestHit = biggestHit;
                        }
                    }
                }
                
                PlayerCalculationHelper.CalculateTopDamagePercents(players.Values);
                PlayerCalculationHelper.CalculateGroupDamagePercents(players.Values);
                if(totalTime > 0)
                {
                    PlayerCalculationHelper.CalculateDps(players.Values, totalTime);
                }
                
                return players.Values;
            }
        }

        public Region()
        {
            Encounters = new ObservableCollection<Encounter>();
        }

    }
}
