using DanielSteginkUtils.Utilities;
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
            // If Charm Changer isn't overriding our changes, or
            // if Glowing Womb isn't exalted, or
            // if Power Patch is enabled to ignore cost/max,
            // then there's nothing to do here
            if (SharedData.charmChanger.charmChangerMod != null && 
                SharedData.exaltationMod.Settings.GlowingWombGlorified &&
                !SharedData.globalSettings.allowBalancePatch)
            {
                // 4 -> 8 means we want double the default amount
                int max = 8;
                if (SharedData.charmChanger.IsEnabled())
                {
                    max = 2 * SharedData.charmChanger.GetField<int>("glowingWombSpawnTotal");
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
                //ExaltationExpanded.Instance.Log($"Primal Womb Patch - Max: {max}, Total: {total}");
                if (total < max)
                {
                    // 8 -> 4 means we want half the default cost
                    int cost = 4;
                    int charmChangerCost = SharedData.charmChanger.GetField<int>("glowingWombSpawnCost");
                    if (SharedData.charmChanger.IsEnabled())
                    {
                        cost = charmChangerCost / 2;
                    }

                    // If we can't afford the discounted price, no need to bother
                    int currentMp = PlayerData.instance.GetInt("MPCharge");
                    //ExaltationExpanded.Instance.Log($"Primal Womb Patch - Cost: {cost}, MP: {currentMp}");
                    if (cost <= currentMp)
                    {
                        // If Charm Changer costs more than the discounted price, we'll add some "temp" SOUL for it to take away
                        // No need to go thru HeroController; this extra SOUL isn't really getting added to the player
                        int soulToAdd = Math.Max(0, charmChangerCost - cost);
                        PlayerData.instance.MPCharge += soulToAdd;
                        //ExaltationExpanded.Instance.Log($"Primal Womb Patch - CC Cost: {charmChangerCost}, MP Added: {soulToAdd}");

                        // Finally, send a custom event that links this step to the hatch step
                        //ExaltationExpanded.Instance.Log($"Primal Womb Patch - Hatching");
                        base.Fsm.Event("ExaltationExpanded");
                    }
                }

                //ExaltationExpanded.Instance.Log($"Primal Womb Patch - Not hatching");
            }

            // If we don't need to do any special logic, Finish the step and move on to the regular process
            Finish();
        }
    }
}
