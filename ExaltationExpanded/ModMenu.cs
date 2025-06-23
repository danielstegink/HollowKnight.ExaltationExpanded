using Modding;
using System;
using System.Collections.Generic;

namespace ExaltationExpanded
{
    public static class ModMenu
    {
        public static List<IMenuMod.MenuEntry> CreateMenu()
        {
            SharedData.Log("Building menu");

            List<IMenuMod.MenuEntry> menuOptions = new List<IMenuMod.MenuEntry>()
            {
                new IMenuMod.MenuEntry()
                {
                    Name = "Integrate Pale Court",
                    Description = "Add exalted version King's Honour",
                    Values = MenuValues(),
                    Saver = value => SharedData.globalSettings.allowPaleCourt = Convert.ToBoolean(value),
                    Loader = () => Convert.ToInt32(SharedData.globalSettings.allowPaleCourt)
                },
                new IMenuMod.MenuEntry()
                {
                    Name = "Nailsage's Glory",
                    Description = "Possible to unlock both Nailsage charms",
                    Values = MenuValues(),
                    Saver = value => SharedData.globalSettings.allowNailsageGlory = Convert.ToBoolean(value),
                    Loader = () => Convert.ToInt32(SharedData.globalSettings.allowNailsageGlory)
                },
                new IMenuMod.MenuEntry()
                {
                    Name = "Balance",
                    Description = "Power balance for original Exaltation mod",
                    Values = MenuValues(),
                    Saver = value => SharedData.globalSettings.allowBalancePatch = Convert.ToBoolean(value),
                    Loader = () => Convert.ToInt32(SharedData.globalSettings.allowBalancePatch)
                },
                new IMenuMod.MenuEntry()
                {
                    Name = "Cost",
                    Description = "Charm cost fix for original Exaltation mod",
                    Values = MenuValues(),
                    Saver = value => SharedData.globalSettings.allowCostPatch = Convert.ToBoolean(value),
                    Loader = () => Convert.ToInt32(SharedData.globalSettings.allowCostPatch)
                },
            };

            return menuOptions;
        }

        private static string[] MenuValues()
        {
            return new string[] { "OFF", "ON" };
        }
    }
}
