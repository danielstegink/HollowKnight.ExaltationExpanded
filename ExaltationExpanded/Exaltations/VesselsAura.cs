using DanielSteginkUtils.Utilities;
using Modding;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// Replaces King's Majesty
    /// </summary>
    public class VesselsAura : KingsMajesty
    {
        public override string Name => "Vessel's Aura";
        public override string Description => "Contains an echo of the Hollow Knight's charisma.\n\n" +
                                                "Imbues the bearer with a powerful aura.";

        public override string GodText => "god of moss and grove";

        public override bool CanUpgrade()
        {
            if (SharedData.paleCourt.IsEnabled())
            {
                object saveSettings = ClassIntegrations.GetProperty<IMod, object>(SharedData.paleCourt.paleCourtMod, "SaveSettings");
                object ismaSettings = ClassIntegrations.GetField<object, object>(saveSettings, "CompletionIsma");
                return ClassIntegrations.GetField<object, bool>(ismaSettings, "completedTier2");
            }

            return false;
        }

        /// <summary>
        /// Vessel's Aura is an upgrade of King's Majesty, so it should spend 1 more notch on damage,
        /// and 1 more on size.
        /// 
        /// 2 extra notches worth of damage is a 200% increase, for a total of 300% the original damage.
        /// </summary>
        /// <returns></returns>
        protected override float GetDamageModifier()
        {
            return 1 + 2f / GetCharmCost();
        }

        /// <summary>
        /// 2 extra notches should hit a total of 300% the original number of enemies, so a 100% increase in size should do the trick
        /// </summary>
        /// <returns></returns>
        protected override float GetSizeModifier()
        {
            return 1 + 1f / GetCharmCost();
        }
    }
}