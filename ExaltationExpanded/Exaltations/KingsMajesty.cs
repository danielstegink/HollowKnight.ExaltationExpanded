using DanielSteginkUtils.Utilities;
using Modding;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// Replaces Royal Crest and King's Honour
    /// </summary>
    public class KingsMajesty : RoyalCrest
    {
        public override string Name => "King's Majesty";
        public override string Description => "Symbolizes the loyalty and kindness of two of the Pale King's greatest knights.\n\n" +
                                                "Shrouds the bearer in a regal and overwhelming aura.";

        public override string GodText => "gods of moss and honour";

        public override bool CanUpgrade()
        {
            if (SharedData.paleCourt.IsEnabled())
            {
                object saveSettings = ClassIntegrations.GetProperty<IMod, object>(SharedData.paleCourt.paleCourtMod, "SaveSettings");
                return ClassIntegrations.GetField<object, bool>(saveSettings, "upgradedCharm_10");
            }
            else if (SharedData.paleCourt.pcCharmsMod != null) // King's Majesty is eligible for PC Charms
            {
                object saveSettings = ClassIntegrations.GetProperty<IMod, object>(SharedData.paleCourt.pcCharmsMod, "localSettings");
                return ClassIntegrations.GetField<object, bool>(saveSettings, "upgradedCharm_10");
            }

            return false;
        }
    }
}