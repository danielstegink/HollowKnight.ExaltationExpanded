using System;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Unbreakable Greed
    /// </summary>
    public class GoldenTouch : Exaltation
    {
        public override string Name => "Golden Touch";
        public override string Description => "A symbol of wealth and ambition.\n\n" + 
                                                "Causes the bearer to find more Geo.";
        public override string ID => "24_G";

        public override int IntID => 24;

        public override string GodText => "gods of brotherhood";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.bossDoorStateTier1.boundCharms;
        }

        public override void Upgrade()
        {
            base.Upgrade();
            On.HeroController.AddGeo += AddGeo;
        }

        public override void Reset()
        {
            base.Reset();
            On.HeroController.AddGeo -= AddGeo;
        }

        /// <summary>
        /// Unbreakable Greed increases Geo dropped by enemies by 20%.
        /// The exalted version will increase all Geo dropped by 10%.
        /// This stacks with the original 20% to a total of 32% dropped from enemies.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="amount"></param>
        private void AddGeo(On.HeroController.orig_AddGeo orig, HeroController self, int amount)
        {
            //SharedData.Log("Adding geo");
            if (PlayerData.instance.equippedCharm_24)
            {
                int bonusAmount = Math.Max(amount / 10, 1);
                //SharedData.Log($"Unbreakable greed confirmed. Incrementing {amount} by {bonusAmount}");
                amount += bonusAmount;
            }

            orig(self, amount);
        }
    }
}
