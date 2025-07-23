using System.Collections.Generic;
using UnityEngine;

namespace ExaltationExpanded.Helpers
{
    /// <summary>
    /// Used for tracking which mods have altered a given game object
    /// </summary>
    public class ModsApplied : MonoBehaviour
    {
        /// <summary>
        /// List of mods that have altered the property
        /// </summary>
        public List<string> ModList = new List<string>();

        /// <summary>
        /// Result of the modifications on the given field
        /// </summary>
        public float moddedValue = -1f;

        /// <summary>
        /// Stores the original modded value
        /// </summary>
        public float baseValue = -1f;
    }
}