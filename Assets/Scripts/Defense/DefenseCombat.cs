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

            int count = wave <= 2 ? 2 : Mathf.Min(2 + Mathf.FloorToInt(wave * 0.7f), 8);
            float baseHp = wave == 1 ? 28 : wave == 2 ? 40 : 50 + wave * 18;
            float baseSpeed = wave <= 2 ? 0.28f : 0.45f;
            for (int i = 0; i < count; i++)
            {
                enemies.Add(new EnemyUnit
                {
                    x = 50 + Random.Range(0f, 260f),
                    y = -30 - i * 50,
                    hp = baseHp, maxHp = baseHp,
                    speed = baseSpeed * Random.Range(0.9f, 1.15f)
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
            float dmgScale = wave <= 2 ? 0.09f : 0.05f;

            foreach (var e in enemies)
            {
                if (e.hp <= 0) continue;
                e.y += e.speed * dt * 0.4f;
                foreach (var h in heroes)
                {
                    if (h.hp <= 0) continue;
                    if (Mathf.Abs(e.x - h.x) < 55 && e.y > h.y - 45 && e.y < h.y + 20)
                        h.hp -= (wave <= 2 ? 0.12f : 0.3f) * dt;
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
                    if (d < best && d < 180) { best = d; target = e; }
                }
                if (target != null)
                {
                    target.hp -= h.atk * dmgScale * 8f;
                    h.atkCd = h.role == HeroRole.Destroyer ? 18 : 24;
                }
            }

            OnCombatUpdated?.Invoke();

            bool enemiesAlive = enemies.Exists(e => e.hp > 0);
            bool heroesAlive = heroes.Exists(h => h.hp > 0);
            bool breach = enemies.Exists(e => e.y > 350);

            if (!enemiesAlive)
            {
                running = false;
                ResourceManager.Instance?.Add(ResourceType.Wood, 40 + wave * 18);
                ResourceManager.Instance?.Add(ResourceType.Stone, 25 + wave * 12);
                ResourceManager.Instance?.Add(ResourceType.Crystal, 1 + wave / 2);
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
                ResourceManager.Instance?.Add(ResourceType.Food, -20);
                SaveSystem.Instance?.Save();
                Core.AudioManager.Instance?.Play("lose");
                OnWaveLost?.Invoke();
            }
        }
    }
}
