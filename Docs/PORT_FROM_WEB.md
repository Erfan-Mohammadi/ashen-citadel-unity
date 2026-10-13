# Port map: HTML5 → Unity

| Web (game.js) | Unity |
|---------------|--------|
| Game.resources | ResourceManager.amounts |
| Game.production | ResourceManager.production |
| Game.heat* | HeatSystem |
| Game.buildings | BuildingManager.levels |
| Game.upgradeBuilding | BuildingManager.TryUpgrade |
| Game.heroes | HeroManager.heroes |
| Game.startDefenseWave | DefenseCombat.StartWave |
| Game.missions | MissionManager.missions |
| localStorage | SaveSystem + PlayerPrefs |

Balance constants kept identical.
