using DanielSteginkUtils.Utilities;
using Modding;
using System;
using System.Collections;
using UnityEngine;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// Replaces Vessel's Aura
    /// </summary>
    public class VesselsBirthright : VesselsAura
    {
        public override string Name => "Vessel's Birthright";
        public override string Description => "Embodies the Hollow Knight's charisma and its connection to the Pale King.\n\n" +
                                                "Imbues the bearer with a powerful aura and a gradual supply of SOUL.";

        public override string GodText => "gods of honour and care";

        public override bool CanUpgrade()
        {
            if (SharedData.paleCourt.IsEnabled())
            {
                object saveSettings = ClassIntegrations.GetProperty<IMod, object>(SharedData.paleCourt.paleCourtMod, "SaveSettings");
                if (saveSettings == null)
                {
                    throw new Exception("Unable to get PC Save settings");
                }

                object ismaSettings = ClassIntegrations.GetField<object, object>(saveSettings, "CompletionIsma2");
                return ClassIntegrations.GetField<object, bool>(ismaSettings, "completedTier2");
            }

            return false;
        }

        public override void Equip()
        {
            base.Equip();

            GameManager.instance.StartCoroutine(GainSoul());
        }

        public override void Unequip()
        {
            base.Unequip();
        }

        /// <summary>
        /// Vessel's Birthright gives SOUL
        /// </summary>
        /// <returns></returns>
        private IEnumerator GainSoul()
        {
            // Kingsoul gives 4 SOUL every 2 seconds for 5 notches
            int soulGain = 4;
            float waitTime = 2f;
            if (SharedData.charmChanger.IsEnabled())
            {
                soulGain = SharedData.charmChanger.GetField<int>("kingsoulSoulGain");
                waitTime = SharedData.charmChanger.GetField<float>("kingsoulSoulTime");
            }

            // So for 2 notches, Vessel's Birthright should give 4 SOUL every 5 seconds
            waitTime *= 5 / 2;
            while (IsEquipped)
            {
                HeroController.instance.AddMPCharge(soulGain);
                yield return new WaitForSeconds(waitTime);
            }
        }
    }
}