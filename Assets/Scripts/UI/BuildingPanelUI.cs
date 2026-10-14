using System.Collections.Generic;
using System.Text;
using AshenCitadel.Buildings;
using AshenCitadel.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AshenCitadel.UI
{
    public class BuildingPanelUI : MonoBehaviour
    {
        public GameObject root;
        public Text titleText, descText, levelText, costText;
        public TMP_Text titleTmp, descTmp, levelTmp, costTmp;
        public Button upgradeButton, closeButton;

        BuildingType _selected = BuildingType.Citadel;

        static readonly Dictionary<BuildingType, string> Names = new Dictionary<BuildingType, string>
        {
            { BuildingType.Citadel, "دژ مرکزی" },
            { BuildingType.Woodcamp, "اردوگاه چوب" },
            { BuildingType.Farm, "مزرعه" },
            { BuildingType.Coalpit, "کوره زغال" },
            { BuildingType.Barracks, "اردوگاه نیرو" }
        };

        static readonly Dictionary<BuildingType, string> Descs = new Dictionary<BuildingType, string>
        {
            { BuildingType.Citadel, "قلب دژ و منبع اصلی گرما" },
            { BuildingType.Woodcamp, "تولید چوب" },
            { BuildingType.Farm, "تولید غذا" },
            { BuildingType.Coalpit, "تولید زغال برای گرما" },
            { BuildingType.Barracks, "تربیت سرباز" }
        };

        static readonly Dictionary<ResourceType, string> ResFa = new Dictionary<ResourceType, string>
        {
            { ResourceType.Wood, "چوب" }, { ResourceType.Stone, "سنگ" }, { ResourceType.Food, "غذا" },
            { ResourceType.Coal, "زغال" }, { ResourceType.Iron, "آهن" }, { ResourceType.Crystal, "کریستال" }
        };

        void Start()
        {
            if (upgradeButton != null) { upgradeButton.onClick.RemoveAllListeners(); upgradeButton.onClick.AddListener(OnUpgrade); }
            if (closeButton != null) { closeButton.onClick.RemoveAllListeners(); closeButton.onClick.AddListener(Hide); }
            Hide();
        }

        public bool IsOpen => root != null && root.activeSelf;
        public void ShowCitadel() => Show(BuildingType.Citadel);

        public void Show(BuildingType type)
        {
            _selected = type;
            if (root != null) root.SetActive(true);
            Refresh();
        }

        public void Hide()
        {
            if (root != null) root.SetActive(false);
        }

        void Refresh()
        {
            var bm = BuildingManager.Instance;
            int lv = bm != null ? bm.GetLevel(_selected) : 0;
            Set(titleText, titleTmp, Names.ContainsKey(_selected) ? Names[_selected] : _selected.ToString());
            Set(descText, descTmp, Descs.ContainsKey(_selected) ? Descs[_selected] : "");
            Set(levelText, levelTmp, "سطح فعلی: " + lv);
            var cost = bm != null ? bm.GetUpgradeCost(_selected) : null;
            var sb = new StringBuilder("هزینه ارتقاء: ");
            if (cost != null)
                foreach (var kv in cost)
                    sb.Append(ResFa.ContainsKey(kv.Key) ? ResFa[kv.Key] : kv.Key.ToString()).Append(' ').Append(kv.Value).Append("  ");
            Set(costText, costTmp, sb.ToString());
        }

        void OnUpgrade()
        {
            if (BuildingManager.Instance == null) return;
            if (BuildingManager.Instance.TryUpgrade(_selected))
            {
                UINotifier.Instance?.Show((Names.ContainsKey(_selected) ? Names[_selected] : "") + " ارتقا یافت!");
                Refresh();
            }
            else UINotifier.Instance?.Show("منابع کافی نیست!");
        }

        static void Set(Text legacy, TMP_Text tmp, string s)
        {
            if (tmp != null) tmp.text = s;
            if (legacy != null) legacy.text = s;
        }
    }
}
