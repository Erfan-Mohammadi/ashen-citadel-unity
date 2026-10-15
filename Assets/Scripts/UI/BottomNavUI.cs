using UnityEngine;
using UnityEngine.UI;

namespace AshenCitadel.UI
{
    public class BottomNavUI : MonoBehaviour
    {
        public Button buildButton, heroesButton, defendButton, missionsButton, allianceButton;
        public BuildingPanelUI buildingPanel;
        public HeroPanelUI heroPanel;
        public MissionPanelUI missionPanel;
        public DefenseUI defenseUI;
        public ShopPanelUI shopPanel;

        void Start()
        {
            Wire(buildButton, OnBuild);
            Wire(heroesButton, OnHeroes);
            Wire(defendButton, OnDefend);
            Wire(missionsButton, OnMissions);
            Wire(allianceButton, OnAlliance);
        }

        static void Wire(Button b, UnityEngine.Events.UnityAction a)
        {
            if (b == null) return;
            b.onClick.RemoveAllListeners();
            b.onClick.AddListener(a);
        }

        void CloseAllPanels()
        {
            buildingPanel?.Hide();
            heroPanel?.Hide();
            missionPanel?.Hide();
            shopPanel?.Hide();
        }

        public void OnBuild()
        {
            CloseAllPanels();
            buildingPanel?.ShowCitadel();
        }

        public void OnHeroes()
        {
            bool open = heroPanel != null && heroPanel.IsOpen;
            CloseAllPanels();
            if (!open) heroPanel?.Show();
        }

        public void OnDefend()
        {
            CloseAllPanels();
            defenseUI?.Open();
        }

        public void OnMissions()
        {
            bool open = missionPanel != null && missionPanel.IsOpen;
            CloseAllPanels();
            if (!open) missionPanel?.Show();
        }

        public void OnAlliance()
        {
            if (shopPanel != null)
            {
                if (shopPanel.IsOpen) shopPanel.Hide();
                else { CloseAllPanels(); shopPanel.Show(); }
            }
            else
                UINotifier.Instance?.Show("فروشگاه به‌زودی");
        }
    }
}
