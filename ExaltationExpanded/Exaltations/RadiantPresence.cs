namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Dream Wielder
    /// </summary>
    public class RadiantPresence : Exaltation
    {
        public override string Name => "Radiant Presence";
        public override string Description => "Ephemeral manifestation of a higher being's power over dreams.\n\n" +
                                                "Allows the bearer to charge the Dream Nail faster and collect much more SOUL when striking foes.";
        public override string ID => "30";

        public override string GodText => "god of betrayal";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.statueStateXero.completedTier3;
        }

        public override void Upgrade()
        {
            base.Upgrade();
            On.EnemyDreamnailReaction.RecieveDreamImpact += IncreaseSoulGain;
        }

        public override void Reset()
        {
            base.Reset();
            On.EnemyDreamnailReaction.RecieveDreamImpact -= IncreaseSoulGain;
        }

        /// <summary>
        /// Radiant Presence increases the player's SOUL by 30% of Dream Wielder's default amount
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        private void IncreaseSoulGain(On.EnemyDreamnailReaction.orig_RecieveDreamImpact orig, EnemyDreamnailReaction self)
        {
            //SharedData.Log("Enemy dream nailed");

            if (PlayerData.instance.equippedCharm_30)
            {
                //SharedData.Log("Radiant nail activated");
                int defaultDreamWielderGain = 66;
                double bonus = 0.3 * defaultDreamWielderGain;

                //SharedData.Log("Increasing soul");
                PlayerData.instance.AddMPCharge((int)bonus);
            }

            //SharedData.Log("Calling orig");
            orig(self);
            //SharedData.Log("Dream nail complete");
        }
    }
}