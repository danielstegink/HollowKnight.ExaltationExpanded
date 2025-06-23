using Modding;
using System.Collections.Generic;
using UnityEngine;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Defender's Crest
    /// </summary>
    public class RoyalCrest : Exaltation
    {
        public override string Name => "Royal Crest";
        public override string Description => "The crest of a loyal knight of the Pale King.\n\n" +
                                                "Causes the bearer to emit a powerful odour.";
        public override string ID => "10";

        public override string GodText => "god of honour";

        /// <summary>
        /// Stores the original stats of the dung cloud
        /// </summary>
        Dictionary<string, float> dungStats = new Dictionary<string, float>();

        public override bool CanUpgrade()
        {
            return PlayerData.instance.statueStateDungDefender.completedTier2;
        }

        public override void Upgrade()
        {
            base.Upgrade();
            ModHooks.ObjectPoolSpawnHook += BuffDungCloud;
        }

        public override void Reset()
        {
            base.Reset();
            ModHooks.ObjectPoolSpawnHook -= BuffDungCloud;
        }

        /// <summary>
        /// Royal Crest increases the damage rate and size of Defender's Crest by 10%
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        private GameObject BuffDungCloud(GameObject gameObject)
        {
            if (gameObject.name.Equals("Knight Dung Trail(Clone)") &&
                PlayerData.instance.equippedCharm_10)
            {
                StoreOriginalStats(gameObject);

                gameObject.GetComponent<DamageEffectTicker>().damageInterval = dungStats["Dmg"] * 0.9f;

                gameObject.transform.localScale = new Vector3(dungStats["X"] * 1.1f, dungStats["Y"] * 1.1f);
            }

            return gameObject;
        }

        /// <summary>
        /// Stores the original stats for the dung cloud so that 
        /// we can safely upgrade it only once
        /// </summary>
        /// <param name="gameObject"></param>
        private void StoreOriginalStats(GameObject gameObject)
        {
            if (!dungStats.ContainsKey("X"))
            {
                dungStats.Add("X", gameObject.transform.localScale.x);
            }

            if (!dungStats.ContainsKey("Y"))
            {
                dungStats.Add("Y", gameObject.transform.localScale.y);
            }

            if (!dungStats.ContainsKey("Dmg"))
            {
                dungStats.Add("Dmg", gameObject.GetComponent<DamageEffectTicker>().damageInterval);
            }
        }
    }
}
