using System;
using System.Collections.Generic;
using AshenCitadel.Core;
using AshenCitadel.Heroes;
using UnityEngine;

namespace AshenCitadel.Defense
{
    public class DefenseCombat : MonoBehaviour
    {
        public static DefenseCombat Instance { get; private set; }
        public const int MaxBalancedWave = 20;
        public int wave = 1;
        public bool running;

        public class CombatUnit
        {
            public string name;
            public float x, y, hp, maxHp, atk;
            public HeroRole role;
            public float atkCd;
        }

        public class EnemyUnit
        {
            public float x, y, hp, maxHp, speed;
            public bool isElite;
        }

        public List<CombatUnit> heroes = new List<CombatUnit>();
        public List<EnemyUnit> enemies = new List<EnemyUnit>();

        public event Action OnWaveStarted;
        public event Action OnWaveWon;
        public event Action OnWaveLost;
        public event Action OnCombatUpdated;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public static int EnemyCount(int w)
        {
            w = Mathf.Max(1, w);
            if (w <= 2) return 2;
            if (w <= 5) return 2 + (w - 2);
            if (w <= 10) return 4 + (w - 5) / 2;
            if (w <= 15) return 6 + (w - 10) / 2;
            return Mathf.Min(8 + (w - 15) / 2, 12);
        }

        public static float EnemyHp(int w)
        {
            w = Mathf.Max(1, w);
            if (w == 1) return 26f;
            if (w == 2) return 34f;
            if (w <= 5) return 30f + w * 10f;
            if (w <= 10) return 70f + (w - 5) * 14f;
            if (w <= 15) return 140f + (w - 10) * 18f;
            return 230f + (w - 15) * 22f;
        }

        public static float EnemySpeed(int w)
        {
            if (w <= 2) return 0.26f;
            if (w <= 5) return 0.30f;
            if (w <= 10) return 0.34f;
            if (w <= 15) return 0.38f;
            return 0.42f;
        }

        public static float HeroDamageScale(int w)
        {
            if (w <= 2) return 0.11f;
            if (w <= 5) return 0.09f;
            if (w <= 10) return 0.075f;
            if (w <= 15) return 0.065f;
            return 0.055f;
        }

        public static bool IsBossWave(int w) => w > 0 && w % 5 == 0;

        public void StartWave()
        {
            if (running) return;
            running = true;
            heroes.Clear();
            enemies.Clear();

            var owned = HeroManager.Instance?.OwnedHeroes();
            if (owned != null)
            {
                for (int i = 0; i < owned.Count; i++)
                {
                    var h = owned[i];
                    heroes.Add(new CombatUnit
                    {
                        name = h.nameFa, x = 55 + i * 95, y = 310,
                        hp = h.hp, maxHp = h.hp, atk = h.atk, role = h.role, atkCd = 0
                    });
                }
            }
            if (heroes.Count == 0)
            {
                heroes.Add(new CombatUnit
                {
                    name = "نگهبان", x = 160, y = 310,
                    hp = 350, maxHp = 350, atk = 40, role = HeroRole.Vanguard, atkCd = 0
                });
            }

            int count = EnemyCount(wave);
            float baseHp = EnemyHp(wave);
            float baseSpeed = EnemySpeed(wave);
            bool boss = IsBossWave(wave);

            for (int i = 0; i < count; i++)
            {
                bool elite = boss && i == 0;
                float hp = elite ? baseHp * 2.2f : baseHp;
                float spd = elite ? baseSpeed * 0.85f : baseSpeed;
                enemies.Add(new EnemyUnit
                {
                    x = 50 + Random.Range(0f, 260f),
                    y = -30 - i * 48,
                    hp = hp, maxHp = hp,
                    speed = spd * Random.Range(0.92f, 1.12f),
                    isElite = elite
                });
            }

            Core.AudioManager.Instance?.Play("wave");
            OnWaveStarted?.Invoke();
            OnCombatUpdated?.Invoke();
        }

        void Update()
        {
            if (!running) return;
            float dt = Time.deltaTime * 60f;
            float dmgScale = HeroDamageScale(wave);

            foreach (var e in enemies)
            {
                if (e.hp <= 0) continue;
                e.y += e.speed * dt * 0.4f;
                foreach (var h in heroes)
                {
                    if (h.hp <= 0) continue;
                    if (Mathf.Abs(e.x - h.x) < 55 && e.y > h.y - 45 && e.y < h.y + 20)
                    {
                        float touch = wave <= 3 ? 0.10f : wave <= 10 ? 0.18f : 0.26f;
                        if (e.isElite) touch *= 1.35f;
                        h.hp -= touch * dt;
                    }
                }
            }

            foreach (var h in heroes)
            {
                if (h.hp <= 0) continue;
                h.atkCd -= dt;
                if (h.atkCd > 0) continue;
                EnemyUnit target = null;
                float best = 9999;
                foreach (var e in enemies)
                {
                    if (e.hp <= 0 || e.y < 40) continue;
                    float d = Mathf.Abs(e.x - h.x) + Mathf.Abs(e.y - h.y);
                    if (d < best && d < 190) { best = d; target = e; }
                }
                if (target != null)
                {
                    float mult = h.role == HeroRole.Destroyer ? 1.15f : h.role == HeroRole.Support ? 0.75f : 1f;
                    target.hp -= h.atk * dmgScale * 8f * mult;
                    h.atkCd = h.role == HeroRole.Destroyer ? 16 : h.role == HeroRole.Support ? 28 : 22;
                    if (h.role == HeroRole.Support)
                    {
                        foreach (var ally in heroes)
                            if (ally.hp > 0 && ally.hp < ally.maxHp)
                                ally.hp = Mathf.Min(ally.maxHp, ally.hp + h.atk * 0.04f);
                    }
                }
            }

            OnCombatUpdated?.Invoke();

            bool enemiesAlive = enemies.Exists(e => e.hp > 0);
            bool heroesAlive = heroes.Exists(h => h.hp > 0);
            bool breach = enemies.Exists(e => e.y > 350);

            if (!enemiesAlive)
            {
                running = false;
                GrantRewards();
                wave++;
                Missions.MissionManager.Instance?.Track("defend");
                SaveSystem.Instance?.Save();
                Core.AudioManager.Instance?.Play("win");
                OnWaveWon?.Invoke();
            }
            else if (!heroesAlive || breach)
            {
                running = false;
                HeatSystem.Instance?.ApplyDefeatPenalty();
                ResourceManager.Instance?.Add(ResourceType.Food, -Mathf.Min(20, 8 + wave));
                SaveSystem.Instance?.Save();
                Core.AudioManager.Instance?.Play("lose");
                OnWaveLost?.Invoke();
            }
        }

        void GrantRewards()
        {
            var r = ResourceManager.Instance;
            if (r == null) return;
            int w = wave;
            r.Add(ResourceType.Wood, 35 + w * 16);
            r.Add(ResourceType.Stone, 22 + w * 11);
            r.Add(ResourceType.Food, 15 + w * 6);
            int crystal = (w / 3) + (IsBossWave(w) ? 3 : 0);
            if (crystal > 0) r.Add(ResourceType.Crystal, crystal);
            if (w >= 5) r.Add(ResourceType.Iron, 3 + w / 2);
            if (IsBossWave(w)) HeatSystem.Instance?.AddHeat(12f);
        }
    }
}
