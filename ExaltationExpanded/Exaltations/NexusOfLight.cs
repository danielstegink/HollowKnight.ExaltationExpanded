using DanielSteginkUtils.Helpers.Attributes;
using DanielSteginkUtils.Utilities;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Deep Focus
    /// </summary>
    public class NexusOfLight : Exaltation
    {
        public override string Name { get; set; } = "Nexus Of Light";
        public override string Description { get; set; } = "This broken mask attracts a strange, empowering light.\n\n" +
                                                            "The bearer will focus SOUL at a slower rate, but the healing effect be greater.";
        public override string ID { get; set; } = "34";

        public override string GodText { get; set; } = "empty god";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.statueStateBrokenVessel.completedTier2 || 
                    PlayerData.instance.bossDoorStateTier2.boundShell;
        }

        public override void Equip()
        {
            base.Equip();

            healHelper = new HealHelper(GetHealingChance());
            healHelper.Start();
        }

        public override void Unequip()
        {
            base.Unequip();

            if (healHelper != null)
            {
                healHelper.Stop();
            }
        }

        /// <summary>
        /// Utils helper
        /// </summary>
        private HealHelper healHelper;

        /// <summary>
        /// Nexus of Light increases healing received
        /// </summary>
        /// <returns></returns>
        private int GetHealingChance()
        {
            // Per my Utils library, healing 1 extra mask is worth 8 notches
            // So for 2 notches, NOL should have a 25% chance of healing an extra mask
            return (int)(2 * 100 / NotchCosts.NotchesPerHeal());
        }
    }
}