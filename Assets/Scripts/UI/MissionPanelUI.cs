using AshenCitadel.Missions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AshenCitadel.UI
{
    public class MissionPanelUI : MonoBehaviour
    {
        public GameObject root;
        public Transform listParent;
        public GameObject missionCardPrefab;
        public Button closeButton;

        void Start()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(Hide);
            }
            Hide();
        }

        public bool IsOpen => root != null && root.activeSelf;

        public void Show()
        {
            MissionManager.Instance?.EnsureToday();
            if (root != null) root.SetActive(true);
            Rebuild();
        }

        public void Hide()
        {
            if (root != null) root.SetActive(false);
        }

        public void Rebuild()
        {
            if (listParent == null || MissionManager.Instance == null) return;
            for (int i = listParent.childCount - 1; i >= 0; i--)
                Destroy(listParent.GetChild(i).gameObject);

            foreach (var m in MissionManager.Instance.missions)
            {
                if (missionCardPrefab == null)
                {
                    Debug.Log($"Mission {m.titleFa} {m.progress}/{m.target}");
                    continue;
                }
                var go = Instantiate(missionCardPrefab, listParent);
                var card = go.GetComponent<MissionCardView>();
                if (card != null) card.Bind(m, this);
            }
        }

        public void OnClaim(string id)
        {
            if (MissionManager.Instance != null && MissionManager.Instance.Claim(id))
            {
                UINotifier.Instance?.Show("پاداش مأموریت دریافت شد!");
                Rebuild();
            }
        }
    }

    public class MissionCardView : MonoBehaviour
    {
        public Text titleText, progressText, rewardText;
        public TMP_Text titleTmp, progressTmp, rewardTmp;
        public Button claimButton;
        public Text claimLabel;
        public TMP_Text claimLabelTmp;

        public void Bind(Mission m, MissionPanelUI panel)
        {
            Set(titleText, titleTmp, m.titleFa);
            Set(progressText, progressTmp, $"{m.descFa} — {Mathf.FloorToInt(m.progress)} از {Mathf.FloorToInt(m.target)}");
            string reward = "پاداش: ";
            if (m.reward != null)
                foreach (var kv in m.reward)
                    reward += kv.Key + " " + kv.Value + "  ";
            Set(rewardText, rewardTmp, reward);

            bool done = m.progress >= m.target;
            if (claimButton != null)
            {
                claimButton.onClick.RemoveAllListeners();
                if (m.claimed)
                {
                    claimButton.interactable = false;
                    Set(claimLabel, claimLabelTmp, "دریافت شد");
                }
                else if (done)
                {
                    claimButton.interactable = true;
                    Set(claimLabel, claimLabelTmp, "دریافت پاداش");
                    claimButton.onClick.AddListener(() => panel.OnClaim(m.id));
                }
                else
                {
                    claimButton.interactable = false;
                    Set(claimLabel, claimLabelTmp, Mathf.FloorToInt(m.progress) + " از " + Mathf.FloorToInt(m.target));
                }
            }
        }

        static void Set(Text legacy, TMP_Text tmp, string s)
        {
            if (tmp != null) tmp.text = s;
            if (legacy != null) legacy.text = s;
        }
    }
}
