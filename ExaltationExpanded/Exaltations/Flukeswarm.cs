using HutongGames.PlayMaker;
using Modding;
using SFCore.Utils;
using System;
using System.Reflection;
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
            ModHooks.ObjectPoolSpawnHook += BuffFlukes;
        }

        public override void Reset()
        {
            base.Reset();
            ModHooks.ObjectPoolSpawnHook -= BuffFlukes;
        }

        /// <summary>
        /// Flukeswarm increases damage from Flukeswarm by 30%
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        private GameObject BuffFlukes(GameObject gameObject)
        {
            if (gameObject.name.StartsWith("Spell Fluke") &&
                gameObject.name.Contains("Clone"))
            {
                if (gameObject.name.Contains("Dung"))
                {
                    // At Lv 1, the Dung Fluke stores the dung cloud at step 8 of the Blow state in its Control FSM
                    int step = 8;
                    if (gameObject.name.Contains("Lv2")) // At level 2, the dung cloud is stored in step 10
                    {
                        step = 10;
                    }

                    // Get the Dung Fluke's Control FSM
                    PlayMakerFSM fsm = FSMUtility.LocateFSM(gameObject, "Control");

                    // Get the dung cloud object from the Blow state
                    FsmOwnerDefault fsmDungCloudOwner = fsm.GetAction<HutongGames.PlayMaker.Actions.ActivateGameObject>("Blow", step).gameObject;
                    FsmGameObject fsmDungCloud = fsmDungCloudOwner.GameObject;
                    GameObject dungCloudPrefab = fsmDungCloud.Value;
                    //SharedData.Log($"Dung Cloud Prefab found: {dungCloudPrefab.name}");

                    // We can't easily control how much damage the dung cloud does, but
                    // we CAN increase its damage rate, which is more reliable anyway
                    dungCloudPrefab.GetComponent<DamageEffectTicker>().damageInterval *= 0.7f;

                    // Then we just have to put the modified dung cloud back into the FSM
                    fsmDungCloud.Value = dungCloudPrefab;
                    fsmDungCloudOwner.GameObject = fsmDungCloud;
                    fsm.GetAction<HutongGames.PlayMaker.Actions.ActivateGameObject>("Blow", step).gameObject = fsmDungCloudOwner;
                }
                else
                {
                    SpellFluke fluke = gameObject.GetComponent<SpellFluke>();

                    // Fluke damage is stored in a private variable, so we need to
                    //  get the field the variable is stored in
                    FieldInfo damageField = fluke.GetType().GetField("damage");
                    int baseDamage = (int)damageField.GetValue(fluke);

                    double bonusDamage = Math.Min(1, baseDamage * 0.3);
                    damageField.SetValue(fluke, baseDamage + bonusDamage);
                }
            }

            return gameObject;
        }
    }
}