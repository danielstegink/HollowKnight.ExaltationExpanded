using DanielSteginkUtils.Utilities;
using ExaltationExpanded.Exaltations;
using ExaltationExpanded.Helpers;
using Modding;
using System;
using UnityEngine;

namespace ExaltationExpanded.Patches
{
    /// <summary>
    /// Handles integrations with the Pale Court mod
    /// </summary>
    public class PaleCourt : ExaltationPatch
    {
        /// <summary>
        /// Stores the Pale Court mod if its installed
        /// </summary>
        public IMod paleCourtMod;

        public void ApplyHooks()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles applying exaltation to King's Honour
        /// </summary>
        public void CheckDefendersCrest()
        {
            bool upgradeCrest = IsDefendersCrestUpgraded();
            Exaltations.Exaltation dungDefender = SharedData.exaltations["10"];

            // Replace Royal Crest with King's Majesty upon unlocking King's Honour
            if (upgradeCrest &&
                !(dungDefender is KingsMajesty))
            {
                SharedData.Log("Replacing Royal Crest");
                dungDefender.Reset();
                SharedData.exaltations["10"] = new KingsMajesty();
                string id = SharedData.exaltations["10"].ID + "_P";
                Sprite sprite = SpriteHelper.GetLocalSprite(id);
                SharedData.sprites[SharedData.exaltations["10"].ID] = sprite;
            }
            else if (!upgradeCrest &&
                     dungDefender is KingsMajesty) // Make sure to reset to Royal Crest if King's Honour hasn't been unlocked
            {
                SharedData.Log("Replacing King's Majesty");
                dungDefender.Reset();
                SharedData.exaltations["10"] = new RoyalCrest();
                Sprite sprite = SpriteHelper.GetLocalSprite(SharedData.exaltations["10"].ID);
                SharedData.sprites[SharedData.exaltations["10"].ID] = sprite;
            }
        }

        /// <summary>
        /// Checks if Defender's Crest has been upgraded
        /// </summary>
        /// <returns></returns>
        private bool IsDefendersCrestUpgraded()
        {
            // If we're not using Pale Court, default to no
            if (!IsEnabled())
            {
                return false;
            }

            object saveSettings = ClassIntegrations.GetProperty<IMod, object>(paleCourtMod, "SaveSettings");
            return ClassIntegrations.GetField<object, bool>(saveSettings, "upgradedCharm_10");
        }

        /// <summary>
        /// Checks if this patch is enabled
        /// </summary>
        /// <returns></returns>
        private bool IsEnabled()
        {
            return paleCourtMod != null &&
                    SharedData.globalSettings.allowPaleCourt;
        }
    }
}
