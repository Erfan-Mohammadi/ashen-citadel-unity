using AshenCitadel.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AshenCitadel.UI
{
    public class StartScreenUI : MonoBehaviour
    {
        public GameObject root;
        public Button startButton;
        public Text subtitleText;
        public TMP_Text subtitleTmp;
        public Text buttonLabel;
        public TMP_Text buttonLabelTmp;

        void Start()
        {
            bool hasSave = SaveSystem.Instance != null && SaveSystem.Instance.HasSave();
            string sub = hasSave ? "پیشرفت قبلی شما ذخیره شده است." : "جهان یخ زده است. تو آخرین امید مردمی...";
            string btn = hasSave ? "ادامه بازی" : "شروع بازی";
            if (subtitleTmp != null) subtitleTmp.text = sub;
            if (subtitleText != null) subtitleText.text = sub;
            if (buttonLabelTmp != null) buttonLabelTmp.text = btn;
            if (buttonLabel != null) buttonLabel.text = btn;
            if (startButton != null)
            {
                startButton.onClick.RemoveAllListeners();
                startButton.onClick.AddListener(OnStart);
            }
        }

        public void OnStart()
        {
            bool fromSave = SaveSystem.Instance != null && SaveSystem.Instance.HasSave();
            GameManager.Instance?.StartGame(fromSave);
            UINotifier.Instance?.Show(fromSave ? "خوش برگشتی، فرمانده!" : "به دژ خاکستر خوش آمدی، فرمانده!");
            if (root != null) root.SetActive(false);
            else gameObject.SetActive(false);

            if (GameManager.Instance != null && !GameManager.Instance.tutorialDone)
            {
                var canvas = GameObject.Find("AshenCanvas");
                if (canvas != null)
                {
                    var tut = canvas.AddComponent<TutorialUI>();
                    tut.Build(canvas.transform);
                }
            }
        }
    }
}
