using GlobalEnums;
using System;
using UnityEngine;

namespace ExaltationExpanded.Exaltations
{
    /// <summary>
    /// This exaltation replaces Mark of Pride
    /// </summary>
    public class MarkOfBetrayal : Exaltation
    {
        public override string Name => "Mark of Betrayal";
        public override string Description => "The mark of one who betrayed their tribe for power.\n\n" +
                                                "Greatly increases the range of the bearer's nail, and gives their attacks a chance to send shockwaves towards their enemies.";
        public override string ID => "13";

        public override string GodText => "god of anger";

        public override bool CanUpgrade()
        {
            return PlayerData.instance.statueStateTraitorLord.completedTier2;
        }

        public override void Upgrade()
        {
            base.Upgrade();
            On.HeroController.Attack += SendElegyBeam;
        }

        public override void Reset()
        {
            base.Reset();
            On.HeroController.Attack -= SendElegyBeam;
        }

        /// <summary>
        /// MOB gives nail attacks a 20% chance to trigger the beam attack associated with Grubberfly's Elegy
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="attackDir"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void SendElegyBeam(On.HeroController.orig_Attack orig, HeroController self, AttackDirection attackDir)
        {
            orig(self, attackDir);

            if (PlayerData.instance.equippedCharm_13)
            {
                int random = UnityEngine.Random.Range(1, 101);
                if (random <= 20)
                {
                    GameObject grubberFlyBeam;

                    switch (attackDir)
                    {
                        case AttackDirection.normal:
                            if (self.transform.localScale.x < 0f)
                            {
                                grubberFlyBeam = self.grubberFlyBeamPrefabR.Spawn(self.transform.position);
                            }
                            else
                            {
                                grubberFlyBeam = self.grubberFlyBeamPrefabL.Spawn(self.transform.position);
                            }

                            grubberFlyBeam.transform.SetScaleY(1.35f);

                            break;
                        case AttackDirection.upward:
                            grubberFlyBeam = self.grubberFlyBeamPrefabU.Spawn(self.transform.position);
                            grubberFlyBeam.transform.SetScaleY(self.transform.localScale.x);
                            grubberFlyBeam.transform.localEulerAngles = new Vector3(0f, 0f, 270f);
                            grubberFlyBeam.transform.SetScaleY(grubberFlyBeam.transform.localScale.y * 1.35f);

                            break;
                        case AttackDirection.downward:
                            grubberFlyBeam = self.grubberFlyBeamPrefabD.Spawn(self.transform.position);
                            grubberFlyBeam.transform.SetScaleY(self.transform.localScale.x);
                            grubberFlyBeam.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
                            grubberFlyBeam.transform.SetScaleY(grubberFlyBeam.transform.localScale.y * 1.35f);

                            break;
                    }
                }
            }
        }
    }
}