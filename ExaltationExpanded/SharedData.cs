using ExaltationExpanded.Exaltations;
using ExaltationExpanded.Patches;
using ExaltationExpanded.Settings;
using System.Collections.Generic;
using System.Reflection;

namespace ExaltationExpanded
{
    /// <summary>
    /// Stores variables and functions used by multiple files in this project
    /// </summary>
    public static class SharedData
    {
        private static ExaltationExpanded _logger = new ExaltationExpanded();

        #region External Mods
        /// <summary>
        /// Parent mod Exaltation
        /// </summary>
        public static Exaltation.Exaltation exaltationMod { get; set; }
        #endregion

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
        };

        /// <summary>
        /// Stores the ID of the second Grimmchild/Carefree Melody
        /// charm, if it exists
        /// </summary>
        public static int carefreeGrimmId { get; set; } = -1;

        #region Patches
        public static NailsageGlory nailsageGlory { get; set; } = new NailsageGlory();

        public static CostPatch costPatch { get; set; } = new CostPatch();

        public static BalancePatch balancePatch { get; set; } = new BalancePatch();

        public static VoidSoul voidSoul { get; set; } = new VoidSoul();
        #endregion

        /// <summary>
        /// Logs message to the shared mod log at AppData\LocalLow\Team Cherry\Hollow Knight\ModLog.txt
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message)
        {
            _logger.Log(message);
        }

        #region Getting private fields, methods, etc
        /// <summary>
        /// Gets a non-static field from the given input class
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="O"></typeparam>
        /// <param name="input"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static O GetField<I, O>(I input, string fieldName)
        {
            FieldInfo fieldInfo = input.GetType()
                                       .GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return (O)fieldInfo.GetValue(input);
        }

        /// <summary>
        /// Sets the value of non-static field in a given class
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <param name="input"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public static void SetField<I>(I input, string fieldName, object value)
        {
            FieldInfo fieldInfo = input.GetType()
                                       .GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(input, value);
        }

        /// <summary>
        /// Gets a non-static property from the given input class
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="O"></typeparam>
        /// <param name="input"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static O GetProperty<I, O>(I input, string fieldName)
        {
            PropertyInfo propertyInfo = input.GetType()
                                       .GetProperty(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return (O)propertyInfo.GetValue(input);
        }
        #endregion

        /// <summary>
        /// List of the object names of the regular nail attacks
        /// </summary>
        public static List<string> nailAttackNames = new List<string>()
        {
            "Slash",
            "AltSlash",
            "UpSlash",
            "DownSlash",
        };

        /// <summary>
        /// List of the object names of the Nail Art attacks
        /// </summary>
        public static List<string> nailArtNames = new List<string>()
        {
            "Cyclone Slash",
            "Great Slash",
            "Dash Slash",
            "Hit L",
            "Hit R"
        };
    }
}