using DanielSteginkUtils.Utilities;
using Modding;
using System;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// Replaces Mark of Purity
    /// </summary>
    public class VesselsAssault : Exaltation
    {
        public override string Name { get; set; } = "Vessel's Assault";

        public override string Description { get; set; } = "This charm shines like an expertly crafted nail, symbolizing the Hollow Knight's speed and aggression.\n\n" +
                                                            "Hold ATTACK to concentrate and swing the nail in a frenzy. Gradually increases the bearer's rate of attack as they land nail strikes in quick succession.";

        public override string GodText { get; set; } = "god of Root and King";

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
                paleCourtId = SharedData.paleCourt.charmIds[0];
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

            object saveSettings = ClassIntegrations.GetProperty<IMod, object>(SharedData.paleCourt.paleCourtMod, "SaveSettings");
            object dryyaSettings = ClassIntegrations.GetField<object, object>(saveSettings, "CompletionDryya");
            return ClassIntegrations.GetField<object, bool>(dryyaSettings, "completedTier2");
        }
    }
}