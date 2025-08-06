using DanielSteginkUtils.Helpers.Attributes;
using DanielSteginkUtils.Utilities;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Heavy Blow
    /// </summary>
    public class CrushingBlow : Exaltation
    {
        public override string Name { get; set; } = "Crushing Blow";
        public override string Description { get; set; } = "Formed from the hammer of an angry god.\n\n" +
                                                            "Increases the force of the bearer's nail, causing enemies to recoil further when hit.\n\n" +
                                                            "Additionally has the power to cripple great foes.";
        public override string ID { get; set; } = "15";

        public override string GodText { get; set; } = "god of the downtrodden";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.statueStateFalseKnight.completedTier3;
        }

        public override void Equip()
        {
            base.Equip();
            helper = new StaggerHelper(GetStaggerCount(), GetStaggerCount() / 2);
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
        private StaggerHelper helper;

        /// <summary>
        /// Crushing Blow reduces the number of hits required to stagger bosses
        /// </summary>
        /// <returns></returns>
        private int GetStaggerCount()
        {
            // Per my Utils, 1 Stagger is worth 1 notch
            // So we can reduce the counts by 2 for 2 notches
            return 2 / NotchCosts.NotchesPerStagger();
        }
    }
}