using System;
using System.Collections.Generic;
using AshenCitadel.Buildings;
using AshenCitadel.Heroes;
using AshenCitadel.Missions;
using UnityEngine;

namespace AshenCitadel.Core
{
    [Serializable]
    public class SaveData
    {
        public int version = 10;
        public float wood, stone, food, coal, iron, crystal;
        public float heat, heatMax, heatProduction, heatConsumption;
        public float prodWood, prodStone, prodFood, prodCoal, prodIron;
        public int citadel, woodcamp, farm, coalpit, barracks;
        public int defenseWave;
        public bool tutorialDone;
        public string missionDay;
    }

    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance { get; private set; }
        const string Key = "ashen_citadel_unity_save";

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public bool HasSave() => PlayerPrefs.HasKey(Key);

        public void Save()
        {
            var r = ResourceManager.Instance;
            var h = HeatSystem.Instance;
            var b = BuildingManager.Instance;
            var data = new SaveData
            {
                wood = r?.amounts.wood ?? 0,
                stone = r?.amounts.stone ?? 0,
                food = r?.amounts.food ?? 0,
                coal = r?.amounts.coal ?? 0,
                iron = r?.amounts.iron ?? 0,
                crystal = r?.amounts.crystal ?? 0,
                heat = h?.heat ?? 100,
                heatMax = h?.heatMax ?? 100,
                heatProduction = h?.heatProduction ?? 2.4f,
                heatConsumption = h?.heatConsumption ?? 1f,
                prodWood = r?.production.wood ?? 0,
                prodStone = r?.production.stone ?? 0,
                prodFood = r?.production.food ?? 0,
                prodCoal = r?.production.coal ?? 0,
                prodIron = r?.production.iron ?? 0,
                citadel = b?.GetLevel(BuildingType.Citadel) ?? 1,
                woodcamp = b?.GetLevel(BuildingType.Woodcamp) ?? 1,
                farm = b?.GetLevel(BuildingType.Farm) ?? 1,
                coalpit = b?.GetLevel(BuildingType.Coalpit) ?? 0,
                barracks = b?.GetLevel(BuildingType.Barracks) ?? 0,
                defenseWave = Defense.DefenseCombat.Instance?.wave ?? 1,
                tutorialDone = GameManager.Instance?.tutorialDone ?? false,
                missionDay = MissionManager.Instance?.missionDay
            };
            PlayerPrefs.SetString(Key, JsonUtility.ToJson(data));
            PlayerPrefs.Save();
        }

        public void Load()
        {
            if (!HasSave()) return;
            var data = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(Key));
            if (data == null || data.version < 2) return;

            var r = ResourceManager.Instance;
            if (r != null)
            {
                r.amounts.wood = data.wood; r.amounts.stone = data.stone; r.amounts.food = data.food;
                r.amounts.coal = data.coal; r.amounts.iron = data.iron; r.amounts.crystal = data.crystal;
                r.production.wood = data.prodWood; r.production.stone = data.prodStone;
                r.production.food = data.prodFood; r.production.coal = data.prodCoal; r.production.iron = data.prodIron;
            }
            var h = HeatSystem.Instance;
            if (h != null)
            {
                h.heat = data.heat; h.heatMax = data.heatMax;
                h.heatProduction = data.heatProduction; h.heatConsumption = data.heatConsumption;
            }
            var b = BuildingManager.Instance;
            if (b != null)
            {
                b.levels[BuildingType.Citadel] = data.citadel;
                b.levels[BuildingType.Woodcamp] = data.woodcamp;
                b.levels[BuildingType.Farm] = data.farm;
                b.levels[BuildingType.Coalpit] = data.coalpit;
                b.levels[BuildingType.Barracks] = data.barracks;
            }
            if (Defense.DefenseCombat.Instance != null)
                Defense.DefenseCombat.Instance.wave = data.defenseWave;
            if (GameManager.Instance != null)
                GameManager.Instance.tutorialDone = data.tutorialDone;
        }

        public void DeleteSave()
        {
            PlayerPrefs.DeleteKey(Key);
            PlayerPrefs.Save();
        }
    }
}
