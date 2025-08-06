using DanielSteginkUtils.Utilities;
using System;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Unbreakable Heart
    /// </summary>
    public class CompleteMask : Exaltation
    {
        public override string Name { get; set; } = "Complete Mask";
        public override string Description { get; set; } = "An expertly carved mask, like so many others worn throughout Hallownest.\n\n" +
                                                            "Increases the health of the bearer, allowing them to take more damage.";
        public override string ID { get; set; } = "23_G";

        public override int IntID => 23;

        public override string GodText { get; set; } = "god of nothingness";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.bossDoorStateTier4.boundShell;
        }

        public override void Equip()
        {
            base.Equip();
            //On.HeroController.CharmUpdate += IncreaseHealth;
            On.HeroController.MaxHealth += IncreaseHealth;
        }

        public override void Unequip()
        {
            base.Unequip();
            //On.HeroController.CharmUpdate -= IncreaseHealth;
            On.HeroController.MaxHealth -= IncreaseHealth;
        }

        private void IncreaseHealth(On.HeroController.orig_MaxHealth orig, HeroController self)
        {
            PlayerData.instance.maxHealth += GetBonusHealth();
            orig(self);
        }

        /// <summary>
        /// Gets how much to increase our max health by
        /// </summary>
        /// <returns></returns>
        private int GetBonusHealth()
        {
            // Per my Utils library, 1 Mask is worth 2 notches
            // So for 2 notches we get 1 additional mask
            return Math.Max(1, (int)(2 / NotchCosts.NotchesPerMask()));
        }
    }
}