using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using KingsDamageMeter.Helpers;
using KingsDamageMeter.Localization;
using KingsDamageMeter.Properties;

namespace KingsDamageMeter.Controls
{
    public class AllEncounters : Region
    {
        public override string Name
        {
            get
            {
                return WindowMainRes.AllEncounterName;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public ObservableCollection<Encounter> Data { get; private set; }
        
        public override ICollection<Player> Players
        {
            get
            {
                return PlayerCalculationHelper.CalculatePlayersData(Data);
            }
        }

        public AllEncounters()
        {
            Data = new ObservableCollection<Encounter>();
            Settings.Default.PropertyChanged += SettingsChanged;
        }

        private void SettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedLanguage")
            {
                NotifyPropertyChanged("Name");
            }
        }
    }
}