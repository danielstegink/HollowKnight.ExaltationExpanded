using Modding;
using System;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// Template class for the exalted versions of charms
    /// </summary>
    public abstract class Exaltation
    {
        /// <summary>
        /// Name of the exalted charm
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// Description of the exalted charm
        /// </summary>
        public abstract string Description { get; set; }

        /// <summary>
        /// Numeric ID of the original charm
        /// </summary>
        public abstract string ID { get; set; }

        /// <summary>
        /// Numeric representation of the ID
        /// </summary>
        public virtual int IntID => int.Parse(ID);

        /// <summary>
        /// Text displayed when the charm gets glorified
        /// </summary>
        public abstract string GodText { get; set; }

        /// <summary>
        /// Whether or not the exaltation can be upgraded
        /// </summary>
        /// <returns></returns>
        public abstract bool CanUpgrade();

        /// <summary>
        /// Unlocks the exaltation and applies it to the charm
        /// </summary>
        public void Upgrade() 
        {
            //SharedData.Log($"Upgrading {Name}");
            ModHooks.SetPlayerBoolHook += CheckEquipped;

            if (PlayerData.instance.GetBool($"equippedCharm_{IntID}"))
            {
                Equip();
            }
        }

        /// <summary>
        /// Resets the exaltation when a new game is loaded
        /// </summary>
        public void Reset() 
        {
            //SharedData.Log($"Resetting {Name}");
            ModHooks.SetPlayerBoolHook -= CheckEquipped;
            Unequip();
        }

        /// <summary>
        /// Tracks when the related charm is equipped or unequipped
        /// </summary>
        /// <param name="name"></param>
        /// <param name="orig"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        internal bool CheckEquipped(string name, bool orig)
        {
            if (name.Equals($"equippedCharm_{IntID}"))
            {
                if (orig)
                {
                    //SharedData.Log($"Equipping {Name}");
                    Equip();
                }
                else
                {
                    //SharedData.Log($"Unequipping {Name}");
                    Unequip();
                }
            }

            return orig;
        }

        /// <summary>
        /// Tracks whether or not the exaltation is active
        /// </summary>
        public bool IsEquipped = false;

        /// <summary>
        /// Applies the exaltation when the charm is equipped
        /// </summary>
        public virtual void Equip()
        {
            if (IsEquipped) // VERY IMPORTANT that we don't equip twice
            {
                return;
            }

            //SharedData.Log($"Equipping {Name}");
            IsEquipped = true;
        }

        /// <summary>
        /// Removes the exaltation when the charm is unequipped
        /// </summary>
        public virtual void Unequip()
        {
            //SharedData.Log($"Unequipping {Name}");
            IsEquipped = false;
        }
    }
}