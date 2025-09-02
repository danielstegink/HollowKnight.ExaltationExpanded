using DanielSteginkUtils.Helpers.Attributes;
using DanielSteginkUtils.Utilities;

namespace ExaltationExpanded.Helpers
{
    /// <summary>
    /// Helper for Lord Nail
    /// </summary>
    public class LordNailHelper : NailRangeHelper
    {
        public LordNailHelper()
        {
            forceMantisAnim = true;
        }

        public override bool CustomCheck()
        {
            return true;
        }

        /// <summary>
        /// Lord Nail increases the range of Longnail
        /// </summary>
        /// <returns></returns>
        public override float GetModifier()
        {
            // Per my Utils, nail range can be increased by 8.33% per notch
            // For 2 notches, thats 16.67%
            float modifierIncrease = 2 * NotchCosts.NailRangePerNotch();

            // We know Longnail is equipped, which means the nail's range is currently 115%
            // To increase it by 16.67%, we need to multiply so the total is 131.67%
            // 1.15 * x = 131.67, x = 1.14
            float modifier = GetLongNailModifier();
            modifier = (modifier + modifierIncrease) / modifier;
            return modifier;
        }

        /// <summary>
        /// Gets Longnail's range modifier
        /// </summary>
        /// <returns></returns>
        private float GetLongNailModifier()
        {
            if (SharedData.charmChanger.IsEnabled())
            {
                int modifier = SharedData.charmChanger.GetField<int>("longnailScale");
                //ExaltationExpanded.Instance.Log($"Lord Nail - Range modifier {modifier}");
                return 1f + (float)modifier / 100;
            }

            return 1.15f;
        }
    }
}
