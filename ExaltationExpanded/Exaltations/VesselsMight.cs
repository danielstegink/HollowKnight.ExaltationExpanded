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
        /// Vessel's Might uses SOUL to increase damage dealt by nail attacks
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
                // Calculate how much SOUL to spend and how much damage its worth
                int soulUsed = GetSoul();
                float damagePerSoul = GetDamagePerSoul();
                int damage = (int)Math.Floor(soulUsed * damagePerSoul);

                HeroController.instance.TakeMP(soulUsed);
                hitInstance.DamageDealt += damage;
                //SharedData.Log($"Nail damage increased by {damage} in exchange for {soulUsed} SOUL");
            }

            orig(self, hitInstance);
        }


        /// <summary>
        /// Get how much SOUL to spend on the attack
        /// </summary>
        /// <returns></returns>
        private int GetSoul()
        {
            // Vessel's Might essentially adds a Spell cast to our attack
            // For 3 notches, Shaman Stone and Quick Slash both increase DPS
            //      by 40%, so for 1 notch we should aim for a 13% increase
            //      in DPS, cast 13% of a Spell
            float baseSoul = 33f;
            int maxSoul = (int)Math.Floor(baseSoul * 0.4f / 3f);

            // Remember to not use more SOUL than we have
            return Math.Min(maxSoul, PlayerData.instance.MPCharge);
        }

        /// <summary>
        /// Get how much damage to deal per SOUL used
        /// </summary>
        /// <returns></returns>
        private float GetDamagePerSoul()
        {
            // The most similar spell to nail attacks is arguably 
            //      Dive because its I-Frames make it easy to use
            // D-Dive has 3 parts that ultimately deal about 60
            //      damage to whoever we land on, so we'll use that
            //      as our point of reference.
            float baseDamage = 60;
            float soulSpent = 33;
            return baseDamage / soulSpent;
        }
    }
}
