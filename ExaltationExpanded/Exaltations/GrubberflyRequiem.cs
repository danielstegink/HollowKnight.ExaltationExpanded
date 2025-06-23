using GlobalEnums;
using HKMirror.Reflection.SingletonClasses;
using Modding;
using UnityEngine;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Grubberfly's Elegy
    /// </summary>
    public class GrubberflyRequiem : Exaltation
    {
        public override string Name => "Grubberfly's Requiem";

        public override string Description => "Contains the fury of the grubberfly. Imbues weapons with a holy strength.\n\n" +
                                                "The bearer will fire beams of white-hot energy from their nail.";
        public override string ID => "35";

        public override string GodText => "gods of brotherhood";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.bossDoorStateTier1.boundNail;
        }

        public override void Upgrade()
        {
            base.Upgrade();
            On.HeroController.Attack += KeepGrubberfly;
        }

        public override void Reset()
        {
            base.Reset();
            On.HeroController.Attack -= KeepGrubberfly;
        }

        /// <summary>
        /// Elegy's energy attack is allowed on 1 of 2 conditions:
        /// 1) Player has max health and Joni's Blessing isn't equipped, or
        /// 2) Joni's Blessing is equipped and joniBeam is set to true
        /// I'd rather not tamper with the Player's health values
        /// So instead I will briefly simulate condition 2, trigger the attack, then reset everything
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="attackDir"></param>
        private void KeepGrubberfly(On.HeroController.orig_Attack orig, HeroController self, AttackDirection attackDir)
        {
            // Cache the player data
            bool origJoniBeam = HeroControllerR.joniBeam;
            bool origEquip27 = PlayerData.instance.equippedCharm_27;

            // Set conditions for elegy beam
            if (PlayerData.instance.equippedCharm_35)
            {
                HeroControllerR.joniBeam = true;
                PlayerData.instance.equippedCharm_27 = true;
            }

            // Perform the attack
            orig(self, attackDir);

            // Reset the player data
            HeroControllerR.joniBeam = origJoniBeam;
            PlayerData.instance.equippedCharm_27 = origEquip27;
        }
    }
}
