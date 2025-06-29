using System.Collections.Generic;

namespace ExaltationExpanded.Settings
{
    public class LocalSaveSettings
    {
        /// <summary>
        /// Stores which charms have already been exalted
        /// </summary>
        public List<string> Exalted = new List<string>();

        /// <summary>
        /// Stores if the second Nailsage's Glory charm is equipped
        /// </summary>
        public bool nsgEquipped = false;
    }
}
