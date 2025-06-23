using Modding;
using UnityEngine;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Weaversong
    /// </summary>
    public class BeastsCall : Exaltation
    {
        public override string Name => "Beast's Call";
        public override string Description => "This web-covered charm carries the cry of the Beast.\n\n" +
                                                "Summons many weaverlings to give the bearer companionship and protection.";
        public override string ID => "39";

        public override string GodText => "god protector";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.statueStateHornet2.completedTier2;
        }

        public override void Upgrade()
        {
            base.Upgrade();
            ModHooks.ObjectPoolSpawnHook += SpawnWeaverlings;
        }

        public override void Reset()
        {
            base.Reset();
            ModHooks.ObjectPoolSpawnHook -= SpawnWeaverlings;
        }

        /// <summary>
        /// Beast's Call doubles the number of weaverlings produced by Weaversong
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        private GameObject SpawnWeaverlings(GameObject gameObject)
        {
            if (gameObject.name.Contains("Weaverling"))
            {
                _ = GameObject.Instantiate(gameObject, 
                    new Vector3(HeroController.instance.transform.GetPositionX(), 
                    HeroController.instance.transform.GetPositionY()), 
                    Quaternion.identity);
            }

            return gameObject;
        }
    }
}
