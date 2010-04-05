using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using KingsDamageMeter.Helpers;
using KingsDamageMeter.Localization;
using KingsDamageMeter.Properties;

namespace KingsDamageMeter.Controls
{
    public class AllEncounters : EncounterBase
    {
        public ObservableCollection<Region> Regions { get; private set; }
        private bool IsPlayerRemovedInternally { get; set; }

        public AllEncounters()
        {
            Name = WindowMainRes.AllEncounterName;

            Regions = new ObservableCollection<Region>();
            Regions.CollectionChanged += OnRegionsCollectionChanged;
            Settings.Default.PropertyChanged += OnSettingsChanged;
        }

        private void OnRegionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    RecalculatePlayersData();
                    break;
            }
        }

        private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedLanguage")
            {
                Name = WindowMainRes.AllEncounterName;
                NotifyPropertyChanged("Name");
            }
        }

        public override void UpdateData()
        {
            double totalTime = 0;
            foreach (var region in Regions)
            {
                foreach (var encounter in region.Encounters)
                {
                    totalTime += encounter.Time;
                }
            }
            PlayerCalculationHelper.CalculateDps(Players, totalTime);
            UpdateSort();
        }

        public override void RemovePlayer(string playerName)
        {
            try
            {
                IsPlayerRemovedInternally = true;
                base.RemovePlayer(playerName);
                foreach (var region in Regions)
                {
                    region.RemovePlayer(playerName);
                }
                UpdatePercents();
            }
            finally
            {
                IsPlayerRemovedInternally = false;
            }
        }

        public void RecalculatePlayersData()
        {
            if(IsPlayerRemovedInternally)
            {
                return;
            }

            Players.Clear();
            var newPlayersData = PlayerCalculationHelper.CalculatePlayersData(Regions);
            foreach (var player in newPlayersData)
            {
                Players.Add(player);
            }
            UpdateSort();
            UpdatePercents();
        }
    }
}