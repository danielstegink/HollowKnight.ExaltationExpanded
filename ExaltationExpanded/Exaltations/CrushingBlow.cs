using HutongGames.PlayMaker;
using Modding;
using System.Linq;
using UnityEngine;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Heavy Blow
    /// </summary>
    public class CrushingBlow : Exaltation
    {
        public override string Name => "Crushing Blow";
        public override string Description => "Formed from the hammer of an angry god.\n\n" +
                                                "Increases the force of the bearer's nail, causing enemies to recoil further when hit.\n\n" +
                                                "Additionally has the power to cripple great foes.";
        public override string ID => "15";

        public override string GodText => "god of the downtrodden";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.statueStateFalseKnight.completedTier3;
        }

        public override void Upgrade()
        {
            base.Upgrade();
            ModHooks.OnEnableEnemyHook += ExtraStagger;
        }

        public override void Reset()
        {
            base.Reset();
            ModHooks.OnEnableEnemyHook -= ExtraStagger;
        }

        /// <summary>
        /// Crushing Blow reduces the number of hits required to stagger bosses by an
        /// additional 1 point for a total of 2
        /// </summary>
        /// <param name="enemy"></param>
        /// <param name="isAlreadyDead"></param>
        /// <returns></returns>
        private bool ExtraStagger(GameObject enemy, bool isAlreadyDead)
        {
            //SharedData.Log($"Enabling enemy: {enemy.name}");

            // If the enemy is already dead, no need to adjust the stagger rate
            if (isAlreadyDead)
            {
                //SharedData.Log("This enemy is already dead.");
                return isAlreadyDead;
            }

            // Get the Stun or Stun Control FSM
            PlayMakerFSM stunFsm = enemy.LocateMyFSM("Stun");
            if (stunFsm == null)
            {
                stunFsm = enemy.LocateMyFSM("Stun Control");
                if (stunFsm == null)
                {
                    //SharedData.Log("This enemy cannot be staggered.");
                    return isAlreadyDead;
                }
            }
            
            // Get the Heavy Blow state of the FSM
            FsmState heavyBlowState = stunFsm.FsmStates.FirstOrDefault(x => x.Name.Equals("Heavy Blow"));
            if (heavyBlowState == default)
            {
                //SharedData.Log("This enemy's stagger is not affected by Heavy Blow?");
                return isAlreadyDead;
            }

            // If Heavy Blow is equipped, reduce the number of hits required to stagger the enemy
            if (PlayerData.instance.equippedCharm_15)
            {
                // The FSM will have 2 values of note: Stun Combo and Stun Hit Max. Both need to be reduced
                stunFsm.FsmVariables.FindFsmInt("Stun Hit Max").Value -= 1;
                stunFsm.FsmVariables.FindFsmInt("Stun Combo").Value -= 1;

                //SharedData.Log($"New stun values set: max hits {stunFsm.FsmVariables.FindFsmInt("Stun Hit Max").Value}, " +
                //    $"combo max {stunFsm.FsmVariables.FindFsmInt("Stun Combo").Value}");
            }

            return isAlreadyDead;
        }

    }
}