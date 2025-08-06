using DanielSteginkUtils.Helpers.Charms;
using DanielSteginkUtils.Utilities;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Mark of Pride
    /// </summary>
    public class MarkOfBetrayal : Exaltation
    {
        public override string Name { get; set; } = "Mark of Betrayal";
        public override string Description { get; set; } = "The mark of one who betrayed their tribe for power.\n\n" +
                                                            "Greatly increases the range of the bearer's nail, and gives their attacks a chance to send shockwaves towards their enemies.";
        public override string ID { get; set; } = "13";
        public override string GodText { get; set; } = "god of anger";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.statueStateTraitorLord.completedTier2;
        }

        public override void Equip()
        {
            base.Equip();
            elegyBeamAttacker = new ElegyBeamAttacker(GetBeamChance());
            elegyBeamAttacker.Start();
        }

        public override void Unequip()
        {
            base.Unequip();
            if (elegyBeamAttacker != null)
            {
                elegyBeamAttacker.Stop();
            }
        }

        /// <summary>
        /// Utils folder
        /// </summary>
        private ElegyBeamAttacker elegyBeamAttacker;

        /// <summary>
        /// Mark of Betrayal has a chance to send Grubberfly's Elegy beam attacks when performing nail strikes
        /// </summary>
        /// <returns></returns>
        public int GetBeamChance()
        {
            // Per my Utils folder, GE would be worth 12 notches if it didn't require full health
            float totalValue = 3f * NotchCosts.FullHealthModifier();

            // That means for 2 notches, we can have a 1/6 chance of the beam triggering
            return (int)(2 * 100 / totalValue);
        }
    }
}