using DanielSteginkUtils.Helpers.Charms;
using ExaltationExpanded.Helpers;
using Modding;
using SFCore;
using System;
using UnityEngine;
namespace ExaltationExpanded.Patches
{
    /// <summary>
    /// Upon defeating Nightmare King Grimm on radiant difficulty, Grimmchild and Carefree Melody become separate
    /// charms with the same cost
    /// </summary>
    public class KnightmareLullaby : ExaltationPatch
    {
        /// <summary>
        /// Stores the second charm ID for Grimmchild/CM
        /// </summary>
        public int charmId = -1;

        public void ApplyHooks()
        {
            // Check if another mod, such as Carefree Grimm, is installed that added Grimmchild or Carefree Melody as an extra charm
            charmId = GetModCharmHelper.GetCharmId(new string[] { "Carefree Melody", "Grimmchild" });
            if (charmId != -1)
            {
                return;
            }

            // Add the charm to the charm list and get its new ID number
            Sprite grimmSprite = CharmIconList.Instance.spriteList[40];
            charmId = CharmHelper.AddSprites(new Sprite[] { grimmSprite })[0];

            // Apply charm effects
            On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.OnEnter += SpawnGrimmchild;
            On.HutongGames.PlayMaker.Actions.GetPlayerDataInt.OnEnter += SetGrimmchildLevel;
            On.HeroController.CharmUpdate += UpdateMelody;

            // Set hooks for getting charm data
            ModHooks.LanguageGetHook += GetCharmText;
            ModHooks.GetPlayerBoolHook += GetCharmBools;
            ModHooks.SetPlayerBoolHook += SetCharmBools;
            ModHooks.GetPlayerIntHook += GetCharmCosts;
            On.CharmIconList.GetSprite += GetIcon;
        }

        /// <summary>
        /// In the Spawn Grimmchild FSM, we need to bypass the Check state if Grimmchild is the 2nd charm
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void SpawnGrimmchild(On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.orig_OnEnter orig, HutongGames.PlayMaker.Actions.PlayerDataBoolTest self)
        {
            // Regardless of which test this is, we just need to send the EQUIPPED event
            if (self.Fsm.Name.Equals("Spawn Grimmchild") &&
                self.State.Name.Equals("Check") &&
                IsGrimmchildEquipped())
            {
                self.Fsm.Event("EQUIPPED");
            }
            else
            {
                orig(self);
            }
        }

        /// <summary>
        /// In the Grimmchild's Control FSM, we need to set the level to 4 if CM is the 1st charm
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void SetGrimmchildLevel(On.HutongGames.PlayMaker.Actions.GetPlayerDataInt.orig_OnEnter orig, HutongGames.PlayMaker.Actions.GetPlayerDataInt self)
        {
            orig(self);

            if (self.Fsm.Name.Equals("Control") &&
                self.State.Name.Equals("Init") &&
                IsGrimmchildEquipped())
            {
                if (self.storeValue.Value == 5)
                {
                    self.storeValue.Value = 4;
                }
            }

        }

        /// <summary>
        /// Gets if Grimmchild is equipped
        /// </summary>
        /// <returns></returns>
        private bool IsGrimmchildEquipped()
        {
            // If Grimmchild is the first charm
            if (PlayerData.instance.grimmChildLevel < 5)
            {
                return PlayerData.instance.GetBool($"equippedCharm_40");
            }
            else // If Grimmchild is the 2nd charm
            {
                return SharedData.saveSettings.knightmareLullabyEquipped;
            }
        }

        /// <summary>
        /// We need to ensure CM triggers on either charm
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        private void UpdateMelody(On.HeroController.orig_CharmUpdate orig, HeroController self)
        {
            orig(self);

            if (PlayerData.instance.GetInt("grimmChildLevel") == 4)
            {
                self.carefreeShieldEquipped = SharedData.saveSettings.knightmareLullabyEquipped;
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
            if (sheetName.Equals("UI"))
            {
                if (key.Equals($"CHARM_NAME_{charmId}"))
                {
                    if (PlayerData.instance.GetInt("grimmChildLevel") == 4)
                    {
                        return Language.Language.Get("CHARM_NAME_40_N", "UI");
                    }

                    return Language.Language.Get("CHARM_NAME_40", "UI");
                }

                if (key.Equals($"CHARM_DESC_{charmId}"))
                {
                    if (PlayerData.instance.GetInt("grimmChildLevel") == 4)
                    {
                        return Language.Language.Get("CHARM_DESC_40_N", "UI");
                    }

                    return Language.Language.Get("CHARM_DESC_40_F", "UI");
                }
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
                return IsUnlocked();
            }
            else if (key.Equals($"equippedCharm_{charmId}"))
            {
                return SharedData.saveSettings.knightmareLullabyEquipped;
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
                SharedData.saveSettings.knightmareLullabyEquipped = orig;
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
                return 2;
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets the charm icon
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private Sprite GetIcon(On.CharmIconList.orig_GetSprite orig, CharmIconList self, int id)
        {
            if (id == charmId)
            {
                if (PlayerData.instance.GetInt("grimmChildLevel") == 5)
                {
                    return self.grimmchildLevel4;
                }

                return self.nymmCharm;
            }

            return orig(self, id);
        }
        #endregion

        /// <summary>
        /// KL is only unlocked if the settings allow it and NKG has been beaten on radiant difficulty
        /// </summary>
        /// <returns></returns>
        private bool IsUnlocked()
        {
            if (!SharedData.globalSettings.allowKnightmareLullaby)
            {
                return false;
            }

            if (!PlayerData.instance.statueStateNightmareGrimm.completedTier3)
            {
                return false;
            }

            return true;
        }
    }
}
