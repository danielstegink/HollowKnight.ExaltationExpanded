# Exaltation Expanded
A supplement to the Exaltation mod created by Xhuis (and ported/updated by Théodore). 

This mod adds exaltations to the remaining charms as well as a handful of balance patches. The patches can be toggled via the menu.

Details on each exaltation can be found in the SPOILERS file and info on the various patches can be found in the PATCH SPOILERS file.

The patch notes can be found in the UPDATE HISTORY file.

## Thanks
Thank you to Xhuis, Théodore and everyone else who made Exaltation; it is an incredibly creative addition to the game and I would have never thought to create it.

Huge shout out to the Hollow Knight modding community as a whole. This was an especially intensive project and I was lost on multiple occasions without their help.

Thank you to Volt for help with bug detection and maintenance.

## Patch Notes
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
