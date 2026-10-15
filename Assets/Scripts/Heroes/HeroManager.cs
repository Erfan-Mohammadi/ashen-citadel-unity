using System;
using System.Collections.Generic;
using AshenCitadel.Core;
using UnityEngine;

namespace AshenCitadel.Heroes
{
    public class HeroManager : MonoBehaviour
    {
        public static HeroManager Instance { get; private set; }

        public List<HeroState> heroes = new List<HeroState>
        {
            new HeroState { id = "kaveh", nameFa = "کاوه آهنین", role = HeroRole.Vanguard, level = 1, stars = 1, hp = 420, atk = 35, def = 55, skillFa = "سپر فولادی", owned = true },
            new HeroState { id = "arash", nameFa = "آرش آتشین", role = HeroRole.Destroyer, level = 1, stars = 1, hp = 280, atk = 78, def = 22, skillFa = "تیر آتشین", owned = true },
            new HeroState { id = "anahita", nameFa = "آناهیتا", role = HeroRole.Support, level = 1, stars = 1, hp = 310, atk = 28, def = 30, skillFa = "چشمه حیات", owned = false },
            new HeroState { id = "rostam", nameFa = "رستم سایه", role = HeroRole.Destroyer, level = 1, stars = 2, hp = 300, atk = 95, def = 28, skillFa = "ضربه سایه‌ای", owned = false }
        };

        public event Action OnHeroesChanged;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public HeroState Find(string id) => heroes.Find(h => h.id == id);
        public List<HeroState> OwnedHeroes() => heroes.FindAll(h => h.owned);

        public bool Unlock(string id, float crystalCost = 6f)
        {
            var h = Find(id);
            if (h == null || h.owned) return false;
            if (ResourceManager.Instance.Get(ResourceType.Crystal) < crystalCost) return false;
            ResourceManager.Instance.Add(ResourceType.Crystal, -crystalCost);
            h.owned = true;
            Core.AudioManager.Instance?.Play("buy");
            OnHeroesChanged?.Invoke();
            SaveSystem.Instance?.Save();
            return true;
        }

        public bool LevelUp(string id)
        {
            var h = Find(id);
            if (h == null || !h.owned || h.level >= 20) return false;
            float food = Mathf.Floor(35 * Mathf.Pow(1.32f, h.level - 1));
            float iron = Mathf.Floor(6 * Mathf.Pow(1.35f, h.level - 1));
            float crystal = h.level >= 5 ? Mathf.Floor(1 + (h.level - 4) * 0.4f) : 0;
            var cost = new Dictionary<ResourceType, float> { { ResourceType.Food, food }, { ResourceType.Iron, iron } };
            if (crystal > 0) cost[ResourceType.Crystal] = crystal;
            if (!ResourceManager.Instance.TrySpend(cost)) return false;
            h.level++;
            float g = 1 + h.stars * 0.03f;
            h.hp = Mathf.Floor(h.hp * (1.08f + g * 0.02f));
            h.atk = Mathf.Floor(h.atk * (1.07f + g * 0.02f));
            h.def = Mathf.Floor(h.def * (1.06f + g * 0.02f));
            Missions.MissionManager.Instance?.Track("hero");
            Core.AudioManager.Instance?.Play("upgrade");
            OnHeroesChanged?.Invoke();
            SaveSystem.Instance?.Save();
            return true;
        }

        public bool StarUp(string id)
        {
            var h = Find(id);
            if (h == null || !h.owned || h.stars >= 6) return false;
            var cost = new Dictionary<ResourceType, float> {
                { ResourceType.Crystal, 4 + h.stars * 3 },
                { ResourceType.Iron, 25 * h.stars },
                { ResourceType.Food, 70 * h.stars }
            };
            if (!ResourceManager.Instance.TrySpend(cost)) return false;
            h.stars++;
            h.hp = Mathf.Floor(h.hp * 1.18f);
            h.atk = Mathf.Floor(h.atk * 1.15f);
            h.def = Mathf.Floor(h.def * 1.12f);
            Missions.MissionManager.Instance?.Track("hero");
            Core.AudioManager.Instance?.Play("upgrade");
            OnHeroesChanged?.Invoke();
            SaveSystem.Instance?.Save();
            return true;
        }
    }
}
