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

        /// <summary>
        /// Stores if Lordsoul is equipped through Void Soul
        /// </summary>
        public bool voidSoulEquipped = false;

        /// <summary>
        /// Stores if the second CarefreeGrimm charm is equipped
        /// </summary>
        public bool knightmareLullabyEquipped = false;
    }
}