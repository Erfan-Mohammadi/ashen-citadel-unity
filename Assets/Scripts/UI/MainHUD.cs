using UnityEngine;

namespace AshenCitadel.UI
{
    public class MainHUD : MonoBehaviour
    {
        public StartScreenUI startScreen;
        public ResourceBarUI resourceBar;
        public HeatBarUI heatBar;
        public BottomNavUI bottomNav;
        public BuildingPanelUI buildingPanel;
        public HeroPanelUI heroPanel;
        public MissionPanelUI missionPanel;
        public DefenseUI defenseUI;
        public ToastUI toast;

        void Start()
        {
            if (bottomNav != null)
            {
                if (bottomNav.buildingPanel == null) bottomNav.buildingPanel = buildingPanel;
                if (bottomNav.heroPanel == null) bottomNav.heroPanel = heroPanel;
                if (bottomNav.missionPanel == null) bottomNav.missionPanel = missionPanel;
                if (bottomNav.defenseUI == null) bottomNav.defenseUI = defenseUI;
            }
        }
    }
}
