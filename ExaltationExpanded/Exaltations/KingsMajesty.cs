namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// Replaces Royal Crest and King's Honour
    /// </summary>
    public class KingsMajesty : RoyalCrest
    {
        public override string Name => "King's Majesty";
        public override string Description => "Symbolizes the loyalty and kindness of two of the Pale King's greatest knights.\n\n" +
                                                "Shrouds the bearer in a regal and overwhelming aura.";

        public override string GodText => "gods of moss and honour";
    }
}