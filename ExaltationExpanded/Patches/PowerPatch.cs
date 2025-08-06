using DanielSteginkUtils.Utilities;
using Modding;
using UnityEngine;

namespace ExaltationExpanded.Patches
{
    /// <summary>
    /// Some of the charms need to be rebalanced so they are worth 2 extra notches.
    /// Some of them also simply don't work.
    /// </summary>
    public class PowerPatch : Patches.ExaltationPatch
    {
        public void ApplyHooks()
        {
            // Glowing Womb and Quick Focus get checked on Charm Update and GameSave
            ModHooks.CharmUpdateHook += CharmUpdateHooks;
            ModHooks.SavegameSaveHook += SaveGameHooks;

            ModHooks.HeroUpdateHook += SteelTempestPatch;
        }

        private void CharmUpdateHooks(PlayerData data, HeroController controller)
        {
            SwiftFocusPatch();
            PrimalWombPatch();
        }

        private void SaveGameHooks(int obj)
        {
            SwiftFocusPatch();
            PrimalWombPatch();
        }

        /// <summary>
        /// At 3 notches, Quick Focus reduces the focus cooldown from 0.891 to 0.594, which is about a 33% reduction.
        /// Swift Focus reduces it to 0.33, which is a 63% reduction.
        /// </summary>
        private void SwiftFocusPatch()
        {
            if (SharedData.exaltationMod.Settings.QuickFocusGlorified)
            {
                float exaltedValue = ClassIntegrations.GetField<Exaltation.Exaltation, float>(SharedData.exaltationMod, "SWIFT_FOCUS_SPEED_CH");
                if (SharedData.globalSettings.allowBalancePatch)
                {
                    // For 2 notches the cooldown should be 55%, reducing the speed from 0.891 to 0.396.
                    // 0.33 * 1.2 ~= 0.396
                    exaltedValue *= 1.2f;
                }

                HeroController.instance.spellControl.Fsm.GetFsmFloat("Time Per MP Drain CH").Value = exaltedValue;
                //SharedData.Log($"Swift Focus patch: {exaltedValue}");
            }
        }

        /// <summary>
        /// At 2 notches, Glowing Womb produces a hatchling every 4 seconds at 8 SOUL each, for a maximum of 4 hatchlings.
        /// Primal Womb produces them every 2 seconds at 4 SOUL each, maximum 8.
        /// </summary>
        private void PrimalWombPatch()
        {
            if (SharedData.exaltationMod.Settings.GlowingWombGlorified)
            {
                // Doubling the speed is worth 2 notches.
                // Halving the SOUL cost is also worth 2 notches.
                // Increasing the maximum is worth another notch, as it doesn't affect the rate but promotes increased starting potential.
                // So overall, Primal Womb increases the value of Glowing Womb by about 5 notches.
                // For 2 extra notches, I will keep the increased speed.
                GameObject charmEffects = GameObject.Find("Charm Effects");
                if (charmEffects != null)
                {
                    bool patch = SharedData.globalSettings.allowBalancePatch;

                    PlayMakerFSM fsm = charmEffects.LocateMyFSM("Hatchling Spawn");
                    fsm.Fsm.GetFsmInt("Hatchling Max").Value = patch ? 4 : 8;
                    fsm.Fsm.GetFsmFloat("Hatch Time").Value = 2f;
                    fsm.Fsm.GetFsmInt("Soul Cost").Value = patch ? 8 : 4;

                    //int hatchlings = fsm.Fsm.GetFsmInt("Hatchling Max").Value;
                    //float time = fsm.Fsm.GetFsmFloat("Hatch Time").Value;
                    //int cost = fsm.Fsm.GetFsmInt("Soul Cost").Value;
                    //SharedData.Log($"Primal Womb patch: max {hatchlings} at {cost} SOUL per hatchling every {time} seconds");
                }
            }
        }

        /// <summary>
        /// At 3 notches, Quick Slash reduces attack cooldown from 0.41 to 0.25, a 39% reduction.
        /// Steel Tempest reduces it from 0.41 to 0.05, an 88% reduction.
        /// At 5 notches, Steel Tempest should only give about a 65% reduction, resulting in a value of 0.143
        /// </summary>
        private void SteelTempestPatch()
        {
            if (SharedData.exaltationMod.Settings.QuickSlashGlorified)
            {
                float cooldown = ClassIntegrations.GetField<Exaltation.Exaltation, float>(SharedData.exaltationMod, "STEEL_TEMPEST_ATTACK_COOLDOWN");
                float duration = ClassIntegrations.GetField<Exaltation.Exaltation, float>(SharedData.exaltationMod, "STEEL_TEMPEST_ATTACK_DURATION");
                
                if (SharedData.globalSettings.allowBalancePatch)
                {
                    // 0.05 * 2.867 = 0.143
                    cooldown *= 2f + 13f / 15f;

                    // Duration should also be adjusted to slightly longer than cooldown
                    // 0.1 * 1.8 = 0.18
                    duration *= 1.8f;
                }

                HeroController.instance.ATTACK_COOLDOWN_TIME_CH = cooldown;
                HeroController.instance.ATTACK_DURATION_CH = duration;
                //SharedData.Log($"Steel Tempest patch: {cooldown}, {duration}");
            }
        }
    }
}