using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Dreamshield
    /// </summary>
    public class Mothshield : Exaltation
    {
        public override string Name => "Mothshield";
        public override string Description => "Defensive charm once wielded by a hermetic god.\n\n" +
                                                "Conjures two shields that follow the bearer and attempt to protect them.";
        public override string ID => "38";

        public override string GodText => "god of isolation";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.statueStateMarkoth.completedTier2;
        }

        public override void Upgrade()
        {
            base.Upgrade();

            ModHooks.ObjectPoolSpawnHook += SpawnExtraShield;
        }

        public override void Reset()
        {
            base.Reset();
            ModHooks.ObjectPoolSpawnHook -= SpawnExtraShield;
        }

        /// <summary>
        /// Mothshield spawns a second shield on the opposite side
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        private GameObject SpawnExtraShield(GameObject gameObject)
        {
            if (gameObject.name.Contains("Shield"))
            {
                GameObject extraShield = GameObject.Instantiate(gameObject,
                    new Vector3(HeroController.instance.transform.GetPositionX(),
                    HeroController.instance.transform.GetPositionY()),
                    Quaternion.identity);
                extraShield.transform.Rotate(0, 0, 180);
            }

            return gameObject;
        }
    }
}
