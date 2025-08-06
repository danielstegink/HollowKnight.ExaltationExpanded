using UnityEngine;

namespace ExaltationExpanded.Helpers
{
    public static class SpriteHelper
    {
        /// <summary>
        /// Gets the sprite from embedded resources
        /// </summary>
        /// <param name="spriteId"></param>
        /// <returns></returns>
        public static Sprite GetLocalSprite(string spriteId)
        {
            return DanielSteginkUtils.Helpers.SpriteHelper.GetLocalSprite($"ExaltationExpanded.Resources.{spriteId}.png", "ExaltationExpanded");
        }

        /// <summary>
        /// Gets a sprite from Exaltation's embedded resources
        /// </summary>
        /// <param name="spriteId"></param>
        /// <returns></returns>
        public static Sprite GetExaltedSprite(string spriteId)
        {
            return DanielSteginkUtils.Helpers.SpriteHelper.GetLocalSprite($"Exaltation.Resources.Charms.{spriteId}.png", "Exaltation");
        }
    }
}
