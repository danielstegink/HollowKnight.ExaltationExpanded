using Modding;
using UnityEngine;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Weaversong
    /// </summary>
    public class BeastsCall : Exaltation
    {
        public override string Name { get; set; } = "Beast's Call";
        public override string Description { get; set; } = "This web-covered charm carries the cry of the Beast.\n\n" +
                                                            "Summons many weaverlings to give the bearer companionship and protection.";
        public override string ID { get; set; } = "39";

        public override string GodText { get; set; } = "god protector";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.statueStateHornet2.completedTier2;
        }

        public override void Equip()
        {
            base.Equip();
            ModHooks.ObjectPoolSpawnHook += SpawnWeaverlings;
        }

        public override void Unequip()
        {
            ModHooks.ObjectPoolSpawnHook -= SpawnWeaverlings;
        }

        /// <summary>
        /// Beast's Call increases the number of weaverlings created by Weaversong
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        private GameObject SpawnWeaverlings(GameObject gameObject)
        {
            if (gameObject.name.Contains("Weaverling"))
            {
                // Weaversong is worth 2 notches, so we can spend our 2 notches to double the number of weaverlings
                _ = GameObject.Instantiate(gameObject, 
                    new Vector3(HeroController.instance.transform.GetPositionX(), 
                    HeroController.instance.transform.GetPositionY()), 
                    Quaternion.identity);
            }

            return gameObject;
        }
    }
}