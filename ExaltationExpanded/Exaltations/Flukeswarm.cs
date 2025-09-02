using DanielSteginkUtils.Helpers.Charms.Dung;
using DanielSteginkUtils.Helpers.Charms.Pets;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Flukenest
    /// </summary>
    public class Flukeswarm : Exaltation
    {
        public override string Name { get; set; } = "Flukeswarm";
        public override string Description { get; set; } = "Essence of a Flukemarm that has been consumed by the Void.\n\n" +
                                                            "Transforms the Vengeful Spirit spell into a horde of powerful baby flukes.";
        public override string ID { get; set; } = "11";
        public override string GodText { get; set; } = "god of motherhood";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.statueStateFlukemarm.completedTier3;
        }

        public override void Equip()
        {
            base.Equip();
            flukeHelper = new FlukeHelper(GetModifier());
            flukeHelper.Start();

            dungFlukeHelper = new DungFlukeHelper(ExaltationExpanded.Instance.Name, Name, 1 / GetModifier());
            dungFlukeHelper.Start();
        }

        public override void Unequip()
        {
            base.Unequip();
            if (flukeHelper != null)
            {
                flukeHelper.Stop();
            }

            if (dungFlukeHelper != null)
            {
                dungFlukeHelper.Stop();
            }
        }

        /// <summary>
        /// Helper for regular Flukes
        /// </summary>
        private FlukeHelper flukeHelper;

        /// <summary>
        /// Helper for Dung Flukes
        /// </summary>
        private DungFlukeHelper dungFlukeHelper;

        /// <summary>
        /// Flukeswarm increases damage dealt by Flukenest
        /// </summary>
        /// <returns></returns>
        private float GetModifier()
        {
            // Flukenest costs 3 notches
            // So for 2 notches, we can increase its damage by 67%
            float cost = SharedData.charmChanger.GetCharmNotches(IntID, PlayerData.instance.GetInt("charmCost_11"), 0.5f);
            return 1f + 2f / cost;
        }
    }
}