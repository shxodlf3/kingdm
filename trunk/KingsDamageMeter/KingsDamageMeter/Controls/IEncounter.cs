using System.Collections.Generic;

namespace KingsDamageMeter.Controls
{
    public interface IEncounter
    {
        string Name { get; set; }
        ICollection<Player> Players { get; }
        //YouPlayer You { get; }

        /// <summary>
        /// This property is used for synchorize visual selection in TreeView
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// This property is used for synchorize visual expanding in TreeView
        /// </summary>
        bool IsExpanded { get; set; }
    }
}
