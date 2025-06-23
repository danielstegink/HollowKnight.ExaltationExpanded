namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Shape of Unn
    /// </summary>
    public class BlessingOfUnn : Exaltation
    {
        public override string Name => "Blessing of Unn";
        public override string Description => "This charm contains of true essence of Unn.\n\n" +
                                                "While focusing SOUL, the bearer takes on a new shape, can move freely, and heals more efficiently.";
        public override string ID => "28";

        public override string GodText => "god of creation";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.bossDoorStateTier2.boundCharms;
        }

        public override void Upgrade()
        {
            base.Upgrade();
            On.HeroController.AddHealth += BuffHealAmount;
        }

        public override void Reset()
        {
            base.Reset();
            On.HeroController.AddHealth -= BuffHealAmount;
        }

        /// <summary>
        /// Blessing of Unn adds a 50% chance of healing being doubled
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="amount"></param>
        private void BuffHealAmount(On.HeroController.orig_AddHealth orig, HeroController self, int amount)
        {
            if (PlayerData.instance.equippedCharm_28)
            {
                int random = UnityEngine.Random.Range(1, 101);
                if (random <= 50)
                {
                    amount *= 2;
                }
            }

            orig(self, amount);
        }
    }
}