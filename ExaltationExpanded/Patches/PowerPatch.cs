using DanielSteginkUtils.Helpers.Abilities;
using DanielSteginkUtils.Utilities;
using ExaltationExpanded.Helpers;
using GlobalEnums;
using HutongGames.PlayMaker;
using Modding;
using SFCore.Utils;
using System;
using UnityEngine;

namespace ExaltationExpanded.Patches
{
    /// <summary>
    /// Some of the charms need to be rebalanced so they are worth 2 extra notches.
    /// Some of them also simply don't work.
    /// </summary>
    public class PowerPatch : Patches.ExaltationPatch
    {
        private DashHelper dashmasterHelper;

        public void ApplyHooks()
        {
            On.HeroController.StartMPDrain += SwiftFocusPatch;

            On.HealthManager.TakeDamage += ShamanRelicPatch;

            On.HeroController.Start += PrimalWombPatch;
            On.HutongGames.PlayMaker.Actions.Wait.OnEnter += PrimalWombTimePatch;

            ModHooks.CharmUpdateHook += StagwayCoinPatch;

            On.HeroController.DoAttack += SteelTempestCooldownPatch;
            On.HeroController.Attack += SteelTempestDurationPatch;

            On.HeroController.Move += MarathonPatch;
        }

        /// <summary>
        /// At 3 notches, Quick Focus reduces the focus cooldown from 0.891 to 0.594, which is about a 33% reduction.
        /// 
        /// Swift Focus reduces it to 0.33, which is a 63% reduction.
        /// 
        /// For 2 extra notches, Swift Focus should reduce it by 55% to 0.41.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="time"></param>
        private void SwiftFocusPatch(On.HeroController.orig_StartMPDrain orig, HeroController self, float time)
        {
            //ExaltationExpanded.Instance.Log($"Swift Focus Patch - MP Drain started");

            if (SharedData.exaltationMod.Settings.QuickFocusGlorified &&
                PlayerData.instance.GetBool("equippedCharm_7"))
            {
                float oldTime = time;

                // First get the modifiers
                // Remember to adjust for Quick Focus' default bonus, as that gets applied no matter what
                float baseValue = 0.891f;
                float charmModifier = 0.594f / baseValue;
                float modModifier = 0.33f / baseValue / charmModifier;
                float newModifier = 0.45f / charmModifier;
                bool charmChangerInstalled = SharedData.charmChanger.charmChangerMod != null;

                // If the balance patch is enabled, we want to apply the new modifier
                if (SharedData.globalSettings.allowBalancePatch)
                {
                    // If charm changer is installed, it will have overwritten Exaltation's changes
                    // Otherwise, need to adjust the new modifier to balance with Exaltation's modifier
                    if (!charmChangerInstalled)
                    {
                        newModifier /= modModifier;
                    }

                    time *= newModifier;

                }
                else // If balance patch is not enabled, we want to apply Exaltation's modifier
                {
                    // Though we only need to do this if Charm Changer has overwritten our changes
                    if (charmChangerInstalled)
                    {
                        time *= modModifier;
                    }
                }

                //ExaltationExpanded.Instance.Log($"Swift Focus Patch - {oldTime} -> {time}");
            }

            orig(self, time);
        }

        /// <summary>
        /// For 3 notches, Shaman Stone increases the damage of spells by an average of 50% (33% for VS, but it gets a size boost).
        /// 
        /// Shaman Relic increases damage by a further 12.5%, for a total of 68.75%.
        /// 
        /// To be worth 2 notches, it should increase spell damage by a total of 83%.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="hitInstance"></param>
        private void ShamanRelicPatch(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            if (hitInstance.AttackType == AttackTypes.Spell &&
                SharedData.exaltationMod.Settings.ShamanStoneGlorified &&
                PlayerData.instance.GetBool("equippedCharm_19") &&
                SharedData.globalSettings.allowBalancePatch)
            {
                // Get the new modifier, and remove the bonus from Exaltation's modifier
                float baseModifier = 0.5f;
                int notchCost = 3;
                float boostPerNotch = baseModifier / notchCost;
                float newModifier = 1 + boostPerNotch * (notchCost + 2);
                float charmModifier = 1.5f;
                float modModifier = 1.125f;
                newModifier /= charmModifier;
                newModifier /= modModifier;

                int baseDamage = hitInstance.DamageDealt;
                hitInstance.DamageDealt = Calculations.GetModdedInt(baseDamage, newModifier);
                //ExaltationExpanded.Instance.Log($"Shaman Relic Patch - {baseDamage} * {newModifier} = {hitInstance.DamageDealt}");
            }

            orig(self, hitInstance);
        }

        /// <summary>
        /// At 2 notches, Glowing Womb produces a hatchling every 4 seconds at 8 SOUL each, for a maximum of 4 hatchlings.
        /// Primal Womb produces them every 2 seconds at 4 SOUL each, maximum 8.
        /// Any one of these categories would be worth the 2 notches. I've elected to keep the time boost.
        /// 
        /// However, Charm Changer overwrites Primal Womb's changes, so I need to preserve original functionality.
        /// Normally I would modify the IntCompares in the FSM, but for some reason Charm Changer takes precedence.
        /// So instead I will insert a custom action that checks max/cost using the desired values.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        private void PrimalWombPatch(On.HeroController.orig_Start orig, HeroController self)
        {
            //ExaltationExpanded.Instance.Log($"Primal Womb Patch - Modifying FSM");
            GameObject charmEffects = HeroController.instance.gameObject.FindGameObjectInChildren("Charm Effects");
            PlayMakerFSM fsm = charmEffects.LocateMyFSM("Hatchling Spawn");
            FsmState canHatch = fsm.Fsm.GetState("Can Hatch?");
            FsmStateAction checkSoulAndCount = new CheckSoulAndCountAction();
            canHatch.InsertAction(checkSoulAndCount, 2);
            canHatch.AddTransition("ExaltationExpanded", "Hatch");

            orig(self);
        }

        /// <summary>
        /// At 2 notches, Glowing Womb produces a hatchling every 4 seconds at 8 SOUL each, for a maximum of 4 hatchlings.
        /// 
        /// Primal Womb produces them every 2 seconds at 4 SOUL each, maximum 8.
        /// 
        /// Any one of these properties would be worth the 2 notches. I've elected to keep the time boost.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void PrimalWombTimePatch(On.HutongGames.PlayMaker.Actions.Wait.orig_OnEnter orig, HutongGames.PlayMaker.Actions.Wait self)
        {
            float defaultValue = self.time.Value;

            if (self.Fsm.Name.Equals("Hatchling Spawn") &&
                self.State.Name.Equals("Equipped") &&
                SharedData.exaltationMod.Settings.GlowingWombGlorified &&
                PlayerData.instance.GetBool("equippedCharm_22"))
            {
                self.time.Value = 2f;
                //ExaltationExpanded.Instance.Log($"Primal Womb Patch - Time: {defaultValue} -> {self.time}");
            }

            orig(self);

            if (self.time.Value != defaultValue)
            {
                //ExaltationExpanded.Instance.Log($"Primal Womb Patch - Final time: {self.time.Value}");
                self.time.Value = defaultValue;
            }
        }

        /// <summary>
        /// Currently, Stagway Coin only reduces the notch cost of Dashmaster by 1.
        /// 
        /// So, we will spend 1 additional notch to reduce Dashmaster's cooldown.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="controller"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void StagwayCoinPatch(PlayerData data, HeroController controller)
        {
            if (dashmasterHelper != null)
            {
                dashmasterHelper.Stop();
            }

            if (SharedData.exaltationMod.Settings.DashmasterGlorified &&
                PlayerData.instance.GetBool("equippedCharm_31") &&
                SharedData.globalSettings.allowBalancePatch)
            {
                float modifier = 1 - NotchCosts.GetDashCooldownPerNotch();
                dashmasterHelper = new DashHelper(modifier, 1);
                dashmasterHelper.Start();
            }
        }

        /// <summary>
        /// At 3 notches, Quick Slash reduces attack cooldown from 0.41 to 0.25, a 39% reduction.
        /// 
        /// Steel Tempest reduces it from 0.41 to 0.05, an 88% reduction.
        /// 
        /// At 5 notches, Steel Tempest should only give about a 65% reduction, resulting in a value of 0.143.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="attackDir"></param>
        private void SteelTempestCooldownPatch(On.HeroController.orig_DoAttack orig, HeroController self)
        {
            // Charm Changer takes effect during DoAttack, so we have to make our change afterwards
            orig(self);

            if (SharedData.exaltationMod.Settings.QuickSlashGlorified &&
                PlayerData.instance.GetBool("equippedCharm_32"))
            {
                float baseCooldown = 0.41f;
                float charmCooldown = ClassIntegrations.GetField<Exaltation.Exaltation, float>(SharedData.exaltationMod, "BASE_ATTACK_COOLDOWN_CH");
                float modCooldown = ClassIntegrations.GetField<Exaltation.Exaltation, float>(SharedData.exaltationMod, "STEEL_TEMPEST_ATTACK_COOLDOWN");

                if (SharedData.globalSettings.allowBalancePatch)
                {
                    int notchCost = 3;
                    float cooldownPerNotch = (1 - charmCooldown / baseCooldown) / notchCost;
                    float percentCooldown = cooldownPerNotch * (2 + notchCost);
                    float modifier = 1 - percentCooldown;
                    modCooldown = self.ATTACK_COOLDOWN_TIME * modifier;
                }

                ClassIntegrations.SetField<HeroController>(self, "attack_cooldown", modCooldown);
            }
        }

        /// <summary>
        /// Steel Tempest reduces the attack duration from 0.28 to 0.1.
        /// 
        /// Based on the cooldown reduction, the new duration should probably be 0.18 or so.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="attackDir"></param>
        private void SteelTempestDurationPatch(On.HeroController.orig_Attack orig, HeroController self, AttackDirection attackDir)
        {
            orig(self, attackDir);

            if (SharedData.exaltationMod.Settings.QuickSlashGlorified &&
                PlayerData.instance.GetBool("equippedCharm_32"))
            {
                float baseDuration = 0.36f;
                float charmDuration = ClassIntegrations.GetField<Exaltation.Exaltation, float>(SharedData.exaltationMod, "BASE_ATTACK_DURATION_CH");
                float modDuration = ClassIntegrations.GetField<Exaltation.Exaltation, float>(SharedData.exaltationMod, "STEEL_TEMPEST_ATTACK_DURATION");

                if (SharedData.globalSettings.allowBalancePatch)
                {
                    int notchCost = 3;
                    float cooldownPerNotch = (1 - charmDuration / baseDuration) / notchCost;
                    float percentCooldown = cooldownPerNotch * (2 + notchCost);
                    float modifier = 1 - percentCooldown;
                    modDuration = self.ATTACK_DURATION * modifier;
                }

                ClassIntegrations.SetField<HeroController>(self, "attackDuration", modDuration);
            }
        }

        private HitInstance PrismaticLensPatch(Fsm owner, HitInstance hit)
        {
            if (hit.AttackType == AttackTypes.Spell)
            {
                if (SharedData.globalSettings.allowBalancePatch &&
                    SharedData.exaltationMod.Settings.SpellTwisterGlorified &&
                    PlayerData.instance.GetBool("equippedCharm_33"))
                {
                    float DamageIncrease = PlayerData.instance.MPCharge / 4;
                    ExaltationExpanded.Instance.Log($"Spell Twister Patch - Percent increase: {DamageIncrease}");

                    if (DamageIncrease > 25)
                    {
                        DamageIncrease = 25;
                        ExaltationExpanded.Instance.Log($"Spell Twister Patch - Percent increase capped at {DamageIncrease}");
                    }

                    DamageIncrease /= 100; //turn "25", "10" etc. into 0.25f, 0.1f
                    ExaltationExpanded.Instance.Log($"Spell Twister Patch - Percent increase converted to {DamageIncrease}");

                    DamageIncrease += 1f; //turn 0.25f, 0.1f etc. into 1.25f, 1.1f
                    ExaltationExpanded.Instance.Log($"Spell Twister Patch - Percent increase converted to {DamageIncrease}");

                    int baseDamage = hit.DamageDealt;
                    hit.DamageDealt = (int)(hit.DamageDealt * DamageIncrease);
                    ExaltationExpanded.Instance.Log($"Spell Twister Patch - {baseDamage} -> {hit.DamageDealt} ({DamageIncrease})");
                }
            }

            return hit;
        }

        /// <summary>
        /// Marathon Master increases Sprintmaster's movement speed by 20%.
        /// 
        /// This is worth 1 notch as Sprintmaster increases movement speed by 20% for 1 notch.
        /// 
        /// So we need to increase it by another 20%, for a total of 60%
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="move_direction"></param>
        private void MarathonPatch(On.HeroController.orig_Move orig, HeroController self, float move_direction)
        {
            if (SharedData.globalSettings.allowBalancePatch &&
                SharedData.exaltationMod.Settings.SprintmasterGlorified &&
                PlayerData.instance.GetBool("equippedCharm_37"))
            {
                // First get the percent speed boost per notch
                // By default, this will be 20% for 1 notch
                float speedBoostPerNotch = 20f;
                int notchValue = 1;

                // Next, we need the desired bonus for Sprintmaster's notch cost plus 2
                notchValue += 2;
                float speedBoost = speedBoostPerNotch * notchValue;
                speedBoost = 1 + (speedBoost / 100f);

                // Exaltation Expanded sets the bonus to 1.4 already, so we need
                // 1.4 * x = 1.6, x = 1.14
                float modifier = speedBoost / 1.4f;
                move_direction *= modifier;
            }

            orig(self, move_direction);
        }
    }
}