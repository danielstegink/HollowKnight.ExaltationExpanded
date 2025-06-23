using Newtonsoft.Json.Linq;
using System;

namespace ExaltationExpanded.Integration
{
    public static class PaleCourt
    {
        /// <summary>
        /// Checks the given save file to see if the player
        /// has defeated Isma from Pale Court
        /// </summary>
        /// <param name="saveId"></param>
        /// <returns></returns>
        public static bool DefeatedIsma(int saveId)
        {
            try
            {
                //JToken defeatedIsma = SaveFile.GetModProperty(saveId, new string[]
                //{
                //    "modData",
                //    "Pale Court",
                //    "CompletionIsma",
                //    "isUnlocked"
                //});

                // Get the mod data
                string modData = SaveFile.GetModdedSaveFile(saveId);

                // Convert the json string to a json object
                JObject saveFile = JObject.Parse(modData);
                if (saveFile == null)
                {
                    SharedData.Log("Save File JObject not found");
                    return false;
                }

                JToken token = saveFile["modData"];
                if (token == null)
                {
                    SharedData.Log("modData JToken not found");
                    return false;
                }

                token = token["Pale Court"];
                if (token == null)
                {
                    SharedData.Log("Pale Court JToken not found");
                    return false;
                }

                token = token["CompletionIsma"];
                if (token == null)
                {
                    SharedData.Log("CompletionIsma JToken not found");
                    return false;
                }

                token = token["isUnlocked"];
                if (token == null)
                {
                    SharedData.Log("isUnlocked JToken not found");
                    return false;
                }
                //SharedData.Log(token.ToString());

                return bool.Parse(token.ToString());
            }
            catch (Exception ex) // If this breaks, we probly don't have Pale Court installed
            {
                SharedData.Log($"Exception while checking Isma: \n{ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }
    }
}
