using Modding;
using UnityEngine;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Weaversong
    /// </summary>
    public class BeastsCall : Exaltation
    {
        public override string Name { get; set; } = "Beast's Call";
        public override string Description { get; set; } = "This web-covered charm carries the cry of the Beast.\n\n" +
                                                            "Summons many weaverlings to give the bearer companionship and protection.";
        public override string ID { get; set; } = "39";

        public override string GodText { get; set; } = "god protector";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.statueStateHornet2.completedTier2;
        }

        public override void Equip()
        {
            base.Equip();
            ModHooks.ObjectPoolSpawnHook += SpawnWeaverlings;
        }

        public override void Unequip()
        {
            ModHooks.ObjectPoolSpawnHook -= SpawnWeaverlings;
        }

        /// <summary>
        /// Beast's Call increases the number of weaverlings created by Weaversong
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        private GameObject SpawnWeaverlings(GameObject gameObject)
        {
            if (gameObject.name.Contains("Weaverling"))
            {
                // Weaversong is worth 2 notches, so we can spend our 2 notches to double the number of weaverlings
                int loops = 1;

                // If Charm Changer adjusts the cost, we need to plan accordingly
                if (SharedData.charmChanger.IsEnabled())
                {
                    // If it costs 1, we want a 200% chance
                    // If it still costs 2, we want a 100% chance
                    // If it costs 3, we want a 66.7% chance
                    float cost = SharedData.charmChanger.GetCharmNotches(IntID, PlayerData.instance.GetInt("charmCost_39"), 0.5f);
                    int chance = (int)(2 * 100 / cost);

                    // If we get a chance greater than 100%, we want a guarantee of spawning weaverlings, and a chance of spawning more after that
                    loops = 0;
                    while (chance >= 100)
                    {
                        loops++;
                        chance -= 100;
                    }
                    //ExaltationExpanded.Instance.Log($"Beast's Call - Guaranteed loops: {loops}");

                    if (chance > 0)
                    {
                        int random = UnityEngine.Random.Range(1, 101);
                        //ExaltationExpanded.Instance.Log($"Beast's Call - {random} vs {100 - chance}");
                        if (random <= 100 - chance)
                        {
                            loops++;
                        }
                    }
                }

                for (int i = 0; i < loops; i++)
                {
                    _ = GameObject.Instantiate(gameObject,
                                                new Vector3(HeroController.instance.transform.GetPositionX(),
                                                            HeroController.instance.transform.GetPositionY()),
                                                            Quaternion.identity);
                }
            }

            return gameObject;
        }
    }
}