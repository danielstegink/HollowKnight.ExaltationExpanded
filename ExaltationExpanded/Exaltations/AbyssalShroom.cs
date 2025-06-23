using Modding;
using System.Collections.Generic;
using UnityEngine;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Spore Shroom
    /// </summary>
    public class AbyssalShroom : Exaltation
    {
        public override string Name => "Abyssal Shroom";
        public override string Description => "Composed of fungal matter enfused with Void. Scatters spores when exposed to SOUL.\n\n" +
                                                "When focusing SOUL, emit a large spore cloud that damages enemies.";
        public override string ID => "17";

        public override string GodText => "god of sages";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.statueStateElderHu.completedTier3 || PlayerData.instance.bossDoorStateTier3.boundCharms;
        }

        public override void Upgrade()
        {
            base.Upgrade();

            ModHooks.ObjectPoolSpawnHook += BuffShroom;
        }

        public override void Reset()
        {
            base.Reset();
            ModHooks.ObjectPoolSpawnHook -= BuffShroom;
        }

        /// <summary>
        /// Stores the original stats of the spore cloud
        /// </summary>
        Dictionary<string, float> sporeStats = new Dictionary<string, float>();

        /// <summary>
        /// Abyssal Shroom makes the spore cloud created by Spore Shroom 20% larger
        /// and more lethal
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        private GameObject BuffShroom(GameObject gameObject)
        {
            if (gameObject.name.Equals("Knight Spore Cloud(Clone)") &&
                PlayerData.instance.equippedCharm_17)
            {
                StoreOriginalStats(gameObject);

                gameObject.GetComponent<DamageEffectTicker>().damageInterval = sporeStats["Dmg"] * 0.8f;
                gameObject.transform.localScale = new Vector3(sporeStats["X"] * 1.2f, sporeStats["Y"] * 1.2f);
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
            if (!sporeStats.ContainsKey("X"))
            {
                sporeStats.Add("X", gameObject.transform.localScale.x);
            }

            if (!sporeStats.ContainsKey("Y"))
            {
                sporeStats.Add("Y", gameObject.transform.localScale.y);
            }

            if (!sporeStats.ContainsKey("Dmg"))
            {
                sporeStats.Add("Dmg", gameObject.GetComponent<DamageEffectTicker>().damageInterval);
            }
        }
    }
}
