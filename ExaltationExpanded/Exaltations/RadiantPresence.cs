using DanielSteginkUtils.Utilities;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Dream Wielder
    /// </summary>
    public class RadiantPresence : Exaltation
    {
        public override string Name { get; set; } = "Radiant Presence";
        public override string Description { get; set; } = "Ephemeral manifestation of a higher being's power over dreams.\n\n" +
                                                            "Allows the bearer to charge the Dream Nail faster and collect much more SOUL when striking foes.\n\n" +
                                                            "Additionally, the bearer will be able to store excess SOUL to heal.";
        public override string ID { get; set; } = "30";
        public override string GodText { get; set; } = "god of betrayal";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.statueStateXero.completedTier3;
        }

        public override void Equip()
        {
            base.Equip();
            extraSoul = 0;
            On.EnemyDreamnailReaction.RecieveDreamImpact += RecieveDreamImpact;
        }

        public override void Unequip()
        {
            base.Unequip();
            On.EnemyDreamnailReaction.RecieveDreamImpact -= RecieveDreamImpact;
        }

        /// <summary>
        /// Handles the results of something being Dream Nailed by Radiant Presence
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        private void RecieveDreamImpact(On.EnemyDreamnailReaction.orig_RecieveDreamImpact orig, EnemyDreamnailReaction self)
        {
            //SharedData.Log("Applying Dream Nail");
            orig(self);

            int soulGained = GainSoul();
            StoreExtraSoul(soulGained);
            HealCheck();
        }

        /// <summary>
        /// Radiant Presence uses 1 notch to increase SOUL gained from Dream Nail
        /// </summary>
        private int GainSoul()
        {
            // Per my Utils, 1 notch is worth a 300% increase in SOUL gained from Dream Nail
            int bonus = (int)(NotchCosts.DreamNailSoulPerNotch() * 33);
            HeroController.instance.AddMPCharge(bonus);
            //SharedData.Log($"Total bonus SOUL: {bonusInt}");

            return bonus;
        }

        /// <summary>
        /// Stores excess SOUL for passive healing
        /// </summary>
        private int extraSoul = 0;

        /// <summary>
        /// Radiant Presence uses 1 notch to store excess SOUL gained from Dream Nail to passively heal
        /// </summary>
        /// <param name="bonusSoul"></param>
        private void StoreExtraSoul(int bonusSoul)
        {
            int currentSoul = PlayerValues.CurrentSoul();
            int maxSoul = PlayerValues.MaxSoul(true);
            if (currentSoul + bonusSoul > maxSoul)
            {
                extraSoul += currentSoul + bonusSoul - maxSoul;
            }
        }

        /// <summary>
        /// Checks if we have enough SOUL stored to heal, and uses it if eligible
        /// </summary>
        private void HealCheck()
        {
            int soulRequired = GetHealingRequirement();

            // Only trigger if the player is below max health
            while (extraSoul > soulRequired &&
                    PlayerData.instance.GetInt("health") < PlayerData.instance.GetInt("CurrentMaxHealth"))
            {
                extraSoul -= soulRequired;
                HeroController.instance.AddHealth(1);
                //SharedData.Log($"Healing requirement met. Remaining SOUL: {extraSoul}");
            }
        }

        /// <summary>
        /// Gets amount of SOUL required to heal
        /// </summary>
        /// <returns></returns>
        private int GetHealingRequirement()
        {
            // Per my Utils, healing an extra Mask is worth 8 notches,
            // so for 1 notch we could have a 1/8 chance of healing a Mask

            // Alternatively, we can just require 8 times the regular SOUL cost to trigger the healing
            return (int)(33 * NotchCosts.NotchesPerHeal());
        }
    }
}