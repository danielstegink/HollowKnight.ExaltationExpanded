using DanielSteginkUtils.Utilities;
using ExaltationExpanded.Exaltations;
using ExaltationExpanded.Helpers;
using Modding;
using System.Collections.Generic;
using UnityEngine;

namespace ExaltationExpanded.Patches
{
    /// <summary>
    /// Handles integrations with the Pale Court mod
    /// </summary>
    public class PaleCourt : ExaltationPatch
    {
        /// <summary>
        /// Stores the Pale Court mod if installed
        /// </summary>
        public IMod paleCourtMod;

        /// <summary>
        /// Stores the PC Charms mod if installed
        /// </summary>
        public IMod pcCharmsMod;

        public List<int> charmIds;

        public void ApplyHooks()
        {
            ModHooks.GetPlayerIntHook += AdjustCharmCost;
        }

        /// <summary>
        /// Most of the PC charms just have reduced cost
        /// </summary>
        /// <param name="name"></param>
        /// <param name="orig"></param>
        /// <returns></returns>
        private int AdjustCharmCost(string name, int orig)
        {
            if (SharedData.paleCourt.IsEnabled())
            {
                // Vessel's Duty and Vessel's Resolve cost 1 notch less than Vessel's Lament
                Exaltations.Exaltation vesselsLament = SharedData.exaltations["VesselsLament"];
                if (name.Equals($"charmCost_{vesselsLament.IntID}") &&
                    SharedData.IsUpgraded("VesselsLament"))
                {
                    return 1;
                }

                // Vessel's Assault costs 2 notches less than Mark of Purity
                Exaltations.Exaltation markOfPurity = SharedData.exaltations["MarkOfPurity"];
                if (name.Equals($"charmCost_{markOfPurity.IntID}") &&
                    SharedData.IsUpgraded("MarkOfPurity"))
                {
                    return 1;
                }

                // Vessel's Spirit costs 2 notches less than Boon of Hallownest
                Exaltations.Exaltation boonOfHallownest = SharedData.exaltations["BoonOfHallownest"];
                if (name.Equals($"charmCost_{boonOfHallownest.IntID}") &&
                    SharedData.IsUpgraded("BoonOfHallownest"))
                {
                    return 2;
                }

                // Vessel's Darkness costs 2 notches less than Abyssal Bloom
                Exaltations.Exaltation abyssalBloom = SharedData.exaltations["AbyssalBloom"];
                if (name.Equals($"charmCost_{abyssalBloom.IntID}") &&
                    SharedData.IsUpgraded("AbyssalBloom"))
                {
                    return 3;
                }
            }

            return orig;
        }

        /// <summary>
        /// Gets the Charm IDs from PC and PC Charms
        /// </summary>
        public void GetCharmIds()
        {
            if (paleCourtMod != null)
            {
                charmIds = ClassIntegrations.GetField<IMod, List<int>>(paleCourtMod, "charmIDs");
            }
            else if (pcCharmsMod != null)
            {
                charmIds = ClassIntegrations.GetField<IMod, List<int>>(pcCharmsMod, "CharmIDs");
            }
        }

        /// <summary>
        /// Defender's Crest and Vessel's Lament both have multiple exaltations, so we need to swap between them as needed
        /// </summary>
        public void SwapExaltations()
        {
            //ExaltationExpanded.Instance.Log($"Pale Court - Swapping Exaltations");
            SwapDefendersCrest();
            SwapVesselsLament();
        }

        /// <summary>
        /// Handles applying exaltation to King's Honour
        /// </summary>
        private void SwapDefendersCrest()
        {
            // By default, Defender's Crest should evolve into Royal Crest
            SharedData.exaltations["10"].Reset();
            SharedData.exaltations["10"] = new RoyalCrest();
            Sprite sprite = SpriteHelper.GetLocalSprite("10");
            SharedData.sprites["10"] = sprite;

            // At the peak is Vessel's Birthright
            VesselsBirthright lv4 = new VesselsBirthright();
            if (lv4.CanUpgrade())
            {
                SharedData.exaltations["10"].Reset();
                SharedData.exaltations["10"] = lv4;

                sprite = SpriteHelper.GetLocalSprite("10_4");
                SharedData.sprites["10"] = sprite;
                return;
            }

            // Vessel's Aura is just below it
            VesselsAura lv3 = new VesselsAura();
            if (lv3.CanUpgrade())
            {
                SharedData.exaltations["10"].Reset();
                SharedData.exaltations["10"] = lv3;

                sprite = SpriteHelper.GetLocalSprite("10_3");
                SharedData.sprites["10"] = sprite;
                return;
            }

            // But if we can always upgrade to King's Majesty, if able
            KingsMajesty lv2 = new KingsMajesty();
            if (lv2.CanUpgrade())
            {
                SharedData.exaltations["10"].Reset();
                SharedData.exaltations["10"] = lv2;

                sprite = SpriteHelper.GetLocalSprite("10_2");
                SharedData.sprites["10"] = sprite;
            }
        }

        /// <summary>
        /// Handles swapping between Vessel's Duty and Vessel's Resolve
        /// </summary>
        private void SwapVesselsLament()
        {
            // By default, Vessel's Lament should turn into Vessel's Duty
            SharedData.exaltations["VesselsLament"].Reset();
            SharedData.exaltations["VesselsLament"] = new VesselsDuty();
            Sprite sprite = SpriteHelper.GetLocalSprite("VesselsLament");
            SharedData.sprites["VesselsLament"] = sprite;

            // But if we can upgrade to Resolve, do so
            VesselsResolve resolve = new VesselsResolve();
            if (resolve.CanUpgrade())
            {
                SharedData.exaltations["VesselsLament"].Reset();
                SharedData.exaltations["VesselsLament"] = resolve;

                sprite = SpriteHelper.GetLocalSprite("VesselsLament_2");
                SharedData.sprites["VesselsLament"] = sprite;
            }
        }

        /// <summary>
        /// Checks if this patch is enabled
        /// </summary>
        /// <returns></returns>
        internal bool IsEnabled()
        {
            bool isEnabled = paleCourtMod != null &&
                                SharedData.globalSettings.allowPaleCourt;
            //ExaltationExpanded.Instance.Log($"Pale Court - Enabled: {isEnabled}");
            return isEnabled;
        }
    }
}