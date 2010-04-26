/**************************************************************************\
 * 
    This file is part of KingsDamageMeter.

    KingsDamageMeter is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    KingsDamageMeter is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with KingsDamageMeter. If not, see <http://www.gnu.org/licenses/>.
 * 
\**************************************************************************/

using System;

namespace KingsDamageMeter.Controls
{
    public class YouPlayer : Player
    {
        private DateTime startTime = DateTime.Now;

        private int exp;
        public int Exp
        {
            get { return exp; }
            set
            {
                if (exp != value)
                {
                    exp = value;
                    NotifyPropertyChanged("Exp");
                    NotifyPropertyChanged("ExpPerHour");
                }
            }
        }

        public int ExpPerHour
        {
            get
            {
                if (Exp == 0)
                {
                    return 0;
                }
                TimeSpan span = DateTime.Now - startTime;
                return (int)((Exp / span.TotalSeconds) * 3600);
            }
        }

        private int kinahEarned;
        public int KinahEarned
        {
            get { return kinahEarned; }
            set
            {
                if (kinahEarned != value)
                {
                    kinahEarned = value;
                    NotifyPropertyChanged("KinahEarned");
                    NotifyPropertyChanged("TotalKinah");
                }
            }
        }

        private int kinahSpent;
        public int KinahSpent
        {
            get { return kinahSpent; }
            set
            {
                if (kinahSpent != value)
                {
                    kinahSpent = value;
                    NotifyPropertyChanged("KinahSpent");
                    NotifyPropertyChanged("TotalKinah");
                }
            }
        }

        public int TotalKinah
        {
            get
            {
                return KinahEarned + KinahSpent;
            }
        }

        private int ap;
        public int Ap
        {
            get { return ap; }
            set
            {
                if (ap != value)
                {
                    ap = value;
                    NotifyPropertyChanged("Ap");
                }
            }
        }

        public override void Reset()
        {
            base.Reset();

            Exp = 0;
            KinahEarned = 0;
            KinahSpent = 0;
            Ap = 0;

            startTime = DateTime.Now;
        }
    }
}
