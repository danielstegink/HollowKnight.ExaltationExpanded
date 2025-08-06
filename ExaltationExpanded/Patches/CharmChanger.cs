using DanielSteginkUtils.Utilities;
using Modding;

namespace ExaltationExpanded.Patches
{
    /// <summary>
    /// Implements Charm Changer settings on various charms and exaltations
    /// </summary>
    public class CharmChanger : ExaltationPatch
    {
        /// <summary>
        /// Stores the Charm Changer mod if its installed
        /// </summary>
        public IMod charmChangerMod;

        public void ApplyHooks() { }

        /// <summary>
        /// Checks if this patch is enabled
        /// </summary>
        /// <returns></returns>
        private bool IsEnabled()
        {
            return charmChangerMod != null &&
                    SharedData.globalSettings.allowCharmChanger;
        }

        /// <summary>
        /// Gets the basic SOUL gained from using Dream Nail
        /// </summary>
        /// <returns></returns>
        public int GetDreamNailSoulGain()
        {
            if (IsEnabled())
            {
                object saveSettings = ClassIntegrations.GetProperty<IMod, object>(charmChangerMod, "LS");
                return ClassIntegrations.GetField<object, int>(saveSettings, "regularDreamSoul");
            }
            else // The default is 33
            {
                return 33; 
            }
        }
    }
}