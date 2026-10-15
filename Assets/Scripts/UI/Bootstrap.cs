using AshenCitadel.Core;
using AshenCitadel.Buildings;
using AshenCitadel.Heroes;
using AshenCitadel.Defense;
using AshenCitadel.Missions;
using UnityEngine;

namespace AshenCitadel.UI
{
    public class Bootstrap : MonoBehaviour
    {
        void Awake()
        {
            Ensure<GameManager>();
            Ensure<ResourceManager>();
            Ensure<HeatSystem>();
            Ensure<BuildingManager>();
            Ensure<HeroManager>();
            Ensure<DefenseCombat>();
            Ensure<MissionManager>();
            Ensure<SaveSystem>();
            Ensure<UINotifier>();

            if (FindObjectOfType<AutoUIBuilder>() == null)
            {
                var ui = new GameObject("AutoUIBuilder");
                ui.AddComponent<AutoUIBuilder>();
            }
        }

        void Ensure<T>() where T : Component
        {
            if (FindObjectOfType<T>() != null) return;
            var go = new GameObject(typeof(T).Name);
            go.AddComponent<T>();
            DontDestroyOnLoad(go);
        }

        public void OnClickStartGame()
        {
            bool fromSave = SaveSystem.Instance != null && SaveSystem.Instance.HasSave();
            GameManager.Instance?.StartGame(fromSave);
            UINotifier.Instance?.Show(fromSave ? "خوش برگشتی، فرمانده!" : "به دژ خاکستر خوش آمدی، فرمانده!");
        }
    }
}
