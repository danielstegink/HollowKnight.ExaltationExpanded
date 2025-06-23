using Modding;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Unbreakable Heart
    /// </summary>
    public class CompleteMask : Exaltation
    {
        /// <summary>
        /// Tracks the state of the charm: 0 means not applied, 1 means applied, 2 means broken
        /// </summary>
        private bool broken = false;

        public override string Name => "Complete Mask";
        public override string Description => "An expertly carved mask, like so many others worn throughout Hallownest.\n\n" +
                                                "Increases the health of the bearer, allowing them to take more damage.";
        public override string ID => "23_G";

        public override int IntID => 23;

        public override string GodText => "god of nothingness";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.bossDoorStateTier4.boundShell;
        }

        public override void Upgrade()
        {
            base.Upgrade();
            On.PlayerData.UpdateBlueHealth += ResetMask;
            ModHooks.TakeHealthHook += BreakMask;
            On.HeroController.AddHealth += HealMask; 
        }

        public override void Reset()
        {
            base.Reset();
            On.PlayerData.UpdateBlueHealth -= ResetMask;
            ModHooks.TakeHealthHook -= BreakMask;
            On.HeroController.AddHealth -= HealMask;
        }

        /// <summary>
        /// Complete Mask gives you a single blue mask
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        private void ResetMask(On.PlayerData.orig_UpdateBlueHealth orig, PlayerData self)
        {
            orig(self);
            if (PlayerData.instance.equippedCharm_23)
            {
                broken = false;
                self.healthBlue++;
            }
        }

        /// <summary>
        /// If the player takes damage, the mask becomes broken
        /// </summary>
        /// <param name="damage"></param>
        /// <returns></returns>
        private int BreakMask(int damage)
        {
            if (PlayerData.instance.equippedCharm_23 && 
                damage > 0)
            {
                broken = true;
            }

            return damage;
        }

        /// <summary>
        /// If the player heals past their max health, the mask is repaired and you 
        /// get the blue mask back. Doesn't work if Joni's Blessing is equipped
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="amount"></param>
        private void HealMask(On.HeroController.orig_AddHealth orig, HeroController self, int amount)
        {
            bool healMask = PlayerData.instance.equippedCharm_23 &&
                            broken &&
                            !PlayerData.instance.equippedCharm_27 &&
                            PlayerData.instance.health + amount > PlayerData.instance.maxHealth;

            orig(self, amount);

            if (healMask)
            {
                broken = false;
                EventRegister.SendEvent("ADD BLUE HEALTH");
            }
        }
    }
}