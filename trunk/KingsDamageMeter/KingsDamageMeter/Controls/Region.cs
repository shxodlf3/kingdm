using System.Collections.ObjectModel;
using System.Collections.Specialized;
using KingsDamageMeter.Helpers;

namespace KingsDamageMeter.Controls
{
    public class Region : EncounterBase
    {
        public ObservableCollection<Encounter> Encounters { get; private set; }

        private AllEncounters Parent { get; set; }
        private bool IsPlayerRemovedInternally { get; set; }

        public Region(AllEncounters parent)
        {
            Encounters = new ObservableCollection<Encounter>();
            Encounters.CollectionChanged += OnEncountersCollectionChanged;
            Parent = parent;
            Parent.Regions.Add(this);
        }

        private void OnEncountersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if(e.NewItems != null)
                    {
                        foreach (Encounter encounter in e.NewItems)
                        {
                            encounter.Players.CollectionChanged += OnPlayersCollectionChanged;
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if(e.OldItems != null)
                    {
                        foreach (Encounter encounter in e.OldItems)
                        {
                            encounter.Players.CollectionChanged -= OnPlayersCollectionChanged;
                        }
                        RecalculatePlayersData();
                    }
                    break;
            }
        }

        private void OnPlayersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsPlayerRemovedInternally)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    //If player was deleted:
                    RecalculatePlayersData();
                    break;
            }
        }

        private void RecalculatePlayersData()
        {
            Players.Clear();
            var newPlayersData = PlayerCalculationHelper.CalculatePlayersData(Encounters);
            foreach (var player in newPlayersData)
            {
                Players.Add(player);
            }
            UpdateSort();
            UpdatePercents();
            Parent.RecalculatePlayersData();
        }

        public override void UpdateData()
        {
            double totalTime = 0;
            foreach (var encounter in Encounters)
            {
                totalTime += encounter.Time;
            }
            PlayerCalculationHelper.CalculateDps(Players, totalTime);
            UpdateSort();
            Parent.UpdateData();
        }

        public override void RemovePlayer(string playerName)
        {
            try
            {
                IsPlayerRemovedInternally = true;

                base.RemovePlayer(playerName);
                foreach (var encounter in Encounters)
                {
                    encounter.RemovePlayer(playerName);
                }
                UpdatePercents();
            }
            finally
            {
                IsPlayerRemovedInternally = false;
            }
        }

        public override void UpdatePlayerDamage(string playerName, int damage, string skill, bool isGroupMember)
        {
            base.UpdatePlayerDamage(playerName, damage, skill, isGroupMember);
            Parent.UpdatePlayerDamage(playerName, damage, skill, isGroupMember);
        }

        public override void UpdatePlayerReceivedDamage(string playerName, int damage)
        {
            base.UpdatePlayerReceivedDamage(playerName, damage);
            Parent.UpdatePlayerReceivedDamage(playerName, damage);
        }

    }
}
