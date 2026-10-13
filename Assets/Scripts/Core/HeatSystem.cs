using System;
using UnityEngine;

namespace AshenCitadel.Core
{
    public class HeatSystem : MonoBehaviour
    {
        public static HeatSystem Instance { get; private set; }

        public float heat = 100f;
        public float heatMax = 100f;
        public float heatProduction = 2.4f;
        public float heatConsumption = 1.0f;

        public event Action OnHeatChanged;
        public float Percent => heatMax > 0 ? heat / heatMax : 0f;
        public bool IsCritical => Percent < 0.3f;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void Tick()
        {
            heat += heatProduction - heatConsumption;
            heat = Mathf.Clamp(heat, 0f, heatMax);
            OnHeatChanged?.Invoke();
        }

        public void AddHeat(float amount)
        {
            heat = Mathf.Clamp(heat + amount, 0f, heatMax);
            OnHeatChanged?.Invoke();
        }

        public void IncreaseMax(float amount)
        {
            heatMax += amount;
            OnHeatChanged?.Invoke();
        }

        public void ApplyDefeatPenalty()
        {
            heat = Mathf.Max(15f, heat - 10f);
            OnHeatChanged?.Invoke();
        }

        public float ProductionFactor =>
            Percent >= 0.3f ? 1f : Mathf.Lerp(0.5f, 1f, Percent / 0.3f);
    }
}
