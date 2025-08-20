using Satchel.BetterMenus;
using System;
using System.Collections.Generic;

namespace ExaltationExpanded
{
    public static class ModMenu
    {
        private static Menu menu;
        private static MenuScreen menuScreen;

        public static Dictionary<string, MenuScreen> subMenus;
        private static List<string> subMenuNames = new List<string>()
        {
            "Exaltation Balance",
            "Swappable Charms",
            "Mod Integrations"
        };

        /// <summary>
        /// Builds the Exaltation Expanded menu
        /// </summary>
        /// <param name="modListMenu"></param>
        /// <returns></returns>
        public static MenuScreen CreateMenuScreen(MenuScreen modListMenu)
        {
            // Declare the menu
            menu = new Menu("Exaltation Expanded Options", new Element[] { });

            // Populate main menu
            BuildMenu();

            // Insert the menu into the overall menu
            menuScreen = menu.GetMenuScreen(modListMenu);

            // Populate the sub-menus
            BuildSubMenus();

            return menuScreen;
        }

        /// <summary>
        /// Builds the main menu by initializing the sub-menus
        /// </summary>
        private static void BuildMenu()
        {
            foreach (string subMenuName in subMenuNames)
            {
                menu.AddElement(Blueprints.NavigateToMenu(subMenuName,
                                "",
                                () => subMenus[subMenuName]));
            }
        }

        /// <summary>
        /// Populates the sub-menus
        /// </summary>
        private static void BuildSubMenus()
        {
            subMenus = new Dictionary<string, MenuScreen>();
            
            // Balance patches for Exaltation
            Menu balanceMenu = new Menu(subMenuNames[0], new Element[]
            {
                new HorizontalOption("Power",
                                        "Power balance for Exaltation",
                                        MenuValues(),
                                        value => SharedData.globalSettings.allowBalancePatch = Convert.ToBoolean(value),
                                        () => Convert.ToInt32(SharedData.globalSettings.allowBalancePatch)),
                new HorizontalOption("Cost",
                                        "Charm cost fix for Exaltation",
                                        MenuValues(),
                                        value => SharedData.globalSettings.allowCostPatch = Convert.ToBoolean(value),
                                        () => Convert.ToInt32(SharedData.globalSettings.allowCostPatch)),
            });
            subMenus.Add(subMenuNames[0], balanceMenu.GetMenuScreen(menuScreen));

            // Swappable Charm patches for Exaltation
            Menu swapMenu = new Menu(subMenuNames[1], new Element[]
            {
                new HorizontalOption("Nailsage's Glory",
                                        "Possible to unlock both Nailsage charms",
                                        MenuValues(),
                                        value => SharedData.globalSettings.allowNailsageGlory = Convert.ToBoolean(value),
                                        () => Convert.ToInt32(SharedData.globalSettings.allowNailsageGlory)),
                new HorizontalOption("Void Soul",
                                        "Possible to unlock Lordsoul and Void Heart",
                                        MenuValues(),
                                        value => SharedData.globalSettings.allowVoidSoul = Convert.ToBoolean(value),
                                        () => Convert.ToInt32(SharedData.globalSettings.allowVoidSoul)),
                new HorizontalOption("Knightmare Lullaby",
                                        "Possible to unlock Grimmchild and Carefree Melody",
                                        MenuValues(),
                                        value => SharedData.globalSettings.allowKnightmareLullaby = Convert.ToBoolean(value),
                                        () => Convert.ToInt32(SharedData.globalSettings.allowKnightmareLullaby)),
            });
            subMenus.Add(subMenuNames[1], swapMenu.GetMenuScreen(menuScreen));

            // Mod integrations
            Menu otherModsMenu = new Menu(subMenuNames[2], new Element[]
            {
                new HorizontalOption("Pale Court",
                                        "Add exalted version of King's Honour",
                                        MenuValues(),
                                        value => SharedData.globalSettings.allowPaleCourt = Convert.ToBoolean(value),
                                        () => Convert.ToInt32(SharedData.globalSettings.allowPaleCourt)),
                //new HorizontalOption("Charm Changer",
                //                        "Uses Charm Changer settings for various properties",
                //                        MenuValues(),
                //                        value => SharedData.globalSettings.allowCharmChanger = Convert.ToBoolean(value),
                //                        () => Convert.ToInt32(SharedData.globalSettings.allowCharmChanger)),
            });
            subMenus.Add(subMenuNames[2], otherModsMenu.GetMenuScreen(menuScreen));
        }

        private static string[] MenuValues()
        {
            return new string[] { "OFF", "ON" };
        }
    }
}
