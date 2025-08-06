using DanielSteginkUtils.Helpers.Charms.Shroom;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Spore Shroom
    /// </summary>
    public class AbyssalShroom : Exaltation
    {
        public override string Name { get; set; } = "Abyssal Shroom";
        public override string Description { get; set; } = "Composed of fungal matter enfused with Void. Scatters spores when exposed to SOUL.\n\n" +
                                                            "When focusing SOUL, emit a large spore cloud that damages enemies.";
        public override string ID { get; set; } = "17";

        public override string GodText { get; set; } = "god of sages";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.statueStateElderHu.completedTier3 || PlayerData.instance.bossDoorStateTier3.boundCharms;
        }

        /// <summary>
        /// Abyssal Shroom increases the size and damage rate of spore clouds
        /// </summary>
        public override void Equip()
        {
            base.Equip();
            sizeHelper = new SporeSizeHelper(SharedData.modName, Name, GetSizeModifier());
            sizeHelper.Start();

            damageHelper = new SporeDamageHelper(SharedData.modName, Name, 1 / GetDamageModifier());
            damageHelper.Start();
        }

        public override void Unequip()
        {
            base.Unequip();
            if (sizeHelper != null)
            {
                sizeHelper.Stop();
            }

            if (damageHelper != null)
            {
                damageHelper.Stop();
            }
        }

        /// <summary>
        /// Adjusts cloud size
        /// </summary>
        private SporeSizeHelper sizeHelper;

        /// <summary>
        /// Adjusts cloud damage
        /// </summary>
        private SporeDamageHelper damageHelper;

        /// <summary>
        /// If we spend 1 notch to increase the cloud size, we want to hit 100% more enemies, 
        /// so a 50% boost should be sufficent.
        /// </summary>
        /// <returns></returns>
        private float GetSizeModifier()
        {
            return 1.5f;
        }

        /// <summary>
        /// If we spend 1 notch to boost damage on a 1 notch charm, that will be worth a 100% increase
        /// </summary>
        /// <returns></returns>
        private float GetDamageModifier()
        {
            return 2f;
        }
    }
}
