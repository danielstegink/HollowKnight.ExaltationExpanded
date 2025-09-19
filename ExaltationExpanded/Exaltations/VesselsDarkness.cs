using DanielSteginkUtils.Utilities;
using Modding;
using System;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// Replaces Abyssal Bloom
    /// </summary>
    public class VesselsDarkness : Exaltation
    {
        public override string Name { get; set; } = "Vessel's Darkness";

        public override string Description { get; set; } = "Embodies the dark power of the Void within the Hollow Knight.\n\n" +
                                                            "The bearer gains overwhelming power as they draw nearer to death.";

        public override string GodText { get; set; } = "gods of the Pale Court";

        public override string ID { get => GetPaleCourtId(); set => throw new NotImplementedException(); }

        /// <summary>
        /// Stores the numeric ID of the Pale Court charm
        /// </summary>
        private int paleCourtId = -1;

        /// <summary>
        /// Gets the numeric ID of the charm
        /// </summary>
        /// <returns></returns>
        private string GetPaleCourtId()
        {
            if (SharedData.paleCourt.IsEnabled() &&
                paleCourtId < 0)
            {
                paleCourtId = SharedData.paleCourt.charmIds[3];
                //ExaltationExpanded.Instance.Log($"Pale Court - {Name} ID found: {paleCourtId}");
            }

            return paleCourtId.ToString();
        }

        public override bool CanUpgrade()
        {
            // If we're not using Pale Court, default to no
            if (!SharedData.paleCourt.IsEnabled())
            {
                return false;
            }

            // Abyssal Bloom is upgraded by completing the gauntlet
            object saveSettings = ClassIntegrations.GetProperty<IMod, object>(SharedData.paleCourt.paleCourtMod, "SaveSettings");
            int gauntletCount = ClassIntegrations.GetField<object, int>(saveSettings, "ChampionsCallClears");
            return gauntletCount > 0;
        }
    }
}