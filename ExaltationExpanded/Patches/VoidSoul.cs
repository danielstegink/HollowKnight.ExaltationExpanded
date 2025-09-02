using DanielSteginkUtils.Utilities;
using ExaltationExpanded.Helpers;
using Modding;
using SFCore;
using System;
using System.Collections;
using UnityEngine;

namespace ExaltationExpanded.Patches
{
    /// <summary>
    /// Upon defeating Pure Vessel on radiant difficulty, Lordsoul becomes a separate
    /// charm from Void Heart
    /// </summary>
    public class VoidSoul : ExaltationPatch
    {
        /// <summary>
        /// Stores the second charm ID for VS
        /// </summary>
        private int charmId = -1;

        public void ApplyHooks()
        {
            // Add the charm to the charm list and get its new ID number
            Sprite lordSprite = SpriteHelper.GetExaltedSprite("36");
            charmId = CharmHelper.AddSprites(new Sprite[] { lordSprite })[0];

            // Apply charm effects
            ModHooks.BeforeSavegameSaveHook += BeforeSave;
            ModHooks.LanguageGetHook += GetCharmText;
            ModHooks.GetPlayerBoolHook += GetCharmBools;
            ModHooks.SetPlayerBoolHook += SetCharmBools;
            ModHooks.GetPlayerIntHook += GetCharmCosts;
        }

        /// <summary>
        /// One of the features of this mod is that Exaltation cannot create Lordsoul anymore,
        ///     which means we have to stop it from switching to Lordsoul.
        /// The switch happens in Glorification, which happens in SaveGameSave, so we need to
        ///     set some values to prevent it from swapping the charms.
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void BeforeSave(SaveGameData data)
        {
            // Only run this check if Void Soul is unlocked 
            if (VoidSoulUnlocked())
            {
                // Default Lordsoul to false so Void Heart stays the default, and disable
                //      SwitchSoul so it never happens
                ClassIntegrations.SetField(SharedData.exaltationMod, "SwitchSoul", false);
                SharedData.exaltationMod.Settings.Lordsoul = false;
            }
        }

        #region Charm Data and Settings
        /// <summary>
        /// Gets text data related to the charms (name and description)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sheetName"></param>
        /// <param name="orig"></param>
        /// <returns></returns>
        private string GetCharmText(string key, string sheetName, string orig)
        {
            if (key.Equals($"CHARM_NAME_{charmId}"))
            {
                return "Lordsoul";
            }
            else if (key.Equals($"CHARM_DESC_{charmId}"))
            {
                return "Soul of the Pale Wyrm who gave birth to this land's monarch.\n\n" +
                        "The bearer will slowly absorb the limitless SOUL contained within.";
            }

            return orig;
        }

        /// <summary>
        /// Gets boolean values related to the charms (equipped, new, found)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private bool GetCharmBools(string key, bool defaultValue)
        {
            if (key.Equals($"gotCharm_{charmId}"))
            {
                return VoidSoulUnlocked();
            }
            else if (key.Equals($"equippedCharm_{charmId}"))
            {
                return SharedData.saveSettings.voidSoulEquipped;
            }
            else if (key.Equals($"newCharm_{charmId}"))
            {
                return false;
            }

            return defaultValue;
        }

        /// <summary>
        /// Sets boolean values related to the charms (equipped, new, found)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="orig"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private bool SetCharmBools(string key, bool orig)
        {
            if (key.Equals($"equippedCharm_{charmId}"))
            {
                SharedData.saveSettings.voidSoulEquipped = orig;

                // If the charm was just equipped, run the coroutine
                if (SharedData.saveSettings.voidSoulEquipped)
                {
                    GameManager.instance.StartCoroutine(RunLordsoul());
                }
            }

            return orig;
        }

        /// <summary>
        /// Gets the costs of charms
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private int GetCharmCosts(string key, int defaultValue)
        {
            if (key.Equals($"charmCost_{charmId}"))
            {
                if (SharedData.charmChanger.IsEnabled())
                {
                    // Charm Changer sets Kingoul's cost to 0 since it and Void Heart share the setting
                    // So I've added an extra setting to override the base cost in Charm Changer
                    int defaultCost = SharedData.globalSettings.charmChangerKingsoulCost;
                    return Math.Max(0, defaultCost - 2);
                }

                return 3;
            }

            return defaultValue;
        }
        #endregion

        /// <summary>
        /// VoidSoul is only unlocked if the settings allow it
        /// and Pure Vessel has been beaten on radiant difficulty
        /// </summary>
        /// <returns></returns>
        private bool VoidSoulUnlocked()
        {
            if (!SharedData.globalSettings.allowVoidSoul)
            {
                return false;
            }

            // Confirm Pure Vessel has been beaten on radiant difficulty
            if (!PlayerData.instance.statueStateHollowKnight.completedTier3)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Applies the effects of Kingsoul if Lordsoul is equipped
        /// </summary>
        /// <returns></returns>
        private IEnumerator RunLordsoul()
        {
            // Kingsoul gives 4 SOUL every 2 seconds
            int soulGain = 4;
            float waitTime = 2f;
            if (SharedData.charmChanger.IsEnabled())
            {
                soulGain = SharedData.charmChanger.GetField<int>("kingsoulSoulGain");
                waitTime = SharedData.charmChanger.GetField<float>("kingsoulSoulTime");
            }

            while (SharedData.saveSettings.voidSoulEquipped)
            {
                HeroController.instance.AddMPCharge(soulGain);
                yield return new WaitForSeconds(waitTime);
            }
        }
    }
}
