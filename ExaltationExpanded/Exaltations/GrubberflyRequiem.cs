using DanielSteginkUtils.Helpers.Charms;
using DanielSteginkUtils.Utilities;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Grubberfly's Elegy
    /// </summary>
    public class GrubberflyRequiem : Exaltation
    {
        public override string Name { get; set; } = "Grubberfly's Requiem";

        public override string Description { get; set; } = "Contains the fury of the grubberfly. Imbues weapons with a holy strength.\n\n" +
                                                            "The bearer may fire beams of white-hot energy from their nail.\n\n" +
                                                            "When the bearer is at full health, they will fire beams";
        public override string ID { get; set; } = "35";
        public override string GodText { get; set; } = "gods of brotherhood";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.bossDoorStateTier1.boundNail;
        }

        public override void Equip()
        {
            base.Equip();
            elegyBeamAttacker = new ElegyBeamAttacker(GetBeamChance());
            elegyBeamAttacker.Start();
        }

        public override void Unequip()
        {
            base.Unequip();
            if (elegyBeamAttacker != null)
            {
                elegyBeamAttacker.Stop();
            }
        }

        /// <summary>
        /// Utils helper
        /// </summary>
        private ElegyBeamAttacker elegyBeamAttacker;

        /// <summary>
        /// Grubberfly's Requiem has a chance to trigger even when the player isn't at full health
        /// </summary>
        /// <returns></returns>
        public int GetBeamChance()
        {
            // Per my Utils folder, GE would be worth 12 notches if it didn't require full health
            float totalValue = 3f * NotchCosts.FullHealthModifier();

            // That means for 2 notches, we can have a 1/6 chance of the beam triggering
            return (int)(2 * 100 / totalValue);
        }
    }
}
