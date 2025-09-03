using DanielSteginkUtils.Utilities;
using Exaltation;
using ExaltationExpanded.Helpers;
using ExaltationExpanded.Settings;
using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ExaltationExpanded
{
    public class ExaltationExpanded : Mod, ILocalSettings<LocalSaveSettings>, IGlobalSettings<GlobalSettings>, ICustomMenuMod
    {
        public static ExaltationExpanded Instance { get; private set; }

        public override string GetVersion() => "1.4.1.0";

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
        /// Canvas for displaying glorification text
        /// </summary>
        private GameObject canvas;

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");
            Instance = this;

            // This mod is meant to augment Exaltation
            SharedData.exaltationMod = (Exaltation.Exaltation)ModHooks.GetMod("Exaltation");
            if (SharedData.exaltationMod == null)
            {
                throw new MissingReferenceException("Exaltation not installed.");
            }

            // Check if external mods are installed
            SharedData.paleCourt.paleCourtMod = ModHooks.GetMod("Pale Court");
            SharedData.charmChanger.charmChangerMod = ModHooks.GetMod("CharmChanger");

            Log("Loading sprites");
            LoadSprites();

            Log("Applying patches");
            SharedData.nailsageGlory.ApplyHooks();
            SharedData.costPatch.ApplyHooks();
            SharedData.powerPatch.ApplyHooks();
            SharedData.voidSoul.ApplyHooks();
            SharedData.knightmareLullaby.ApplyHooks();

            Log("Applying hooks");
            On.HeroController.Start += NewGame;
            ModHooks.SavegameSaveHook += SaveGame;
            On.CharmIconList.GetSprite += GetSprite;
            ModHooks.LanguageGetHook += LanguageGet;

            Log("Initialized");
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
                SharedData.sprites.Add(key, sprite);
            }
        }

        #region On Save
        private void NewGame(On.HeroController.orig_Start orig, HeroController self)
        {
            orig(self);

            SharedData.paleCourt.CheckDefendersCrest();

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
                    //Log($"Exaltation {exaltation.Name} loaded from save");
                }
            }
        }

        /// <summary>
        /// Exaltation settings update when we save after a Godhome battle, so this is the best time to update charms
        /// </summary>
        /// <param name="obj"></param>
        private void SaveGame(int obj)
        {
            //SharedData.Log($"Checking if charms need to be upgraded");
            SharedData.paleCourt.CheckDefendersCrest();

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
                    //Log($"Exaltation {key} added to save");
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
        /// Displays a message about the charm being glorified. Code extracted from Exaltation
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
            SpriteFlash flash = ClassIntegrations.GetField<HeroController, SpriteFlash>(HeroController.instance, "spriteFlash");
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
        /// This event gets a charm's sprite, so this is the best time to update an exalted charm's sprite
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
                    return SharedData.sprites[key];
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

        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? modToggleDelegates)
        {
            return ModMenu.CreateMenuScreen(modListMenu);
        }
        #endregion
    }
}