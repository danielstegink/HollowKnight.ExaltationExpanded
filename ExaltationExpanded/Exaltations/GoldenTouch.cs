using DanielSteginkUtils.Helpers.Attributes;
using DanielSteginkUtils.Utilities;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Unbreakable Greed
    /// </summary>
    public class GoldenTouch : Exaltation
    {
        public override string Name { get; set; } = "Golden Touch";
        public override string Description { get; set; } = "A symbol of wealth and ambition.\n\n" + 
                                                            "Causes the bearer to find more Geo.";
        public override string ID { get; set; } = "24_G";
        public override int IntID => 24;
        public override string GodText { get; set; } = "gods of brotherhood";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.bossDoorStateTier1.boundCharms;
        }

        public override void Equip()
        {
            base.Equip();
            helper = new GeoHelper(GetModifier());
            helper.Start();
        }

        public override void Unequip()
        {
            base.Unequip();
            if (helper != null)
            {
                helper.Stop();
            }
        }

        /// <summary>
        /// Utils helper
        /// </summary>
        private GeoHelper helper;

        /// <summary>
        /// Golden Touch increases geo gained from all sources
        /// </summary>
        /// <returns></returns>
        private float GetModifier()
        {
            // Per my Utils, extra Geo is worth 5% per notch
            // So for 2 notches, it is worth 10%
            return 1f + 2 * NotchCosts.GeoPerNotch();
        }
    }
}