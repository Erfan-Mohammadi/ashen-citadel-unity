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
        Font _font;

        static readonly Color Ground = new Color(0.08f, 0.12f, 0.18f, 1f);

        struct Slot
        {
            public BuildingType type;
            public Vector2 anchored;
            public string labelFa;
            public Color color;
        }

        static readonly Slot[] Layout =
        {
            new Slot { type = BuildingType.Citadel, anchored = new Vector2(0, 40), labelFa = "دژ", color = new Color(0.9f, 0.45f, 0.15f) },
            new Slot { type = BuildingType.Woodcamp, anchored = new Vector2(-280, -80), labelFa = "چوب", color = new Color(0.4f, 0.7f, 0.35f) },
            new Slot { type = BuildingType.Farm, anchored = new Vector2(280, -80), labelFa = "مزرعه", color = new Color(0.85f, 0.75f, 0.3f) },
            new Slot { type = BuildingType.Coalpit, anchored = new Vector2(-180, -220), labelFa = "زغال", color = new Color(0.35f, 0.35f, 0.4f) },
            new Slot { type = BuildingType.Barracks, anchored = new Vector2(180, -220), labelFa = "نیرو", color = new Color(0.55f, 0.35f, 0.7f) },
        };

        public void Build(RectTransform parent)
        {
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (_font == null) _font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            var mapGo = new GameObject("WorldMap", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            mapGo.transform.SetParent(parent, false);
            mapRoot = mapGo.GetComponent<RectTransform>();
            mapRoot.anchorMin = mapRoot.anchorMax = new Vector2(0.5f, 0.5f);
            mapRoot.sizeDelta = new Vector2(980, 900);
            mapRoot.anchoredPosition = new Vector2(0, 80);
            mapGo.GetComponent<Image>().color = Ground;

            for (int i = 0; i < 40; i++)
            {
                var fleck = new GameObject("Snow" + i, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                fleck.transform.SetParent(mapRoot, false);
                var rt = fleck.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(Random.Range(4, 12), Random.Range(4, 12));
                rt.anchoredPosition = new Vector2(Random.Range(-460, 460), Random.Range(-420, 420));
                fleck.GetComponent<Image>().color = new Color(1, 1, 1, Random.Range(0.05f, 0.18f));
            }

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
            rt.sizeDelta = s.type == BuildingType.Citadel ? new Vector2(140, 140) : new Vector2(110, 110);
            rt.anchoredPosition = s.anchored;
            go.GetComponent<Image>().color = s.color;

            var type = s.type;
            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                var panel = FindObjectOfType<UI.BuildingPanelUI>();
                panel?.Show(type);
            });

            var labelGo = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            labelGo.transform.SetParent(go.transform, false);
            var lrt = labelGo.GetComponent<RectTransform>();
            lrt.anchorMin = new Vector2(0.5f, 0); lrt.anchorMax = new Vector2(0.5f, 0);
            lrt.anchoredPosition = new Vector2(0, -28);
            lrt.sizeDelta = new Vector2(160, 36);
            var t = labelGo.GetComponent<Text>();
            t.font = _font; t.fontSize = 22; t.color = Color.white;
            t.alignment = TextAnchor.MiddleCenter;
            t.text = s.labelFa + " 1"; t.raycastTarget = false;

            _markers[s.type] = rt;

            if (s.type == BuildingType.Citadel)
            {
                var glow = new GameObject("Glow", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                glow.transform.SetParent(go.transform, false);
                glow.transform.SetAsFirstSibling();
                glow.GetComponent<RectTransform>().sizeDelta = new Vector2(180, 180);
                glow.GetComponent<Image>().color = new Color(1f, 0.5f, 0.15f, 0.25f);
            }
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
                var label = rt.GetComponentInChildren<Text>();
                if (label != null) label.text = s.labelFa + " " + lv;
                float scale = 1f + (lv - 1) * 0.06f;
                rt.localScale = Vector3.one * Mathf.Min(scale, 1.4f);
            }
        }

        void OnDestroy()
        {
            if (BuildingManager.Instance != null)
                BuildingManager.Instance.OnBuildingUpgraded -= OnUpgraded;
        }
    }
}
