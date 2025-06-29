# Patch Notes
1.0.2.0
-	Reduced logging
-	General cleanup
-	Changed Vessel's Might to use HeroController instead of PlayerData to reduce SOUL, so the UI updates properly
-	Modified Save file code to use UnityEngine.Application.persistentDataPath, making it compatible with non-Windows products
-	Fixed bug preventing downward slashes from being buffed
-	Moved .gitignore so .vs folder isn't pushed to Git

1.0.1.0
-	Fixes bug where Loving Lullaby would trigger even if Carefree Melody wasn't equipped