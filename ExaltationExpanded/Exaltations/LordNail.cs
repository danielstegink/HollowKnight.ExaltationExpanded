using ExaltationExpanded.Helpers;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Longnail
    /// </summary>
    public class LordNail : Exaltation
    {
        public override string Name { get; set; } = "Lord Nail";
        public override string Description { get; set; } = "The preferred weapon of those who rule the Mantis tribe.\n\n" +
                                                "Greatly increases the range of the bearer's nail, allowing them to strike foes from further away.";
        public override string ID { get; set; } = "18";

        public override string GodText { get; set; } = "gods of combat";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.statueStateMantisLords.completedTier2 || PlayerData.instance.bossDoorStateTier2.boundNail;
        }

        public override void Equip()
        {
            base.Equip();

            helper = new LordNailHelper();
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
        private LordNailHelper helper;
    }
}