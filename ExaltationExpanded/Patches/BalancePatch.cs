using Modding;
using UnityEngine;

namespace ExaltationExpanded.Patches
{
    /// <summary>
    /// Some of the charms in the base mod aren't balanced so
    /// that they are 1-notch more powerful than their
    /// base versions
    /// </summary>
    public class BalancePatch : Patches.ExaltationPatch
    {
        public void ApplyHooks()
        {
            // Glowing Womb and Quick Focus get checked on Charm Update and GameSave
            ModHooks.CharmUpdateHook += CharmUpdateHooks;
            ModHooks.SavegameSaveHook += SaveGameHooks;

            ModHooks.HeroUpdateHook += SteelTempestPatch;

            // todo - unclear if i want to do anything with lordsoul/voidheart
        }

        private void CharmUpdateHooks(PlayerData data, HeroController controller)
        {
            if (SharedData.globalSettings.allowBalancePatch)
            {
                SwiftFocusPatch();
                PrimalWombPatch();
            }
        }

        private void SaveGameHooks(int obj)
        {
            if (SharedData.globalSettings.allowBalancePatch)
            {
                SwiftFocusPatch();
                PrimalWombPatch();
            }
        }

        /// <summary>
        /// At 3 notches, Quick Focus reduces the focus cooldown from 0.891 to 0.594, which is about a 33% reduction
        /// Swift Focus reduces it to 0.33, which is a 63% reduction
        /// For an extra notch, this cooldown should be about 45%, from 0.891 to 0.49
        /// </summary>
        private void SwiftFocusPatch()
        {
            if (SharedData.exaltationMod.Settings.QuickFocusGlorified &&
                PlayerData.instance.equippedCharm_7)
            {
                // 0.33 * 1.49 ~= 0.49
                float baseValue = HeroController.instance.spellControl.Fsm.GetFsmFloat("Time Per MP Drain CH").Value;
                float newValue = baseValue * 1.49f;
                //SharedData.Log($"Swift Focus patch: {baseValue} -> {newValue}");
                HeroController.instance.spellControl.Fsm.GetFsmFloat("Time Per MP Drain CH").Value = newValue;
            }
        }

        /// <summary>
        /// At 2 notches, Glowing Womb produces a hatchling every 4 seconds at 8 SOUL each, for a 
        ///     maximum of 4 hatchlings
        /// Primal Womb produces them every 2 seconds at 4 SOUL each, maximum 8
        /// An extra notch would be worth 1 of these, but not all of them. I'm going to keep the
        ///     increased speed
        /// </summary>
        private void PrimalWombPatch()
        {
            if (SharedData.exaltationMod.Settings.GlowingWombGlorified &&
                PlayerData.instance.equippedCharm_22)
            {
                GameObject charmEffects = GameObject.Find("Charm Effects");
                if (charmEffects != null)
                {
                    PlayMakerFSM fsm = charmEffects.LocateMyFSM("Hatchling Spawn");
                    fsm.Fsm.GetFsmInt("Hatchling Max").Value = 4;
                    fsm.Fsm.GetFsmInt("Soul Cost").Value = 8;
                }
            }
        }

        /// <summary>
        /// At 3 notches, Quick Slash reduces attack cooldown from 0.41 to 0.25, a 39% reduction
        /// Steel Tempest reduces it from 0.41 to 0.05, an 88% reduction
        /// At 4 notches, Steel Tempest should only give about a 50% reduction, so 0.2
        /// </summary>
        private void SteelTempestPatch()
        {
            if (SharedData.exaltationMod.Settings.QuickSlashGlorified &&
                PlayerData.instance.equippedCharm_32 &&
                SharedData.globalSettings.allowBalancePatch)
            {
                // 0.05 * 4 = 0.2
                float cooldown = HeroController.instance.ATTACK_COOLDOWN_TIME_CH;
                float newCooldown = cooldown * 4;
                //SharedData.Log($"Steel Tempest cooldown: {cooldown} -> {newCooldown}");
                HeroController.instance.ATTACK_COOLDOWN_TIME_CH = newCooldown;

                // Duration should also be adjusted to slightly longer than cooldown
                float duration = HeroController.instance.ATTACK_DURATION_CH;
                float newDuration = newCooldown * 1.1f;
                //SharedData.Log($"Steel Tempest duration: {duration} -> {newDuration}");
                HeroController.instance.ATTACK_DURATION_CH = newDuration;
            }
        }
    }
}