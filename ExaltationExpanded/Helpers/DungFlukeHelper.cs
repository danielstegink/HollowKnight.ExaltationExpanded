using ItemChanger.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ExaltationExpanded.Helpers
{
    /// <summary>
    /// Dung Flukes don't store damage like regular flukes. Instead, they explode creating a 
    ///     custom dung cloud similar to the ones created by Defender's Crest.
    /// My attempts to modify the FSMs creating these clouds have ended in failure, so instead 
    ///     I made this helper that uses coroutines to find fluke clouds as they are made,
    ///     then apply the modifier provided in the constructor.
    /// </summary>
    public class DungFlukeHelper
    {
        public bool isActive { get; set; } = true;

        public float modifier { get; set; } = 1f;

        public bool performLogging { get; set; } = false;

        public DungFlukeHelper(float modifier, bool log = false)
        {
            this.modifier = modifier;
            performLogging = log;
        }

        /// <summary>
        /// Starts the coroutine that applies the modifier
        /// </summary>
        public void Start()
        {
            GameManager.instance.StartCoroutine(DungFlukeCheck());
        }

        /// <summary>
        /// Stops the coroutine
        /// </summary>
        public void Stop()
        {
            isActive = false;
        }

        /// <summary>
        /// Coroutine used to find dung clouds and modify them
        /// </summary>
        /// <returns></returns>
        private IEnumerator DungFlukeCheck()
        {
            while (isActive)
            {
                // Get a list of all the active dung clouds
                List<GameObject> dungClouds = UnityEngine.GameObject.FindObjectsOfType<GameObject>()
                                                                    .Where(x => x.name.Equals("Knight Dung Cloud"))
                                                                    .ToList();
                foreach (GameObject cloud in dungClouds)
                {
                    // Get or add a ModsApplied component
                    ModsApplied modsApplied = cloud.GetOrAddComponent<ModsApplied>();
                    if (performLogging)
                    {
                        SharedData.Log($"DungFlukeHelper - Mods applied: {string.Join("|", modsApplied.ModList)}, " +
                                                            $"Base damage interval: {modsApplied.baseValue}, " +
                                                            $"Modded damage interval: {modsApplied.moddedValue}");
                    }

                    // Get the Damage ticker and its interval
                    DamageEffectTicker damageEffectTicker = cloud.GetComponent<DamageEffectTicker>();
                    float interval = damageEffectTicker.damageInterval;

                    // If we are modding for the first time, the base value will be -1, 
                    // so we need to store the base value for future reference
                    if (modsApplied.baseValue < 0)
                    {
                        modsApplied.baseValue = interval;
                    }

                    // If the modded interval is less than 0, it hasn't been set yet
                    if (modsApplied.moddedValue < 0)
                    {
                        modsApplied.moddedValue = interval;
                    }

                    // If the current interval doesnt match the modded interval, then the Dung Cloud has
                    // been reset and we need to reapply the modded interval
                    if (modsApplied.moddedValue != interval)
                    {
                        // It is possible that the expected modifier has changed: Dung clouds have different
                        // intervals based on if the player has Shaman Stone equipped. When this happens, 
                        // all changes should be cleared out

                        // However, Dung Cloud has different bases depending on if the player has 
                        // Shaman Stone equipped, so the old base and stored base may be different. In this
                        // case, we want to reset and redo the buffs
                        if (interval != modsApplied.baseValue)
                        {
                            modsApplied.moddedValue = interval;
                            modsApplied.ModList = new List<string>();
                        }
                        else
                        {
                            interval = modsApplied.moddedValue;
                        }
                    }

                    // Now, with the base interval established and updated, 
                    // we can check if this mod has been applied
                    if (!modsApplied.ModList.Contains("ExaltationExpanded"))
                    {
                        interval *= modifier;
                        modsApplied.ModList.Add("ExaltationExpanded");
                        //
                        if (performLogging)
                        {
                            SharedData.Log($"DungFlukeHelper - Damage interval changed to {interval}");
                        }
                    }

                    damageEffectTicker.SetDamageInterval(interval);
                    modsApplied.moddedValue = interval;
                }

                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
    }
}
