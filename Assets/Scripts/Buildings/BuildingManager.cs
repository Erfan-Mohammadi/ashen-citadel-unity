using System;
using System.Collections.Generic;
using AshenCitadel.Core;
using UnityEngine;

namespace AshenCitadel.Buildings
{
    public class BuildingManager : MonoBehaviour
    {
        public static BuildingManager Instance { get; private set; }

        public Dictionary<BuildingType, int> levels = new Dictionary<BuildingType, int>
        {
            { BuildingType.Citadel, 1 },
            { BuildingType.Woodcamp, 1 },
            { BuildingType.Farm, 1 },
            { BuildingType.Coalpit, 0 },
            { BuildingType.Barracks, 0 }
        };

        static readonly Dictionary<BuildingType, Dictionary<ResourceType, float>> BaseCosts =
            new Dictionary<BuildingType, Dictionary<ResourceType, float>>
            {
                { BuildingType.Citadel, new Dictionary<ResourceType, float> {
                    { ResourceType.Wood, 45 }, { ResourceType.Stone, 25 }, { ResourceType.Coal, 15 } } },
                { BuildingType.Woodcamp, new Dictionary<ResourceType, float> {
                    { ResourceType.Wood, 25 }, { ResourceType.Stone, 12 } } },
                { BuildingType.Farm, new Dictionary<ResourceType, float> {
                    { ResourceType.Wood, 20 }, { ResourceType.Stone, 8 } } },
                { BuildingType.Coalpit, new Dictionary<ResourceType, float> {
                    { ResourceType.Wood, 35 }, { ResourceType.Stone, 20 } } },
                { BuildingType.Barracks, new Dictionary<ResourceType, float> {
                    { ResourceType.Wood, 50 }, { ResourceType.Stone, 35 }, { ResourceType.Iron, 8 } } }
            };

        public event Action<BuildingType, int> OnBuildingUpgraded;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public int GetLevel(BuildingType type) => levels.TryGetValue(type, out var l) ? l : 0;

        public Dictionary<ResourceType, float> GetUpgradeCost(BuildingType type)
        {
            int lv = GetLevel(type);
            var result = new Dictionary<ResourceType, float>();
            if (!BaseCosts.TryGetValue(type, out var baseCost)) return result;
            foreach (var kv in baseCost)
                result[kv.Key] = Mathf.Floor(kv.Value * Mathf.Pow(1.42f, lv));
            return result;
        }

        public bool TryUpgrade(BuildingType type)
        {
            var cost = GetUpgradeCost(type);
            if (ResourceManager.Instance == null || !ResourceManager.Instance.TrySpend(cost))
                return false;
            levels[type] = GetLevel(type) + 1;
            ApplyUpgradeEffects(type);
            OnBuildingUpgraded?.Invoke(type, levels[type]);
            Missions.MissionManager.Instance?.Track("upgrade");
            SaveSystem.Instance?.Save();
            return true;
        }

        void ApplyUpgradeEffects(BuildingType type)
        {
            var res = ResourceManager.Instance;
            var heat = HeatSystem.Instance;
            if (res == null) return;
            switch (type)
            {
                case BuildingType.Citadel:
                    heat?.IncreaseMax(18f);
                    if (heat != null) heat.heatProduction += 0.9f;
                    heat?.AddHeat(25f);
                    if (GetLevel(BuildingType.Citadel) == 2 && GetLevel(BuildingType.Coalpit) == 0)
                        levels[BuildingType.Coalpit] = 1;
                    if (GetLevel(BuildingType.Citadel) == 3 && GetLevel(BuildingType.Barracks) == 0)
                        levels[BuildingType.Barracks] = 1;
                    break;
                case BuildingType.Woodcamp:
                    res.production.wood += 1.6f;
                    break;
                case BuildingType.Farm:
                    res.production.food += 2.1f;
                    break;
                case BuildingType.Coalpit:
                    res.production.coal += 1.3f;
                    if (heat != null) heat.heatProduction += 0.55f;
                    break;
            }
        }
    }
}
