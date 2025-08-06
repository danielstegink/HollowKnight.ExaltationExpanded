# Exaltation Expanded
A supplement to the Exaltation mod created by Xhuis (and ported/updated by Théodore). 

This mod adds exaltations to the remaining charms as well as a handful of balance patches. The patches can be toggled via the menu.

Details on each exaltation can be found in the SPOILERS file and info on the various patches can be found in the PATCH SPOILERS file.

The patch notes can be found in the UPDATE HISTORY file.

## Thanks
Thank you to Xhuis, Théodore and everyone else who made Exaltation; it is an incredibly creative addition to the game and I would have never thought to create it.

Huge shout out to the Hollow Knight modding community as a whole. This was an especially intensive project and I was lost on multiple occasions without their help.

Thank you to Volt and BenjaLP211 for help with testing and feedback.

## Patch Notes
1.2.0.0
-	Integrated with DanielSteginkUtils
-	Complete Mask gives extra masks instead of a healable Lifeblood Mask
-	Vessel's Might deals its bonus damage as a separate spell attack
-	Lord Nail uses MOP's animation so it doesn't appear smaller
-	Radiant Presence stores excess SOUL and uses it to passively heal damage
-	Rebalanced exaltations to be worth 2 extra notches instead of 1
-	The following charms have been weakened: 
	- Grubberfly's Requiem
	- Vessel's Might
	- Mark of Betrayal
	- Nexus of Light
-	The following charms have been strengthened: 
	- Complete Mask
	- Golden Touch
	- Crushing Blow
	- Lord Nail
	- Flukeswarm
	- Royal Crest / King's Majesty
	- Abyssal Bloom
	- Blessing of Unn
	- Radiant Presence
	- Knightmare
	- Loving Lullaby
-	Renamed Balance patch as Power patch
	- Swift Focus and Steel Tempest are more powerful than before

1.1.1.0
-	Fixed integration bug where Pale Court was a required mod

1.1.0.0
-	Fixed Flukeswarm
-	Modified Vessel's Might to be more balanced
-	Added a new patch: Void Soul, which makes it possible to have Void Heart and Lordsoul at the same time
-	Reworked menu to have a nested structure
-	Cleaned up Pale Court integration code

1.0.2.0
-	Reduced logging
-	General cleanup
-	Changed Vessel's Might to use HeroController instead of PlayerData to reduce SOUL, so the UI updates properly
-	Modified Save file code to use UnityEngine.Application.persistentDataPath, making it compatible with non-Windows products
-	Fixed bug preventing downward slashes from being buffed
-	Moved .gitignore so .vs folder isn't pushed to Git

1.0.1.0
-	Fixes bug where Loving Lullaby would trigger even if Carefree Melody wasn't equipped
