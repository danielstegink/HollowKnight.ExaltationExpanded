using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
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
            //SharedData.Log($"Getting local sprite {spriteId}");

            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream($"ExaltationExpanded.Resources.{spriteId}.png"))
            {
                // Convert stream to bytes
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);

                // Create texture from bytes
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(bytes, true);

                // Create sprite from texture
                //SharedData.Log($"Sprite found: ExaltationExpanded.Resources.{spriteId}.png");
                return Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));
            }
        }

        /// <summary>
        /// Gets a sprite from Exaltation's embedded resources
        /// </summary>
        /// <param name="spriteId"></param>
        /// <returns></returns>
        public static Sprite GetExaltedSprite(string spriteId)
        {
            //SharedData.Log($"Getting Exalted sprite {spriteId}");

            Assembly assembly = Assembly.Load("Exaltation");
            if (assembly == null)
            {
                SharedData.Log("Exaltation assembly not found");
            }

            //SharedData.Log("Exaltation assembly resources:");
            //foreach (string resource in assembly.GetManifestResourceNames())
            //{
            //    SharedData.Log(resource);
            //}

            using (Stream stream = assembly.GetManifestResourceStream($"Exaltation.Resources.Charms.{spriteId}.png"))
            {
                if (stream.Length == 0)
                {
                    SharedData.Log("File stream not found");
                }

                // Convert stream to bytes
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);

                // Create texture from bytes
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(bytes, true);

                // Create sprite from texture
                //SharedData.Log($"Sprite found: ExaltationExpanded.Resources.{spriteId}.png");
                return Sprite.Create(texture, 
                                        new Rect(0, 0, texture.width, texture.height), 
                                        new Vector2(0.5f, 0.5f));
            }
        }
    }
}
