using System;
using System.Timers;
using System.Windows;
using KingsDamageMeter.Helpers;

namespace KingsDamageMeter.Controls
{
    public class Encounter : EncounterBase
    {
        public Region Parent { get; private set; }

        private double time;
        public double Time
        {
            get { return time; }
            private set
            {
                if(time != value)
                {
                    time = value;
                    NotifyPropertyChanged("Time");

                    PlayerCalculationHelper.CalculateDps(Players, Time);
                }
            }
        }

        private bool isEnded;
        public bool IsEnded
        {
            get { return isEnded; }
            private set
            {
                if(isEnded != value)
                {
                    isEnded = value;
                    NotifyPropertyChanged("IsEnded");
                }
            }
        }

        private readonly DateTime startEncounterTime;
        private readonly Timer updateDataTimer = new Timer(5000);
        public Encounter(Region parent)
        {
            Parent = parent;
            Parent.Encounters.Add(this);

            startEncounterTime = DateTime.Now;
            updateDataTimer.Elapsed += delegate
                                           {
                                               updateDataTimer.Stop();
                                               UpdateData();
                                           };
        }

        public void EndEncounter(DateTime encounterEndTime)
        {
            IsEnded = true;
            Time = (encounterEndTime - startEncounterTime).TotalSeconds;
            Parent.UpdateData();
        }

        public override void UpdateData()
        {
            // Think timer is elapsed while exit. This should be fix the problem.
            if (Application.Current == null)
            {
                return;
            }
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action(UpdateData));
            }
            else
            {
                Time = (LastDamageInflictedTime - startEncounterTime).TotalSeconds;
                UpdateSort();
                Parent.UpdateData();
            }
        }

        public override void UpdatePlayerDamage(string playerName, int damage, string skill, bool isGroupMember)
        {
            base.UpdatePlayerDamage(playerName, damage, skill, isGroupMember);
            Parent.UpdatePlayerDamage(playerName, damage, skill, isGroupMember);
            if (!updateDataTimer.Enabled)
            {
                updateDataTimer.Start();
            }
        }

        public override void UpdatePlayerReceivedDamage(string playerName, int damage)
        {
            base.UpdatePlayerReceivedDamage(playerName, damage);
            Parent.UpdatePlayerReceivedDamage(playerName, damage);
            if (!updateDataTimer.Enabled)
            {
                updateDataTimer.Start();
            }
        }
    }
}
