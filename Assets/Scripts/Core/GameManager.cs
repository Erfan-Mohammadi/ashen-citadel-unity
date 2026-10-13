using UnityEngine;

namespace AshenCitadel.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public bool gameStarted;
        public bool tutorialDone;

        float _tickTimer;
        const float TickInterval = 1f;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            if (SaveSystem.Instance != null && SaveSystem.Instance.HasSave())
                SaveSystem.Instance.Load();
        }

        void Update()
        {
            if (!gameStarted) return;
            _tickTimer += Time.deltaTime;
            if (_tickTimer < TickInterval) return;
            _tickTimer -= TickInterval;
            Tick();
        }

        void Tick()
        {
            float factor = HeatSystem.Instance != null ? HeatSystem.Instance.ProductionFactor : 1f;
            ResourceManager.Instance?.TickProduction(factor);
            HeatSystem.Instance?.Tick();
            if (Missions.MissionManager.Instance != null && ResourceManager.Instance != null)
                Missions.MissionManager.Instance.Track("collect", ResourceManager.Instance.production.food);
        }

        public void StartGame(bool fromSave = false)
        {
            gameStarted = true;
            SaveSystem.Instance?.Save();
        }
    }
}
