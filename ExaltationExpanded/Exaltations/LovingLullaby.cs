using GlobalEnums;
using Modding;
using System;
using UnityEngine;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    ///  This exaltation replaces Carefree Melody
    /// </summary>
    public class LovingLullaby : Exaltation
    {
        public override string Name => "Loving Lullaby";

        public override string Description => "Contains a song that promises sweet dreams and happiness for all.";

        public override string ID => GetMelodyId();

        public override string GodText => "god of the troupe";

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
                return SharedData.carefreeGrimmId.ToString();
            }
        }

        public override bool CanUpgrade()
        {
            return PlayerData.instance.statueStateGrimm.completedTier2 &&
                PlayerData.instance.grimmChildLevel >= 4;
        }

        public override void Upgrade()
        {
            ModHooks.TakeHealthHook += BuffMelody;
        }

        public override void Reset()
        {
            ModHooks.TakeHealthHook -= BuffMelody;
        }

        /// <summary>
        /// LL adds a flat 10% chance of negating damage taken
        /// </summary>
        /// <param name="damage"></param>
        /// <returns></returns>
        private int BuffMelody(int damage)
        {
            if (damage > 0 &&
                IsEquipped())
            {
                GameObject carefreeShield = HeroController.instance.carefreeShield;
                if (carefreeShield != null)
                {
                    int random = UnityEngine.Random.Range(1, 101);
                    //SharedData.Log($"CM detected. Dice rolled: {random} out of 100");
                    if (random <= 10)
                    {
                        //SharedData.Log("LL triggered. Damage negated");
                        carefreeShield.SetActive(true);
                        damage = 0;
                    }
                }
            }

            return damage;
        }

        /// <summary>
        /// Checks if Loving Lullaby is equipped
        /// </summary>
        /// <returns></returns>
        private bool IsEquipped()
        {
            if (GetMelodyId().Equals("40"))
            {
                return PlayerData.instance.equippedCharm_40;
            }
            else
            {
                return PlayerData.instance.GetBool($"equippedCharm_{SharedData.carefreeGrimmId}");
            }
        }
    }
}
