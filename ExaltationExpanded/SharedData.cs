using ExaltationExpanded.Exaltations;
using ExaltationExpanded.Patches;
using ExaltationExpanded.Settings;
using System.Collections.Generic;
using UnityEngine;

namespace ExaltationExpanded
{
    /// <summary>
    /// Stores variables and functions used by multiple files in this project
    /// </summary>
    public static class SharedData
    {
        /// <summary>
        /// Parent mod Exaltation
        /// </summary>
        public static Exaltation.Exaltation exaltationMod { get; set; }

        #region Settings
        public static GlobalSettings globalSettings { get; set; } = new GlobalSettings();

        public static LocalSaveSettings saveSettings { get; set; } = new LocalSaveSettings();
        #endregion

        /// <summary>
        /// List of supported exaltations
        /// </summary>
        public static Dictionary<string, Exaltations.Exaltation> exaltations = new Dictionary<string, Exaltations.Exaltation>()
        {
            { "35", new GrubberflyRequiem() },
            { "23_G", new CompleteMask() },
            { "24_G", new GoldenTouch() },
            { "25_G", new VesselsMight() },
            { "15", new CrushingBlow() },
            { "18", new LordNail() },
            { "13", new MarkOfBetrayal() },
            { "11", new Flukeswarm() },
            { "10", new RoyalCrest() },
            { "34", new NexusOfLight() },
            { "17", new AbyssalShroom() },
            { "28", new BlessingOfUnn() },
            { "39", new BeastsCall() },
            { "30", new RadiantPresence() },
            { "38", new Mothshield() },
            { "Grimmchild", new Knightmare() },
            { "CarefreeMelody", new LovingLullaby() },
            { "VesselsLament", new VesselsDuty() },
            { "MarkOfPurity", new VesselsAssault() },
            { "BoonOfHallownest", new VesselsSpirit() },
            { "AbyssalBloom", new VesselsDarkness() },
        };

        /// <summary>
        /// Cache sprites for future reference
        /// </summary>
        public static Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

        /// <summary>
        /// Determines if the charm has already been glorified
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUpgraded(string id)
        {
            return saveSettings.Exalted.Contains(id);
        }

        #region Patches
        public static NailsageGlory nailsageGlory { get; set; } = new NailsageGlory();

        public static CostPatch costPatch { get; set; } = new CostPatch();

        public static PowerPatch powerPatch { get; set; } = new PowerPatch();

        public static PaleCourt paleCourt { get; set; } = new PaleCourt();

        public static KnightmareLullaby knightmareLullaby { get; set; } = new KnightmareLullaby();

        public static VoidSoul voidSoul { get; set; } = new VoidSoul();

        public static CharmChanger charmChanger { get; set; } = new CharmChanger();
        #endregion
    }
}