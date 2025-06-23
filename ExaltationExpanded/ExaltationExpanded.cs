using Exaltation;
using ExaltationExpanded.Exaltations;
using ExaltationExpanded.Helpers;
using ExaltationExpanded.Integration;
using ExaltationExpanded.Settings;
using Modding;
using SFCore.Generics;
using SFCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ExaltationExpanded
{
    public class ExaltationExpanded : Mod, IGlobalSettings<GlobalSettings>, IMenuMod, ILocalSettings<LocalSaveSettings>
    {
        public override string GetVersion() => "1.0.1.0";

        public override int LoadPriority() => 2;

        #region Save Settings
        public void OnLoadGlobal(GlobalSettings s)
        {
            SharedData.globalSettings = s;
        }

        public GlobalSettings OnSaveGlobal()
        {
            return SharedData.globalSettings;
        }

        public void OnLoadLocal(LocalSaveSettings s)
        {
            SharedData.saveSettings = s;
        }

        public LocalSaveSettings OnSaveLocal()
        {
            return SharedData.saveSettings;
        }
        #endregion

        /// <summary>
        /// Cache sprites for future reference
        /// </summary>
        private Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

        /// <summary>
        /// Index of the current save file
        /// </summary>
        private int currentSave = -1;

        /// <summary>
        /// Canvas for displaying glorification text
        /// </summary>
        private GameObject canvas;

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            SharedData.Log("Initializing");

            // This mod is meant to augment Exaltation
            SharedData.exaltationMod = (Exaltation.Exaltation)ModHooks.GetMod("Exaltation");
            if (SharedData.exaltationMod == null)
            {
                throw new MissingReferenceException("Exaltation not installed.");
            }

            // Check if a mod is installed that added Grimmchild or
            // Carefree Melody as an extra charm
            //SharedData.Log("Checking for second CarefreeGrimm charm");
            int customCharmCount = FullSettingsMod<SFCoreSaveSettings, SFCoreGlobalSettings>.GlobalSettings.MaxCustomCharms;
            for (int i = 41; i <= 40 + customCharmCount; i++)
            {
                string charmName = Language.Language.Get($"CHARM_NAME_{i}", "UI");
                //SharedData.Log($"Charm {i}'s name: {charmName}");
                if (charmName.Equals("Carefree Melody") ||
                    charmName.Equals("Grimmchild"))
                {
                    SharedData.carefreeGrimmId = i;
                    break;
                }
            }
            //SharedData.Log($"CarefreeGrimm ID: {SharedData.carefreeGrimmId}");

            SharedData.Log("Loading sprites");
            LoadSprites();

            SharedData.Log("Applying hooks");
            ModHooks.SavegameLoadHook += NewGame;
            ModHooks.SavegameSaveHook += SaveGame;
            On.CharmIconList.GetSprite += GetSprite;
            ModHooks.LanguageGetHook += LanguageGet;

            SharedData.Log("Applying patches");
            SharedData.nailsageGlory.ApplyHooks();
            SharedData.costPatch.ApplyHooks();
            SharedData.balancePatch.ApplyHooks();

            SharedData.Log("Initialized");
        }

        /// <summary>
        /// Caches the exalted sprites for future reference
        /// </summary>
        private void LoadSprites()
        {
            foreach (string key in SharedData.exaltations.Keys)
            {
                //SharedData.Log($"Loading sprite for {key}");
                Sprite sprite = SpriteHelper.GetLocalSprite(key);
                sprites.Add(key, sprite);
            }
        }

        /// <summary>
        /// When a new game is loaded, the exaltations need to be reset
        /// in case the new save doesn't have them unlocked
        /// </summary>
        /// <param name="saveIndex"></param>
        private void NewGame(int saveIndex)
        {
            //SharedData.Log($"Resetting exaltations for a new game: {saveIndex}");
            currentSave = saveIndex;

            foreach (string key in SharedData.exaltations.Keys)
            {
                Exaltations.Exaltation exaltation = SharedData.exaltations[key];

                // Reset the exaltation
                exaltation.Reset();

                // Re-add it if it's been glorified on this save already
                if (exaltation.CanUpgrade() &&
                    IsUpgraded(key))
                {
                    exaltation.Upgrade();
                    //SharedData.Log($"Exaltation {key} loaded from save");
                }
            }
        }

        #region On Save
        /// <summary>
        /// Exaltation settings update when we save after a Godhome battle, so this is the best
        /// time to update charms
        /// </summary>
        /// <param name="obj"></param>
        private void SaveGame(int obj)
        {
            //SharedData.Log($"Checking if charms need to be upgraded");
            CheckPaleCourt();

            // Go through each exaltation and see if it needs to be upgraded
            string gloryText = "";
            foreach (string key in SharedData.exaltations.Keys)
            {
                Exaltations.Exaltation exaltation = SharedData.exaltations[key];
                if (exaltation.CanUpgrade() &&
                    !IsUpgraded(key))
                {
                    SharedData.saveSettings.Exalted.Add(key);
                    exaltation.Upgrade();
                    gloryText = $"Charms glorified by the {exaltation.GodText}";
                    //SharedData.Log($"Exaltation {key} added to save");
                }
            }

            // If we have a message to display, and Exaltation isn't going to display one,
            // display ours
            if (!string.IsNullOrWhiteSpace(gloryText))
            {
                HeroController.instance.StartCoroutine(GloryFX(gloryText));
            }
        }

        /// <summary>
        /// Integration step for Pale Court mod
        /// </summary>
        private void CheckPaleCourt()
        {
            bool defeatedIsma = SharedData.globalSettings.allowPaleCourt && 
                                PaleCourt.DefeatedIsma(currentSave);
            //SharedData.Log($"Defeated Isma: {defeatedIsma}");
            Exaltations.Exaltation dungDefender = SharedData.exaltations["10"];

            // Replace Royal Crest with King's Majesty upon unlocking King's Honour
            if (defeatedIsma && 
                dungDefender is RoyalCrest)
            {
                //SharedData.Log($"Replacing Royal Crest");
                SharedData.exaltations["10"] = new KingsMajesty();
                string id = SharedData.exaltations["10"].ID + "_P";
                Sprite sprite = SpriteHelper.GetLocalSprite(id);
                sprites[SharedData.exaltations["10"].ID] = sprite;
            }
            else if (!defeatedIsma && 
                     dungDefender is KingsMajesty) // Make sure to reset to Royal Crest if Isma hasn't been defeated in this save
            {
                //SharedData.Log($"Replacing King's Majesty");
                SharedData.exaltations["10"] = new RoyalCrest();
                Sprite sprite = SpriteHelper.GetLocalSprite(SharedData.exaltations["10"].ID);
                sprites[SharedData.exaltations["10"].ID] = sprite;
            }
        }

        /// <summary>
        /// Displays a message about the charm being glorified. Code extracted
        /// from Exaltation
        /// </summary>
        /// <param name="gloryText"></param>
        /// <returns></returns>
        public IEnumerator GloryFX(string gloryText)
        {
            //SharedData.Log($"Glorification occurred: {gloryText}");
            if (canvas == null)
            {
                canvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920f, 1080f));
            }

            // Initialize the game object that displays the message
            CanvasUtil.RectData dimensions = new CanvasUtil.RectData(new Vector2(0, 50), new Vector2(0, 45),
                                                                     new Vector2(0, 0), new Vector2(1, 0),
                                                                     new Vector2(0.5f, 0.5f));
            GameObject textPanel = CanvasUtil.CreateTextPanel(canvas, "", 27, TextAnchor.MiddleCenter, dimensions);
            Text messageObject = textPanel.GetComponent<Text>();
            messageObject.font = CanvasUtil.TrajanBold;
            messageObject.text = "";
            messageObject.fontSize = 42;

            // Wait before applying the text
            yield return new WaitForSeconds(0.35f);
            messageObject.text = gloryText;
            messageObject.CrossFadeAlpha(1f, 0f, false);

            // Use the flash animation, shake effect and sound effect associated with charm glorification
            SpriteFlash flash = SharedData.GetField<HeroController, SpriteFlash>(HeroController.instance, "spriteFlash");
            flash.flash(Color.white, 1.75f, 0.25f, 1f, 0.5f);
            ReflectionHelper.GetField<HeroController, AudioSource>(HeroController.instance, "audioSource")
                .PlayOneShot(LoadAssets.GlorifySound, 1f);
            GameCameras.instance.cameraShakeFSM.SendEvent("BigShake");

            // Fade the message back out
            yield return new WaitForSeconds(1.5f);
            messageObject.CrossFadeAlpha(0f, 1f, false);
        }
        #endregion

        #region Exalted Data
        /// <summary>
        /// This event gets a charm's sprite, so this is the best time to 
        /// update an exalted charm's sprite
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private Sprite GetSprite(On.CharmIconList.orig_GetSprite orig, CharmIconList self, int id)
        {
            //SharedData.Log($"Getting charm sprite: {id}");

            // Get the exaltation that matches the numeric ID
            Exaltations.Exaltation exaltation = SharedData.exaltations.Values.FirstOrDefault(x => x.IntID == id);
            if (exaltation != default)
            {
                // Get the matching key, since glorification is linked by key instead of ID
                string key = SharedData.exaltations.FirstOrDefault(x => x.Value == exaltation).Key;
                if (IsUpgraded(key))
                {
                    //SharedData.Log($"Exaltation {key} is upgraded");
                    return sprites[key];
                }
            }

            return orig(self, id);
        }

        /// <summary>
        /// Changes the name and description of exalted charms
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sheetTitle"></param>
        /// <param name="orig"></param>
        /// <returns></returns>
        private string LanguageGet(string key, string sheetTitle, string orig)
        {
            if (key.StartsWith("CHARM_NAME_") || 
                key.StartsWith("CHARM_DESC_"))
            {
                string id = key.Split(new string[] { "CHARM_NAME_", "CHARM_DESC_" }, StringSplitOptions.None)[1];
                //SharedData.Log("Getting charm text: " + id);
                if (id.StartsWith("40"))
                {
                    id = "40";
                }

                Exaltations.Exaltation exaltation = SharedData.exaltations.Values.FirstOrDefault(x => x.ID.Equals(id.ToString()));
                if (exaltation != default)
                {
                    //SharedData.Log("Exaltation found: " + exaltation.Name);

                    // Get the matching key, since glorification is linked by key instead of ID
                    string exaltationKey = SharedData.exaltations.FirstOrDefault(x => x.Value == exaltation).Key;
                    //SharedData.Log("Exaltation key: " + exaltationKey);
                    if (IsUpgraded(exaltationKey))
                    {
                        //SharedData.Log("Exaltation is active");
                        if (key.StartsWith("CHARM_NAME_"))
                        {
                            return exaltation.Name;
                        }
                        else
                        {
                            return exaltation.Description;
                        }
                    }
                }
            }

            return orig;
        }

        /// <summary>
        /// Determines if the charm has already been glorified
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool IsUpgraded(string id)
        {
            return SharedData.saveSettings.Exalted.Contains(id);
        }
        #endregion

        #region Menu Options
        public bool ToggleButtonInsideMenu => false;

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            return ModMenu.CreateMenu();
        }
        #endregion
    }
}