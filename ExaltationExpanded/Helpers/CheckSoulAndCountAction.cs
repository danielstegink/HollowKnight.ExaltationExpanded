using HutongGames.PlayMaker;
using System;
using UnityEngine;

namespace ExaltationExpanded.Helpers
{
    /// <summary>
    /// Custom FSM action for verifying if Primal Womb can hatch another hatchling
    /// </summary>
    public class CheckSoulAndCountAction : FsmStateAction
    {
        public override void OnEnter()
        {
            // If Charm Changer isn't installed, we have nothing to overwrite
            // If Glowing Womb isn't glorified, the bonus isn't active
            if (SharedData.charmChanger.charmChangerMod != null &&
                SharedData.exaltationMod.Settings.GlowingWombGlorified &&
                HaveEnoughSoul() &&
                MaxNotReached())
            {
                base.Fsm.Event("ExaltationExpanded");
            }

            // If we don't need to do any special logic, Finish the step and move on to the regular process
            Finish();
        }

        /// <summary>
        /// Checks if we have enough SOUL to summon a hatchling
        /// </summary>
        /// <returns></returns>
        private bool HaveEnoughSoul()
        {
            // 8 -> 4 means we want half the default cost
            int cost = 4;
            int charmChangerCost = SharedData.charmChanger.GetField<int>("glowingWombSpawnCost");
            if (SharedData.charmChanger.IsEnabled())
            {
                cost = charmChangerCost / 2;
            }
            //ExaltationExpanded.Instance.Log($"Primal Womb Patch - SOUL Cost set to {cost}");

            // If we can't afford the discounted price, no need to bother
            int currentMp = PlayerData.instance.GetInt("MPCharge");
            if (cost > currentMp)
            {
                return false;
            }

            // If Charm Changer costs more than the discounted price, we'll add some "temp" SOUL for it to take away
            // No need to go thru HeroController; this extra SOUL isn't really getting added to the player
            int soulToAdd = Math.Max(0, charmChangerCost - cost);
            PlayerData.instance.MPCharge += soulToAdd;
            //ExaltationExpanded.Instance.Log($"Primal Womb Patch - {soulToAdd} SOUL added to offset Charm Changer");

            return true;
        }

        /// <summary>
        /// Checks if we've reached the maximum number of hatchlings
        /// </summary>
        /// <returns></returns>
        private bool MaxNotReached()
        {
            // The default max is 4
            int max = 4;
            if (SharedData.charmChanger.IsEnabled())
            {
                max = SharedData.charmChanger.GetField<int>("glowingWombSpawnTotal");
            }

            // 4 -> 8 means we want double the default amount, but only if Power patch isn't turned on
            if (!SharedData.globalSettings.allowBalancePatch)
            {
                max *= 2;
            }

            int total;
            try
            {
                total = GameObject.FindGameObjectsWithTag("Knight Hatchling").Length;
            }
            catch // Possible that the game will return a null array instead of an empty one
            {
                total = 0;
            }

            // If we've hit our max, no need to proceed
            return total < max;
        }
    }
}
