using Modding;
using System;
using UnityEngine;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    ///  This exaltation replaces Grimmchild
    /// </summary>
    public class Knightmare : Exaltation
    {
        public override string Name => "Knightmare";

        public override string Description => "Symbol of a transformed ritual.\n\n" +
                                                "Contains a living, scarlet flame.";

        public override string ID => GetGrimmchildId();

        public override string GodText => "god of the troupe";

        /// <summary>
        /// Determines the current ID for Grimmchild
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private string GetGrimmchildId()
        {
            // Check if the default charm is Grimmchild
            if (PlayerData.instance.grimmChildLevel < 5)
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
            ModHooks.ObjectPoolSpawnHook += BuffGrimmchild;
        }

        public override void Reset()
        {
            ModHooks.ObjectPoolSpawnHook -= BuffGrimmchild;
        }

        /// <summary>
        /// Knightmare increases the Grimmchild's attack speed by 20%
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private GameObject BuffGrimmchild(GameObject gameObject)
        {
            if (gameObject.name.Equals("Grimmchild(Clone)"))
            {
                //SharedData.Log($"Grimmchild found: {gameObject.name}");

                PlayMakerFSM fsm = FSMUtility.LocateFSM(gameObject, "Control");

                // The wait time between attacks is a float value in the No Target state
                float waitTime = SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.SetFloatValue>(fsm, "No Target", 0).floatValue.Value;
                SFCore.Utils.FsmUtil.GetAction<HutongGames.PlayMaker.Actions.SetFloatValue>(fsm, "No Target", 0).floatValue.Value = waitTime * 0.8f;
                //SharedData.Log("Grimmchild wait time reduced to 80%");
            }

            return gameObject;
        }
    }
}
