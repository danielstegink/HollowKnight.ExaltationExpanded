using DanielSteginkUtils.Utilities;
using System;
using DanielSteginkUtils.Helpers.Shields;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    ///  This exaltation replaces Carefree Melody
    /// </summary>
    public class LovingLullaby : Exaltation
    {
        public override string Name { get; set; } = "Loving Lullaby";
        public override string Description { get; set; } = "Contains a song that promises sweet dreams and happiness for all.";
        public override string ID { get => GetMelodyId(); set => throw new NotImplementedException(); }
        public override string GodText { get; set; } = "god of the troupe";

        /// <summary>
        /// Determines the current ID for Grimmchild
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private string GetMelodyId()
        {
            // Check if the default charm is Carefree Melody
            if (PlayerData.instance.grimmChildLevel == 5)
            {
                return "40";
            }
            else // Otherwise, check if there is a second charm ID
            {
                return SharedData.knightmareLullaby.charmId.ToString();
            }
        }

        public override bool CanUpgrade()
        {
            return PlayerData.instance.statueStateGrimm.completedTier2 &&
                PlayerData.instance.grimmChildLevel >= 4;
        }

        public override void Equip()
        {
            base.Equip();

            carefreeHelper = new CarefreeHelper(ShieldChance());
            carefreeHelper.Start();
        }

        public override void Unequip()
        {
            base.Unequip();

            if (carefreeHelper != null)
            {
                carefreeHelper.Stop();
            }
        }

        /// <summary>
        /// Utils helper
        /// </summary>
        private CarefreeHelper carefreeHelper;

        /// <summary>
        /// Loving Lullaby increases the chance of Carefree Melody triggering
        /// </summary>
        /// <param name="damage"></param>
        /// <returns></returns>
        private int ShieldChance()
        {
            // Per my Utils, blocking an attack is worth a 7.49% chance per notch
            // For 2 notches, thats a 15% chance
            float bonus = 2 * NotchCosts.ShieldChancePerNotch();

            // However, we are trying to augment a shield we have no control over, 
            // so we actually want to set up a second shield, and calculate what
            // its probability should be to function as an increase to the first shield
            return Calculations.GetSecondMelodyShield(bonus);
        }
    }
}
