using AshenCitadel.Defense;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AshenCitadel.UI
{
    public class DefenseUI : MonoBehaviour
    {
        public GameObject root;
        public Text waveText;
        public TMP_Text waveTmp;
        public Button startWaveButton, exitButton;
        public Text statusText;
        public TMP_Text statusTmp;

        void Start()
        {
            if (startWaveButton != null) { startWaveButton.onClick.RemoveAllListeners(); startWaveButton.onClick.AddListener(OnStartWave); }
            if (exitButton != null) { exitButton.onClick.RemoveAllListeners(); exitButton.onClick.AddListener(Close); }
            if (root != null) root.SetActive(false);
        }

        void OnEnable() => Bind(true);
        void OnDisable() => Bind(false);

        void Bind(bool on)
        {
            var d = DefenseCombat.Instance;
            if (d == null) return;
            if (on)
            {
                d.OnWaveStarted += OnWaveStarted;
                d.OnWaveWon += OnWon;
                d.OnWaveLost += OnLost;
                d.OnCombatUpdated += RefreshWaveLabel;
            }
            else
            {
                d.OnWaveStarted -= OnWaveStarted;
                d.OnWaveWon -= OnWon;
                d.OnWaveLost -= OnLost;
                d.OnCombatUpdated -= RefreshWaveLabel;
            }
        }

        public void Open()
        {
            if (root != null) root.SetActive(true);
            RefreshWaveLabel();
            if (startWaveButton != null) startWaveButton.gameObject.SetActive(true);
            SetStatus("دشمنان نزدیک می‌شوند...");
        }

        public void Close()
        {
            if (DefenseCombat.Instance != null) DefenseCombat.Instance.running = false;
            if (root != null) root.SetActive(false);
        }

        void OnStartWave() => DefenseCombat.Instance?.StartWave();

        void OnWaveStarted()
        {
            if (startWaveButton != null) startWaveButton.gameObject.SetActive(false);
            SetStatus("نبرد در جریان است!");
            UINotifier.Instance?.Show("موج " + (DefenseCombat.Instance?.wave ?? 1) + " شروع شد!");
        }

        void OnWon()
        {
            if (startWaveButton != null) startWaveButton.gameObject.SetActive(true);
            SetStatus("پیروزی!");
            UINotifier.Instance?.Show("پیروزی!");
            RefreshWaveLabel();
        }

        void OnLost()
        {
            if (startWaveButton != null) startWaveButton.gameObject.SetActive(true);
            SetStatus("دژ آسیب دید...");
            UINotifier.Instance?.Show("دژ آسیب دید...");
        }

        void RefreshWaveLabel()
        {
            int w = DefenseCombat.Instance != null ? DefenseCombat.Instance.wave : 1;
            string s = "موج " + w;
            if (waveTmp != null) waveTmp.text = s;
            if (waveText != null) waveText.text = s;
        }

        void SetStatus(string s)
        {
            if (statusTmp != null) statusTmp.text = s;
            if (statusText != null) statusText.text = s;
        }
    }
}
