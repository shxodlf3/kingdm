using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private string name;
        public virtual string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public ObservableCollection<Encounter> Encounters { get; private set; }
        
        public virtual ICollection<Player> Players
        {
            get
            {
                return PlayerCalculationHelper.CalculatePlayersData(Encounters);
            }
        }

        public Region()
        {
            Encounters = new ObservableCollection<Encounter>();
        }

    }
}
