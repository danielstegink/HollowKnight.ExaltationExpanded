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
            On.HeroController.CharmUpdate += IncreaseHealth;
        }

        public override void Unequip()
        {
            base.Unequip();
            On.HeroController.CharmUpdate -= IncreaseHealth;
        }

        /// <summary>
        /// Complete Mask increases the player's max health.
        /// 
        /// There are a lot of factors that affect max health, so the most bug-free approach is 
        /// to increment the health base before everything else changes the max health.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void IncreaseHealth(On.HeroController.orig_CharmUpdate orig, HeroController self)
        {
            PlayerData.instance.IntAdd("maxHealthBase", GetBonusHealth());
            orig(self);

            // Make sure to reset the base afterwards, so this doesn't stack
            PlayerData.instance.IntAdd("maxHealthBase", -GetBonusHealth());
            //SharedData.Log($"Complete Mask - Max health: {PlayerData.instance.maxHealth}");
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