using Modding;
using UnityEngine;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Dreamshield
    /// </summary>
    public class Mothshield : Exaltation
    {
        public override string Name { get; set; } = "Mothshield";
        public override string Description { get; set; } = "Defensive charm once wielded by a hermetic god.\n\n" +
                                                            "Conjures two shields that follow the bearer and attempt to protect them.";
        public override string ID { get; set; } = "38";
        public override string GodText { get; set; } = "god of isolation";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.statueStateMarkoth.completedTier2;
        }

        public override void Equip()
        {
            base.Equip();
            ModHooks.ObjectPoolSpawnHook += SpawnExtraShield;
        }

        public override void Unequip()
        {
            base.Unequip();
            ModHooks.ObjectPoolSpawnHook -= SpawnExtraShield;
        }

        /// <summary>
        /// Mothshield spawns a second shield
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        private GameObject SpawnExtraShield(GameObject gameObject)
        {
            if (gameObject.name.Contains("Shield"))
            {
                GameObject shield = GameObject.Instantiate(gameObject, gameObject.transform.position, Quaternion.identity);
                shield.transform.Rotate(0, 0, 180);
            }

            return gameObject;
        }
    }
}
