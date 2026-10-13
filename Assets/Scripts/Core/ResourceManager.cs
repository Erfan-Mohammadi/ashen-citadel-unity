using System;
using System.Collections.Generic;
using UnityEngine;

namespace AshenCitadel.Core
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }

        [Serializable]
        public class ResourceAmounts
        {
            public float wood = 150;
            public float stone = 100;
            public float food = 180;
            public float coal = 70;
            public float iron = 25;
            public float crystal = 10;
        }

        [Serializable]
        public class ProductionRates
        {
            public float wood = 3.5f;
            public float stone = 1.8f;
            public float food = 4.5f;
            public float coal = 1.0f;
            public float iron = 0.4f;
        }

        public ResourceAmounts amounts = new ResourceAmounts();
        public ProductionRates production = new ProductionRates();
        public event Action OnResourcesChanged;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public float Get(ResourceType type) => type switch
        {
            ResourceType.Wood => amounts.wood,
            ResourceType.Stone => amounts.stone,
            ResourceType.Food => amounts.food,
            ResourceType.Coal => amounts.coal,
            ResourceType.Iron => amounts.iron,
            ResourceType.Crystal => amounts.crystal,
            _ => 0
        };

        public void Add(ResourceType type, float value) => Set(type, Get(type) + value);

        public bool TrySpend(Dictionary<ResourceType, float> cost)
        {
            foreach (var kv in cost)
                if (Get(kv.Key) < kv.Value) return false;
            foreach (var kv in cost)
                Add(kv.Key, -kv.Value);
            return true;
        }

        public void Set(ResourceType type, float value)
        {
            value = Mathf.Max(0, value);
            switch (type)
            {
                case ResourceType.Wood: amounts.wood = value; break;
                case ResourceType.Stone: amounts.stone = value; break;
                case ResourceType.Food: amounts.food = value; break;
                case ResourceType.Coal: amounts.coal = value; break;
                case ResourceType.Iron: amounts.iron = value; break;
                case ResourceType.Crystal: amounts.crystal = value; break;
            }
            OnResourcesChanged?.Invoke();
        }

        public void TickProduction(float heatPenaltyFactor = 1f)
        {
            amounts.wood += production.wood * heatPenaltyFactor;
            amounts.stone += production.stone * heatPenaltyFactor;
            amounts.food += production.food * heatPenaltyFactor;
            amounts.coal += production.coal;
            amounts.iron += production.iron;
            OnResourcesChanged?.Invoke();
        }
    }
}
