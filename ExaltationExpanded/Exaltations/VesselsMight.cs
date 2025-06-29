using System;
using System.Collections.Generic;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Unbreakable Strength
    /// </summary>
    public class VesselsMight : Exaltation
    {
        public override string Name => "Vessel's Might";

        public override string Description => "Embodies the strength of the Hollow Knight's sacrifice. \n\n" +
                                                "Unites the bearer with their spirit, increasing their strength and infusing their attacks with SOUL.";
        public override string ID => "25_G";

        public override int IntID => 25;

        public override string GodText => "god of nothingness";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.bossDoorStateTier4.boundNail;
        }

        public override void Upgrade()
        {
            base.Upgrade();

            On.HealthManager.TakeDamage += BuffNail;
        }

        public override void Reset()
        {
            base.Reset();
            On.HealthManager.TakeDamage -= BuffNail;
        }

        /// <summary>
        /// Vengeful Spirit does 15 damage for 33 SOUL, while Shade Soul does 30 damage
        /// Vessel's Might will deal 1 damage for every 2 SOUL spent, maximum 4 damage at 8 soul per attack
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="hitInstance"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void BuffNail(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            bool isNailAttack = SharedData.nailAttackNames.Contains(hitInstance.Source.name) ||
                                SharedData.nailArtNames.Contains(hitInstance.Source.name) ||
                                hitInstance.Source.name.Contains("Grubberfly");

            if (PlayerData.instance.equippedCharm_25 &&
                isNailAttack)
            {
                // Get how much SOUL we have to work with
                int maxSoul = Math.Min(8, PlayerData.instance.MPCharge);

                // Calculate how much SOUL we use, and how much damage it deals
                int bonusDamage = 0;
                int soulUsed = 0;
                while (maxSoul >= 2)
                {
                    maxSoul -= 2;
                    soulUsed += 2;
                    bonusDamage += 1;
                }

                HeroController.instance.TakeMP(soulUsed);
                hitInstance.DamageDealt += bonusDamage;
                //SharedData.Log($"Nail damage increased by {bonusDamage} in exchange for {soulUsed} SOUL");
            }

            orig(self, hitInstance);
        }
    }
}
