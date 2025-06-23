using Modding;

namespace ExaltationExpanded.Patches
{
    /// <summary>
    /// Bug where some of Exaltation's charms aren't free
    /// </summary>
    public class CostPatch : ExaltationPatch
    {
        public void ApplyHooks()
        {
            ModHooks.GetPlayerIntHook += GetCharmCosts;
        }

        /// <summary>
        /// Exaltation changes the costs of 4 charms, but the code is a little buggy
        /// </summary>
        /// <param name="name"></param>
        /// <param name="orig"></param>
        /// <returns></returns>
        private int GetCharmCosts(string name, int orig)
        {
            if (SharedData.globalSettings.allowCostPatch)
            {
                // Wayward Compass
                if (name.Equals("charmCost_2") &&
                    SharedData.exaltationMod.Settings.WaywardCompassGlorified)
                {
                    return 0;
                }
                // Steady Body
                else if (name.Equals("charmCost_14") &&
                         SharedData.exaltationMod.Settings.SteadyBodyGlorified)
                {
                    return 0;
                }
                // Hiveblood
                else if (name.Equals("charmCost_29") &&
                         SharedData.exaltationMod.Settings.HivebloodGlorified)
                {
                    return 3; // todo - reduces cost by 1. should integrate w charm changer later
                }
                // Dashmaster
                else if (name.Equals("charmCost_31") &&
                         SharedData.exaltationMod.Settings.DashmasterGlorified)
                {
                    return 2; // todo - see hiveblood
                }
            }

            return orig;
        }
    }
}
