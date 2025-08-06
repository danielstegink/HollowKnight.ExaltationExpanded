using DanielSteginkUtils.Helpers.Charms.Pets;
using System;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    ///  This exaltation replaces Grimmchild
    /// </summary>
    public class Knightmare : Exaltation
    {
        public override string Name { get; set; } = "Knightmare";
        public override string Description { get; set; } = "Symbol of a transformed ritual.\n\n" +
                                                            "Contains a living, scarlet flame.";
        public override string ID { get => GetGrimmchildId(); set => throw new NotImplementedException(); }
        public override string GodText { get; set; } = "god of the troupe";

        /// <summary>
        /// Determines the current ID for Grimmchild
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private string GetGrimmchildId()
        {
            // Check if the default charm is Grimmchild
            if (PlayerData.instance.GetInt("grimmChildLevel") < 5)
            {
                return "40";
            }
            else // Otherwise, check if there is a second charm ID
            {
                return SharedData.carefreeGrimmId.ToString();
            }
        }

        public override bool CanUpgrade()
        {
            return PlayerData.instance.statueStateGrimm.completedTier2 &&
                    PlayerData.instance.GetInt("grimmChildLevel") >= 4;
        }

        public override void Equip()
        {
            base.Equip();

            grimmchildHelper = new GrimmchildHelper(1f, 1 / GetModifier());
            grimmchildHelper.Start();
        }

        public override void Unequip()
        {
            base.Unequip();

            if (grimmchildHelper != null)
            {
                grimmchildHelper.Stop();
            }
        }

        /// <summary>
        /// Utils helper
        /// </summary>
        private GrimmchildHelper grimmchildHelper;

        /// <summary>
        /// Knightmare increases the attack speed of Grimmchild
        /// </summary>
        /// <returns></returns>
        private float GetModifier()
        {
            // Grimmchild costs 2 notches
            // So for 2 notches we can increase its damage rate by 100%
            return 2f;
        }
    }
}
