using ExaltationExpanded.Helpers;
using HutongGames.PlayMaker;
using Modding;
using SFCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExaltationExpanded.Patches
{
    /// <summary>
    /// Upon defeating Nailsage Sly on radiant difficulty, Nailsage's Patience and 
    /// Nailsage's Tenacity are unlocked as separate charms.
    /// </summary>
    public class NailsageGlory : ExaltationPatch
    {
        /// <summary>
        /// Stores the second charm ID for NMG
        /// </summary>
        private int charmId = -1;

        /// <summary>
        /// Local copy of the NSP and NST sprites
        /// </summary>
        private Sprite nstSprite, nspSprite;

        public void ApplyHooks()
        {
            // Get the sprites for NST and NSP
            nstSprite = SpriteHelper.GetExaltedSprite("26");
            nspSprite = SpriteHelper.GetExaltedSprite("26_patience");

            // Add the charm on initialization to reduce bugs
            AddNSG();
        }

        #region Adding 2nd NMG as a charm
        /// <summary>
        /// Adds a separate charm for NMG
        /// </summary>
        private void AddNSG()
        {
            //SharedData.Log("Adding Nailsage's Patience as a new charm.");

            // Add the charm to the charm list and get its new ID number
            Texture2D texture = new Texture2D(2, 2);
            Sprite sprite = Sprite.Create(texture,
                                            new Rect(0, 0, texture.width, texture.height),
                                            new Vector2(0.5f, 0.5f));
            charmId = CharmHelper.AddSprites(new Sprite[] { sprite })[0];

            // Apply charm effects
            ModHooks.HitInstanceHook += ApplyNSG;
            On.HeroController.CharmUpdate += AdjustCharmTime;
            On.CharmIconList.GetSprite += GetSprite;
            ModHooks.LanguageGetHook += GetCharmText;
            ModHooks.GetPlayerBoolHook += GetCharmBools;
            ModHooks.SetPlayerBoolHook += SetCharmBools;
            ModHooks.GetPlayerIntHook += GetCharmCosts;
        }

        /// <summary>
        /// Applies the second charm's effects if its equipped
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="hit"></param>
        /// <returns></returns>
        private HitInstance ApplyNSG(Fsm owner, HitInstance hit)
        {
            if ((SharedData.nailAttackNames.Contains(hit.Source.name) || 
                    SharedData.nailArtNames.Contains(hit.Source.name)) &&
                SharedData.saveSettings.nsgEquipped)
            {
                // Nailsage's Patience changes nail hits into spells
                // so that they bypass armor but don't gain SOUL.
                if (!SharedData.exaltationMod.Settings.Patience)
                {
                    hit.AttackType = AttackTypes.Spell;
                }
                // Nailsage's Tenacity increases nail damage dealt
                // based on how much damage the player has taken.
                else
                {
                    float modifier = 0.03f * (PlayerData.instance.maxHealth - PlayerData.instance.health);
                    int bonusDamage = (int)(hit.DamageDealt * modifier);
                    //SharedData.Log($"Increasing damage {hit.DamageDealt} by {bonusDamage} ({modifier * 100}%)");

                    hit.DamageDealt += bonusDamage;
                }
            }

            return hit;
        }

    /// <summary>
    /// Apply NMG's cooldown reduction so long as one of them is equipped,
    /// but don't stack it
    /// </summary>
    /// <param name="orig"></param>
    /// <param name="self"></param>
        private void AdjustCharmTime(On.HeroController.orig_CharmUpdate orig, HeroController self)
        {
            orig(self);

            //SharedData.Log($"NSG Original charge time: {SharedData.GetField<HeroController, float>(self, "nailChargeTime")}");
            if (SharedData.saveSettings.nsgEquipped)
            {
                //SharedData.Log($"Applying NSG cooldown");
                SharedData.SetField(self, "nailChargeTime", self.NAIL_CHARGE_TIME_CHARM);
            }
            //SharedData.Log($"NSG Final charge time: {SharedData.GetField<HeroController, float>(self, "nailChargeTime")}");

        }

        #region Charm Data and Settings
        /// <summary>
        /// Gets the charm sprite
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private Sprite GetSprite(On.CharmIconList.orig_GetSprite orig, CharmIconList self, int id)
        {
            if (id == charmId)
            {
                if (!SharedData.exaltationMod.Settings.Patience)
                {
                    return nspSprite;
                }
                else
                {
                    return nstSprite;
                }
            }

            return orig(self, id);
        }

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
                if (!SharedData.exaltationMod.Settings.Patience)
                {
                    return "Nailsage's Patience";
                }
                else
                {
                    return "Nailsage's Tenacity";
                }
            }
            else if (key.Equals($"CHARM_DESC_{charmId}"))
            {
                if (!SharedData.exaltationMod.Settings.Patience)
                {
                    return "Contains the timeless persistence and resolve of a Nailsage.\n\n" +
                            "Improves the bearer's mastery of Nail Arts and empowers their nail to slice through armor at the cost of its attacks yielding no SOUL.";
                }
                else
                {
                    return "Contains the timeless ferocity and vigor of a Nailsage.\n\n" +
                            "Improves the bearer's mastery of Nail Arts and increases the power of their nail strikes as they near death.";
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
                return NSGUnlocked();
            }
            else if (key.Equals($"equippedCharm_{charmId}"))
            {
                return SharedData.saveSettings.nsgEquipped;
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
                SharedData.saveSettings.nsgEquipped = orig;
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
                return 1; // todo - charm changer patch
            }

            return defaultValue;
        }
        #endregion

        /// <summary>
        /// NSG is only unlocked if the settings allow it
        /// and Sly has been beaten on radiant difficulty
        /// </summary>
        /// <returns></returns>
        private bool NSGUnlocked()
        {
            if (!SharedData.globalSettings.allowNailsageGlory)
            {
                return false;
            }

            // Confirm Sly has been beaten on radiant difficulty
            if (!PlayerData.instance.statueStateSly.completedTier3)
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}
