using DanielSteginkUtils.Utilities;
using Modding;
using System;

namespace ExaltationExpanded.Patches
{
    /// <summary>
    /// Implements Charm Changer settings on various charms and exaltations
    /// </summary>
    public class CharmChanger
    {
        /// <summary>
        /// Stores the Charm Changer mod if its installed
        /// </summary>
        public IMod charmChangerMod;

        /// <summary>
        /// Checks if this patch is enabled
        /// </summary>
        /// <returns></returns>
        public bool IsEnabled()
        {
            return charmChangerMod != null &&
                    SharedData.globalSettings.allowCharmChanger;
        }

        /// <summary>
        /// Gets the number of notches a given charm costs
        /// </summary>
        /// <param name="id"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public int GetCharmNotches(int id, int defaultValue)
        {
            if (IsEnabled() && id <= 40)
            {
                return GetField<int>($"charm{id}NotchCost");
            }
            else
            {
                //ExaltationExpanded.Instance.Log($"Charm Changer - Not enabled");
                return defaultValue;
            }
        }

        /// <summary>
        /// Gets the number of notches a given charm costs.
        /// Has a minimum decimal value it returns.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="defaultValue"></param>
        /// <param name="minimum"></param>
        /// <returns></returns>
        public float GetCharmNotches(int id, int defaultValue, float minimum)
        {
            float cost = GetCharmNotches(id, defaultValue);
            return Math.Max(cost, minimum);
        }

        /// <summary>
        /// Gets a setting from Charm Changer
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public O GetField<O>(string fieldName)
        {
            object saveSettings = ClassIntegrations.GetProperty<IMod, object>(charmChangerMod, "LS");
            return ClassIntegrations.GetField<object, O>(saveSettings, fieldName);
        }
    }
}