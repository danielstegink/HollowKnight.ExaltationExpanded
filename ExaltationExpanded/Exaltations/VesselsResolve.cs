using DanielSteginkUtils.Utilities;
using Modding;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// Replaces Vessel's Lament
    /// </summary>
    public class VesselsResolve : VesselsDuty
    {
        public override string Name { get; set; } = "Vessel's Resolve";

        public override string Description { get; set; } = "Symbolizes the Hollow Knight's resistance against the Radiance.\n\n" +
                                                            "Focus to twist the SOUL of enemies marked by the nail and unleash devastating blasts of energy upon them.\n\n" +
                                                            "Increases the amount of SOUL gained when striking an enemy with the nail.";

        public override string GodText { get; set; } = "god of a sacred land";

        public override bool CanUpgrade()
        {
            // If we're not using Pale Court, default to no
            if (!SharedData.paleCourt.IsEnabled())
            {
                return false;
            }

            object saveSettings = ClassIntegrations.GetProperty<IMod, object>(SharedData.paleCourt.paleCourtMod, "SaveSettings");
            object zemerSettings = ClassIntegrations.GetField<object, object>(saveSettings, "CompletionZemer2");
            return ClassIntegrations.GetField<object, bool>(zemerSettings, "completedTier2");
        }

        /// <summary>
        /// Vessel's Resolve spends 2 more notches than Vessel's Duty
        /// </summary>
        /// <returns></returns>
        protected override int GetSoulNotches()
        {
            return 3;
        }
    }
}