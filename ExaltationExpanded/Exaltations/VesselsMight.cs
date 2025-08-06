using DanielSteginkUtils.Helpers.Attributes;
using DanielSteginkUtils.Utilities;
using System;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Unbreakable Strength
    /// </summary>
    public class VesselsMight : Exaltation
    {
        public override string Name { get; set; } = "Vessel's Might";
        public override string Description { get; set; } = "Embodies the strength of the Hollow Knight's sacrifice. \n\n" +
                                                            "Unites the bearer with their spirit, increasing their strength and infusing their attacks with SOUL.";
        public override string ID { get; set; } = "25_G";
        public override int IntID => 25;
        public override string GodText { get; set; } = "god of nothingness";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.bossDoorStateTier4.boundNail;
        }

        public override void Equip()
        {
            base.Equip();
            On.HealthManager.TakeDamage += BuffNail;
        }

        public override void Unequip()
        {
            base.Unequip();
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
            if (Logic.IsNailAttack(hitInstance))
            {
                // Per my Utils, 2 notches is worth a 20% increase in Nail damage
                float nailDamage = PlayerData.instance.GetInt("nailDamage") * 2 * NotchCosts.NailDamagePerNotch();

                // VM uses SOUL to increase nail damage, so its like casting a spell on top of a nail attack
                // So we will take the nail damage above and convert it to Spell damage
                float maxSpellDamage = Calculations.NailDamageToSpellDamage(nailDamage);

                // Per Calculations, the most similar spell to nail attacks is Shriek, so we will use Shriek to 
                // calculate how much SOUL we should spend on our new spell damage
                float damagePerSoul = Calculations.DamagePerSoul(Calculations.SpellType.AbyssShriek);
                float soulPerDamage = 1 / damagePerSoul;
                float maxSoul = maxSpellDamage * soulPerDamage;
                int soulUsed = PlayerValues.SoulToSpend((int)maxSoul);

                // We then have to calculate how much damage we can do with the SOUL available and use it to launch a spell attack
                if (soulUsed > 0)
                {
                    float damage = soulUsed * soulPerDamage;
                    int damageInt = (int)damage;

                    // We will use the version that creates a separate object so that we don't create an infinite loop of buffed nail attacks
                    DamageHelper.DealDamage(self, damageInt, AttackTypes.Spell, "ExaltationExpanded.VesselsMight");
                    HeroController.instance.TakeMP(soulUsed);
                    //SharedData.Log($"Nail damage increased by {damageInt} in exchange for {soulUsed} SOUL");
                }
            }

            orig(self, hitInstance);
        }
    }
}