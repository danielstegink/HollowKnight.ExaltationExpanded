 namespace ExaltationExpanded.Patches
{
    /// <summary>
    /// Template for patches for Exaltation
    /// </summary>
    public interface ExaltationPatch
    {
        /// <summary>
        /// Applies patch-related hooks to the game
        /// </summary>
        public void ApplyHooks();
    }
}