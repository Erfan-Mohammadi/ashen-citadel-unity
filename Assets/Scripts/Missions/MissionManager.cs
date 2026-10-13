using System;
using System.Collections.Generic;
using AshenCitadel.Core;
using UnityEngine;

namespace AshenCitadel.Missions
{
    [Serializable]
    public class Mission
    {
        public string id;
        public string titleFa;
        public string descFa;
        public float target;
        public float progress;
        public bool claimed;
        public Dictionary<ResourceType, float> reward;
    }

    public class MissionManager : MonoBehaviour
    {
        public static MissionManager Instance { get; private set; }
        public List<Mission> missions = new List<Mission>();
        public string missionDay;
        public event Action OnMissionsChanged;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start() => EnsureToday();

        string TodayKey()
        {
            var d = DateTime.Now;
            return $"{d.Year}-{d.Month}-{d.Day}";
        }

        public void EnsureToday()
        {
            string today = TodayKey();
            if (missionDay == today && missions != null && missions.Count > 0) return;
            missionDay = today;
            missions = DefaultMissions();
            OnMissionsChanged?.Invoke();
        }

        List<Mission> DefaultMissions() => new List<Mission>
        {
            new Mission { id = "upgrade", titleFa = "ارتقای ساختمان", descFa = "یک ساختمان را ارتقا بده", target = 1,
                reward = new Dictionary<ResourceType, float> { { ResourceType.Wood, 60 }, { ResourceType.Crystal, 2 } } },
            new Mission { id = "defend", titleFa = "دفاع موفق", descFa = "یک موج دفاع را ببر", target = 1,
                reward = new Dictionary<ResourceType, float> { { ResourceType.Stone, 50 }, { ResourceType.Crystal, 2 } } },
            new Mission { id = "collect", titleFa = "جمع‌آوری غذا", descFa = "۴۰۰ غذا جمع کن", target = 400,
                reward = new Dictionary<ResourceType, float> { { ResourceType.Food, 120 }, { ResourceType.Iron, 15 } } },
            new Mission { id = "hero", titleFa = "تقویت قهرمان", descFa = "سطح یا ستاره قهرمان را بالا ببر", target = 1,
                reward = new Dictionary<ResourceType, float> { { ResourceType.Crystal, 3 } } }
        };

        public void Track(string id, float amount = 1f)
        {
            EnsureToday();
            var m = missions.Find(x => x.id == id);
            if (m == null || m.claimed) return;
            m.progress = Mathf.Min(m.target, m.progress + amount);
            OnMissionsChanged?.Invoke();
        }

        public bool Claim(string id)
        {
            var m = missions.Find(x => x.id == id);
            if (m == null || m.claimed || m.progress < m.target) return false;
            foreach (var kv in m.reward)
                ResourceManager.Instance?.Add(kv.Key, kv.Value);
            m.claimed = true;
            OnMissionsChanged?.Invoke();
            SaveSystem.Instance?.Save();
            return true;
        }
    }
}
