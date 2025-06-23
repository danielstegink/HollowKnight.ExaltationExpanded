using UnityEngine;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Longnail
    /// </summary>
    public class LordNail : Exaltation
    {
        public override string Name => "Lord Nail";
        public override string Description => "The preferred weapon of those who rule the Mantis tribe.\n\n" +
                                                "Greatly increases the range of the bearer's nail, allowing them to strike foes from further away.";
        public override string ID => "18";

        public override string GodText => "gods of combat";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.statueStateMantisLords.completedTier2 || PlayerData.instance.bossDoorStateTier2.boundNail;
        }

        public override void Upgrade()
        {
            base.Upgrade();
            On.NailSlash.StartSlash += IncreaseNailLength;
        }

        public override void Reset()
        {
            base.Reset();
            On.NailSlash.StartSlash -= IncreaseNailLength;
        }

        /// <summary>
        /// Lord Nail increases the length of nail slashes by an additional 8.7%.
        /// The change is multiplicative, so 15% times an extra 8.7% becomes 25%, 
        /// and adding MOP changes the total bonus from 40% to 52%
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        private void IncreaseNailLength(On.NailSlash.orig_StartSlash orig, NailSlash self)
        {
            //SharedData.Log("Checking nail length");

            // Get the default scale
            Vector3 startingScale = self.scale;
            //SharedData.Log($"Starting scale: {ToString(startingScale)}");

            // If Longnail is equipped, increase the scale by 8.7%
            if (PlayerData.instance.equippedCharm_18)
            {
                Vector3 newScale = new Vector3(startingScale.x * 1.087f, startingScale.y * 1.087f);
                self.scale = newScale;
                //SharedData.Log($"New scale: {ToString(newScale)}");
            }

            // Perform the nail slash
            orig(self);
            //SharedData.Log($"Final scale: {ToString(self.transform.localScale)}");

            self.scale = startingScale;
        }

        /// <summary>
        /// String representation of the scale as (x,y)
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        private string ToString(Vector3 scale)
        {
            return $"({scale.x},{scale.y})";
        }
    }
}
