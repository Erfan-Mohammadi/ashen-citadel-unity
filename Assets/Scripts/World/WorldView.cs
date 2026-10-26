using System.Collections.Generic;
using AshenCitadel.Buildings;
using UnityEngine;
using UnityEngine.UI;

namespace AshenCitadel.World
{
    public class WorldView : MonoBehaviour
    {
        public RectTransform mapRoot;
        readonly Dictionary<BuildingType, RectTransform> _markers = new Dictionary<BuildingType, RectTransform>();
        readonly Dictionary<BuildingType, Image> _icons = new Dictionary<BuildingType, Image>();
        Font _font;

        static readonly Color Ground = new Color(0.07f, 0.11f, 0.17f, 1f);

        struct Slot
        {
            public BuildingType type;
            public Vector2 anchored;
            public string labelFa;
        }

        static readonly Slot[] Layout =
        {
            new Slot { type = BuildingType.Citadel, anchored = new Vector2(0, 50), labelFa = "دژ" },
            new Slot { type = BuildingType.Woodcamp, anchored = new Vector2(-280, -70), labelFa = "چوب" },
            new Slot { type = BuildingType.Farm, anchored = new Vector2(280, -70), labelFa = "مزرعه" },
            new Slot { type = BuildingType.Coalpit, anchored = new Vector2(-180, -230), labelFa = "زغال" },
            new Slot { type = BuildingType.Barracks, anchored = new Vector2(180, -230), labelFa = "نیرو" },
        };

        public void Build(RectTransform parent)
        {
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (_font == null) _font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            var mapGo = new GameObject("WorldMap", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            mapGo.transform.SetParent(parent, false);
            mapRoot = mapGo.GetComponent<RectTransform>();
            mapRoot.anchorMin = mapRoot.anchorMax = new Vector2(0.5f, 0.5f);
            mapRoot.sizeDelta = new Vector2(980, 920);
            mapRoot.anchoredPosition = new Vector2(0, 70);
            mapGo.GetComponent<Image>().color = Ground;

            var ring = new GameObject("Ring", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            ring.transform.SetParent(mapRoot, false);
            ring.GetComponent<RectTransform>().sizeDelta = new Vector2(560, 560);
            ring.GetComponent<Image>().color = new Color(0.12f, 0.18f, 0.28f, 0.35f);

            var path = new GameObject("Path", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            path.transform.SetParent(mapRoot, false);
            var prt = path.GetComponent<RectTransform>();
            prt.sizeDelta = new Vector2(80, 280);
            prt.anchoredPosition = new Vector2(0, -90);
            path.GetComponent<Image>().color = new Color(0.15f, 0.18f, 0.22f, 0.6f);

            foreach (var s in Layout) CreateMarker(s);

            if (BuildingManager.Instance != null)
                BuildingManager.Instance.OnBuildingUpgraded += OnUpgraded;
            RefreshLevels();
        }

        void CreateMarker(Slot s)
        {
            var go = new GameObject(s.type.ToString(), typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            go.transform.SetParent(mapRoot, false);
            var rt = go.GetComponent<RectTransform>();
            float size = s.type == BuildingType.Citadel ? 168f : 128f;
            rt.sizeDelta = new Vector2(size, size);
            rt.anchoredPosition = s.anchored;

            var img = go.GetComponent<Image>();
            int lv = BuildingManager.Instance != null ? BuildingManager.Instance.GetLevel(s.type) : 1;
            img.sprite = BuildingSpriteFactory.Create(s.type, Mathf.Max(1, lv));
            img.preserveAspect = true;
            img.color = Color.white;

            var type = s.type;
            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                var panel = Object.FindObjectOfType<UI.BuildingPanelUI>();
                panel?.Show(type);
                Core.AudioManager.Instance?.Play("click");
            });

            var badge = new GameObject("Badge", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            badge.transform.SetParent(go.transform, false);
            var brt = badge.GetComponent<RectTransform>();
            brt.anchorMin = brt.anchorMax = new Vector2(1f, 1f);
            brt.pivot = new Vector2(1f, 1f);
            brt.anchoredPosition = new Vector2(8, 8);
            brt.sizeDelta = new Vector2(40, 40);
            badge.GetComponent<Image>().color = new Color(0.15f, 0.2f, 0.3f, 0.9f);

            var labelGo = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            labelGo.transform.SetParent(go.transform, false);
            var lrt = labelGo.GetComponent<RectTransform>();
            lrt.anchorMin = new Vector2(0.5f, 0); lrt.anchorMax = new Vector2(0.5f, 0);
            lrt.anchoredPosition = new Vector2(0, -18);
            lrt.sizeDelta = new Vector2(160, 32);
            var t = labelGo.GetComponent<Text>();
            t.font = _font; t.fontSize = 22; t.color = Color.white;
            t.alignment = TextAnchor.MiddleCenter;
            t.text = s.labelFa + " 1";
            t.raycastTarget = false;

            var bn = new GameObject("Bn", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            bn.transform.SetParent(badge.transform, false);
            var bnrt = bn.GetComponent<RectTransform>();
            bnrt.anchorMin = Vector2.zero; bnrt.anchorMax = Vector2.one;
            bnrt.offsetMin = bnrt.offsetMax = Vector2.zero;
            var bt = bn.GetComponent<Text>();
            bt.font = _font; bt.fontSize = 20; bt.color = new Color(1f, 0.75f, 0.3f);
            bt.alignment = TextAnchor.MiddleCenter;
            bt.text = "1";
            bt.raycastTarget = false;

            _markers[s.type] = rt;
            _icons[s.type] = img;
        }

        void OnUpgraded(BuildingType type, int level) => RefreshLevels();

        public void RefreshLevels()
        {
            var bm = BuildingManager.Instance;
            if (bm == null) return;
            foreach (var s in Layout)
            {
                if (!_markers.TryGetValue(s.type, out var rt)) continue;
                int lv = bm.GetLevel(s.type);
                rt.gameObject.SetActive(lv > 0);
                var texts = rt.GetComponentsInChildren<Text>();
                if (texts.Length > 0) texts[0].text = s.labelFa + " " + lv;
                if (texts.Length > 1) texts[1].text = lv.ToString();
                float scale = 1f + (lv - 1) * 0.05f;
                rt.localScale = Vector3.one * Mathf.Min(scale, 1.35f);
                if (_icons.TryGetValue(s.type, out var img) && img != null)
                    img.sprite = BuildingSpriteFactory.Create(s.type, Mathf.Max(1, lv));
            }
        }

        void OnDestroy()
        {
            if (BuildingManager.Instance != null)
                BuildingManager.Instance.OnBuildingUpgraded -= OnUpgraded;
        }
    }
}
