using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace ExaltationExpanded.Integration
{
    public static class SaveFile
    {
        /// <summary>
        /// Gets the JSON-formatted text of the given save file's mod data
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetModdedSaveFile(int index)
        {
            // Get save folder
            string saveFolder = UnityEngine.Application.persistentDataPath;

            // Build file name using save index
            string fileName = $"user{index}.modded.json";

            // Combine folder and file paths
            string fullSavePath = Path.Combine(saveFolder, fileName);

            // Get full json text as a string
            return File.ReadAllText(fullSavePath);

        }

        /// <summary>
        /// Traverses the given list of JSON properties to get a json token from
        /// the given save's modded data
        /// </summary>
        /// <param name="saveId"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static JToken GetModProperty(int saveId, string[] properties)
        {
            // Get the mod data
            string modData = GetModdedSaveFile(saveId);

            // Convert the json string to a json object
            JObject saveFile = JObject.Parse(modData);

            // Traverse the property list to find the desired property
            JToken token = null;
            foreach(string property in properties)
            {
                if (token == null)
                {
                    token = saveFile[property];
                }
                else
                {
                    token = token[property];
                }
            }

            return token;
        }
    }
}