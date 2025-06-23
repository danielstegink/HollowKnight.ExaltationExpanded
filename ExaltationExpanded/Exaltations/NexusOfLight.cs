namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Deep Focus
    /// </summary>
    public class NexusOfLight : Exaltation
    {
        public override string Name => "Nexus Of Light";
        public override string Description => "This broken mask attracts a strange, empowering light.\n\n" +
                                                "The bearer will focus SOUL at a slower rate, but the healing effect will triple.";
        public override string ID => "34";

        public override string GodText => "empty god";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.statueStateBrokenVessel.completedTier2 || PlayerData.instance.bossDoorStateTier2.boundShell;
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
        /// Nexus of Light increases the healing done while Deep Focus is equipped by 50%
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="amount"></param>
        private void BuffHealAmount(On.HeroController.orig_AddHealth orig, HeroController self, int amount)
        {
            if (PlayerData.instance.equippedCharm_34)
            {
                amount += amount / 2;
                //SharedData.Log($"Deep focus healing: {amount}");
            }

            orig(self, amount);
        }
    }
}