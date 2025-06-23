namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// Template class for the exalted versions of charms
    /// </summary>
    public class Exaltation
    {
        /// <summary>
        /// Name of the exalted charm
        /// </summary>
        public virtual string Name => "";

        /// <summary>
        /// Cost of the exalted charm
        /// </summary>
        public virtual int Cost => 0;

        /// <summary>
        /// Description of the exalted charm
        /// </summary>
        public virtual string Description => "";

        /// <summary>
        /// Numeric ID of the original charm
        /// </summary>
        public virtual string ID => "";

        /// <summary>
        /// Numeric representation of the ID
        /// </summary>
        public virtual int IntID => int.Parse(ID);

        /// <summary>
        /// Text displayed when the charm gets glorified
        /// </summary>
        public virtual string GodText => "";

        /// <summary>
        /// Whether or not the exaltation can be upgraded
        /// </summary>
        /// <returns></returns>
        public virtual bool CanUpgrade() => false;

        /// <summary>
        /// Unlocks the exaltation and applies it to the charm
        /// </summary>
        public virtual void Upgrade() { }

        /// <summary>
        /// Resets the exaltation when a new game is loaded
        /// </summary>
        public virtual void Reset() { }
    }
}