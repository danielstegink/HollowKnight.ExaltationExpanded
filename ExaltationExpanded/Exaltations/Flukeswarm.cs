using ExaltationExpanded.Helpers;
using Modding;
using System;
using UnityEngine;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Flukenest
    /// </summary>
    public class Flukeswarm : Exaltation
    {
        public override string Name => "Flukeswarm";
        public override string Description => "Essence of a Flukemarm that has been consumed by the Void.\n\n" +
                                                "Transforms the Vengeful Spirit spell into a horde of powerful baby flukes.";
        public override string ID => "11";
        public override string GodText => "god of motherhood";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.statueStateFlukemarm.completedTier3;
        }

        public override void Upgrade()
        {
            base.Upgrade();

            dungFlukeHelper = new DungFlukeHelper(1 - GetModifier());
            dungFlukeHelper.Start();
            ModHooks.ObjectPoolSpawnHook += BuffFlukes;
        }

        public override void Reset()
        {
            base.Reset();

            if (dungFlukeHelper != null)
            {
                dungFlukeHelper.Stop();
            }
            ModHooks.ObjectPoolSpawnHook -= BuffFlukes;
        }

        /// <summary>
        /// Used for handling damage buff for Dung Flukes
        /// </summary>
        private DungFlukeHelper dungFlukeHelper;

        /// <summary>
        /// Flukeswarm boosts damage from Flukes
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        private GameObject BuffFlukes(GameObject gameObject)
        {
            if (gameObject.name.StartsWith("Spell Fluke") &&
                gameObject.name.Contains("Clone"))
            {
                // Dung Flukes have to be handled separately
                if (!gameObject.name.Contains("Dung"))
                {
                    SpellFluke fluke = gameObject.GetComponent<SpellFluke>();

                    // Fluke damage is stored in a private variable, so we need to
                    //  get the field the variable is stored in
                    int baseDamage = SharedData.GetField<SpellFluke, int>(fluke, "damage");
                    int bonusDamage = (int)Math.Max(1, baseDamage * GetModifier());

                    SharedData.SetField(fluke, "damage", baseDamage + bonusDamage);
                    //SharedData.Log($"Fluke damage increased from {baseDamage} to {baseDamage + bonusDamage}");
                }
            }

            return gameObject;
        }

        /// <summary>
        /// A 1-notch boost would be about 30%
        /// </summary>
        /// <returns></returns>
        private float GetModifier()
        {
            return 0.3f;
        }
    }
}