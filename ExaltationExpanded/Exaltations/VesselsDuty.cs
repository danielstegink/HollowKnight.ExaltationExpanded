using DanielSteginkUtils.Helpers.Charms;
using DanielSteginkUtils.Helpers.Libraries;
using DanielSteginkUtils.Utilities;
using Modding;
using System;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// Replaces Vessel's Lament
    /// </summary>
    public class VesselsDuty : Exaltation
    {
        public override string Name { get; set; } = "Vessel's Duty";

        public override string Description { get; set; } = "Symbolizes the Hollow Knight's commitment to stopping the Infection.\n\n" +
                                                            "Focus to twist the SOUL of enemies marked by the nail and unleash devastating blasts of energy upon them.\n\n" +
                                                            "Slightly increases the amount of SOUL gained when striking an enemy with the nail.";

        public override string GodText { get; set; } = "god of lands beyond";

        public override string ID { get => GetPaleCourtId(); set => throw new NotImplementedException(); }

        /// <summary>
        /// Stores the numeric ID of the Pale Court charm
        /// </summary>
        private int paleCourtId = -1;

        /// <summary>
        /// Gets the numeric ID of the charm
        /// </summary>
        /// <returns></returns>
        private string GetPaleCourtId()
        {
            if (SharedData.paleCourt.IsEnabled() &&
                paleCourtId < 0)
            {
                paleCourtId = SharedData.paleCourt.charmIds[1];
                //ExaltationExpanded.Instance.Log($"Pale Court - {Name} ID found: {paleCourtId}");
            }

            return paleCourtId.ToString();
        }

        public override bool CanUpgrade()
        {
            // If we're not using Pale Court, default to no
            if (!SharedData.paleCourt.IsEnabled())
            {
                return false;
            }

            object saveSettings = ClassIntegrations.GetProperty<IMod, object>(SharedData.paleCourt.paleCourtMod, "SaveSettings");
            object zemerSettings = ClassIntegrations.GetField<object, object>(saveSettings, "CompletionZemer");
            bool canUpgrade = ClassIntegrations.GetField<object, bool>(zemerSettings, "completedTier2");
            return canUpgrade;
        }

        public override void Equip()
        {
            base.Equip();
            On.HealthManager.TakeDamage += ExtraSoul;
        }

        public override void Unequip()
        {
            base.Unequip();
            On.HealthManager.TakeDamage -= ExtraSoul;
        }

        /// <summary>
        /// Vessel's Duty gets extra SOUL from enemies on nail strike
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="hitInstance"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ExtraSoul(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            orig(self, hitInstance);

            if (hitInstance.AttackType == AttackTypes.Nail)
            {
                float soulGained = GetSoulNotches() * NotchCosts.SoulPerNailPerNotch();
                SoulHelper.GainSoul((int)soulGained);
            }
        }

        /// <summary>
        /// Vessel's Duty spends 1 notch on SOUL gained from nail strikes
        /// </summary>
        /// <returns></returns>
        protected virtual int GetSoulNotches()
        {
            return 1;
        }
    }
}