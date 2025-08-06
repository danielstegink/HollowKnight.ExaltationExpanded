using DanielSteginkUtils.Helpers.Attributes;
using DanielSteginkUtils.Utilities;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Shape of Unn
    /// </summary>
    public class BlessingOfUnn : Exaltation
    {
        public override string Name { get; set; } = "Blessing of Unn";
        public override string Description { get; set; } = "This charm contains of true essence of Unn.\n\n" +
                                                            "While focusing SOUL, the bearer takes on a new shape, can move freely, and heals more efficiently.";
        public override string ID { get; set; } = "28";
        public override string GodText { get; set; } = "god of creation";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.bossDoorStateTier2.boundCharms;
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
        /// BOU increases healing received
        /// </summary>
        /// <returns></returns>
        private int GetHealingChance()
        {
            // Per my Utils library, healing 1 extra mask is worth 8 notches
            // So for 2 notches, BOU should have a 25% chance of healing an extra mask
            return (int)(2 * 100 / NotchCosts.NotchesPerHeal());
        }
    }
}