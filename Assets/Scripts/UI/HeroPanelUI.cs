using AshenCitadel.Heroes;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AshenCitadel.UI
{
    public class HeroPanelUI : MonoBehaviour
    {
        public GameObject root;
        public Transform listParent;
        public GameObject heroCardPrefab;
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
            if (root != null) root.SetActive(true);
            Rebuild();
        }

        public void Hide()
        {
            if (root != null) root.SetActive(false);
        }

        public void Rebuild()
        {
            if (listParent == null) return;
            for (int i = listParent.childCount - 1; i >= 0; i--)
                Destroy(listParent.GetChild(i).gameObject);

            var hm = HeroManager.Instance;
            if (hm == null) return;

            foreach (var h in hm.heroes)
            {
                if (heroCardPrefab == null)
                {
                    Debug.Log($"Hero {h.nameFa} lv{h.level} owned={h.owned}");
                    continue;
                }
                var go = Instantiate(heroCardPrefab, listParent);
                var card = go.GetComponent<HeroCardView>();
                if (card != null) card.Bind(h, this);
            }
        }

        public void OnLevelUp(string id)
        {
            if (HeroManager.Instance != null && HeroManager.Instance.LevelUp(id))
            {
                UINotifier.Instance?.Show("سطح قهرمان بالا رفت!");
                Rebuild();
            }
            else UINotifier.Instance?.Show("منابع کافی نیست!");
        }

        public void OnStarUp(string id)
        {
            if (HeroManager.Instance != null && HeroManager.Instance.StarUp(id))
            {
                UINotifier.Instance?.Show("ستاره قهرمان بالا رفت!");
                Rebuild();
            }
            else UINotifier.Instance?.Show("منابع کافی نیست!");
        }

        public void OnUnlock(string id)
        {
            if (HeroManager.Instance != null && HeroManager.Instance.Unlock(id))
            {
                UINotifier.Instance?.Show("قهرمان باز شد!");
                Rebuild();
            }
            else UINotifier.Instance?.Show("کریستال کافی نیست!");
        }
    }

    public class HeroCardView : MonoBehaviour
    {
        public Text nameText, roleText, statsText;
        public TMP_Text nameTmp, roleTmp, statsTmp;
        public Button levelButton, starButton, unlockButton;
        public GameObject ownedRoot, lockedRoot;

        string _id;
        HeroPanelUI _panel;

        public void Bind(HeroState h, HeroPanelUI panel)
        {
            _id = h.id;
            _panel = panel;
            string roleFa = h.role == HeroRole.Vanguard ? "محافظ" : h.role == HeroRole.Destroyer ? "نابودگر" : "پشتیبان";
            Set(nameText, nameTmp, h.nameFa);
            Set(roleText, roleTmp, roleFa + " • سطح " + h.level + " • " + new string('★', h.stars));
            Set(statsText, statsTmp, $"HP {h.hp:0} | ATK {h.atk:0} | DEF {h.def:0}");
            if (ownedRoot != null) ownedRoot.SetActive(h.owned);
            if (lockedRoot != null) lockedRoot.SetActive(!h.owned);
            Wire(levelButton, () => _panel.OnLevelUp(_id), h.owned);
            Wire(starButton, () => _panel.OnStarUp(_id), h.owned);
            Wire(unlockButton, () => _panel.OnUnlock(_id), !h.owned);
        }

        static void Wire(Button b, UnityEngine.Events.UnityAction a, bool show)
        {
            if (b == null) return;
            b.gameObject.SetActive(show);
            b.onClick.RemoveAllListeners();
            if (show) b.onClick.AddListener(a);
        }

        static void Set(Text legacy, TMP_Text tmp, string s)
        {
            if (tmp != null) tmp.text = s;
            if (legacy != null) legacy.text = s;
        }
    }
}
