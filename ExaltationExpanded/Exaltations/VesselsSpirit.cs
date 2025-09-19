using DanielSteginkUtils.Utilities;
using Modding;
using System;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// Replaces Boon of Hallownest
    /// </summary>
    public class VesselsSpirit : Exaltation
    {
        public override string Name { get; set; } = "Vessel's Spirit";

        public override string Description { get; set; } = "Contains the Hollow Knight's mastery of spellcraft.\n\n" +
                                                            "Transforms all spells to take on a purified form.";

        public override string GodText { get; set; } = "god of the kingdom's heart";

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
                paleCourtId = SharedData.paleCourt.charmIds[2];
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
            object hegemolSettings = ClassIntegrations.GetField<object, object>(saveSettings, "CompletionHegemol");
            return ClassIntegrations.GetField<object, bool>(hegemolSettings, "completedTier2");
        }
    }
}