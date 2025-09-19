using DanielSteginkUtils.Helpers.Charms.Dung;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Defender's Crest
    /// </summary>
    public class RoyalCrest : Exaltation
    {
        public override string Name { get; set; } = "Royal Crest";
        public override string Description { get; set; } = "The crest of a loyal knight of the Pale King.\n\n" +
                                                            "Causes the bearer to emit a powerful odour.";
        public override string ID { get; set; } = "10";
        public override string GodText { get; set; } = "god of honour";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.statueStateDungDefender.completedTier2;
        }

        public override void Equip()
        {
            base.Equip();

            dungSizeHelper = new DungSizeHelper(ExaltationExpanded.Instance.Name, ID, GetSizeModifier());
            dungSizeHelper.Start();

            dungDamageHelper = new DungDamageHelper(ExaltationExpanded.Instance.Name, ID, GetDamageModifier());
            dungDamageHelper.Start();
        }

        public override void Unequip()
        {
            base.Unequip();

            if (dungSizeHelper != null)
            {
                dungSizeHelper.Stop();
            }

            if (dungDamageHelper != null)
            {
                dungDamageHelper.Stop();
            }
        }

        /// <summary>
        /// Adjusts cloud size
        /// </summary>
        private DungSizeHelper dungSizeHelper;

        /// <summary>
        /// Adjusts cloud damage
        /// </summary>
        private DungDamageHelper dungDamageHelper;

        /// <summary>
        /// If we spend 1 notch to increase the cloud size, we want to hit 100% more enemies, so a 50% boost should be sufficent
        /// </summary>
        /// <returns></returns>
        protected virtual float GetSizeModifier()
        {
            return 1 + 0.5f / GetCharmCost();
        }

        /// <summary>
        /// If we spend 1 notch to boost damage on a 1 notch charm, that will be worth a 100% increase
        /// </summary>
        /// <returns></returns>
        protected virtual float GetDamageModifier()
        {
            return 1 + 1f / GetCharmCost();
        }

        /// <summary>
        /// Gets the notch cost of the charm from Charm Changer
        /// </summary>
        /// <returns></returns>
        protected float GetCharmCost()
        {
            return SharedData.charmChanger.GetCharmNotches(IntID, PlayerData.instance.GetInt("charmCost_10"), 0.5f);
        }
    }
}
