using AshenCitadel.Core;
using UnityEngine;
using UnityEngine.UI;

namespace AshenCitadel.UI
{
    public class ShopPanelUI : MonoBehaviour
    {
        public GameObject root;

        public void Build(Transform canvas)
        {
            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                       ?? Resources.GetBuiltinResource<Font>("Arial.ttf");

            root = new GameObject("ShopPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            root.transform.SetParent(canvas, false);
            var rt = root.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0f);
            rt.anchorMax = new Vector2(0.5f, 0f);
            rt.anchoredPosition = new Vector2(0, 160);
            rt.sizeDelta = new Vector2(980, 720);
            root.GetComponent<Image>().color = new Color(0.04f, 0.07f, 0.12f, 0.97f);
            root.SetActive(false);

            var titleGo = MakeText(root.transform, font, "فروشگاه کریستال", 34, new Color(0.24f, 0.6f, 0.88f));
            Place(titleGo, 0.5f, 1f, 0, -28, 900, 40);

            var sub = MakeText(root.transform, font, "حالت تست — خرید رایگان برای بالانس", 22, new Color(0.55f, 0.6f, 0.68f));
            Place(sub, 0.5f, 1f, 0, -72, 900, 32);

            float y = -140;
            foreach (var pack in ShopSystem.Packs)
            {
                var row = new GameObject(pack.id, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                row.transform.SetParent(root.transform, false);
                var rrt = row.GetComponent<RectTransform>();
                rrt.anchorMin = rrt.anchorMax = new Vector2(0.5f, 1f);
                rrt.anchoredPosition = new Vector2(0, y);
                rrt.sizeDelta = new Vector2(900, 110);
                row.GetComponent<Image>().color = new Color(0.08f, 0.12f, 0.2f, 1f);

                var name = MakeText(row.transform, font, pack.nameFa + "  ·  " + pack.crystals + " 💎", 28, Color.white);
                Place(name, 0.05f, 0.5f, 200, 0, 400, 40);
                name.GetComponent<Text>().alignment = TextAnchor.MiddleRight;

                var price = MakeText(row.transform, font, pack.priceLabel, 24, new Color(1f, 0.7f, 0.35f));
                Place(price, 0.35f, 0.5f, 0, 0, 120, 36);

                var buy = MakeButton(row.transform, font, "خرید تست", new Color(0.24f, 0.6f, 0.88f), new Vector2(0.82f, 0.5f), new Vector2(200, 56));
                string id = pack.id;
                buy.onClick.AddListener(() => ShopSystem.Instance?.GrantDevPack(id));
                y -= 130;
            }

            var close = MakeButton(root.transform, font, "بستن", new Color(0.12f, 0.18f, 0.28f), new Vector2(0.5f, 0f), new Vector2(280, 64));
            var cRt = close.GetComponent<RectTransform>();
            cRt.anchorMin = cRt.anchorMax = new Vector2(0.5f, 0f);
            cRt.anchoredPosition = new Vector2(0, 30);
            close.onClick.AddListener(Hide);
        }

        public void Show() { if (root != null) root.SetActive(true); }
        public void Hide() { if (root != null) root.SetActive(false); }
        public bool IsOpen => root != null && root.activeSelf;

        static GameObject MakeText(Transform parent, Font font, string text, int size, Color c)
        {
            var go = new GameObject("T", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            go.transform.SetParent(parent, false);
            var t = go.GetComponent<Text>();
            t.font = font; t.text = text; t.fontSize = size; t.color = c;
            t.alignment = TextAnchor.MiddleCenter; t.raycastTarget = false;
            return go;
        }

        static Button MakeButton(Transform parent, Font font, string label, Color bg, Vector2 anchor, Vector2 size)
        {
            var go = new GameObject("Btn", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = anchor;
            rt.sizeDelta = size;
            go.GetComponent<Image>().color = bg;
            var btn = go.GetComponent<Button>();
            var tGo = MakeText(go.transform, font, label, 26, Color.white);
            var trt = tGo.GetComponent<RectTransform>();
            trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
            trt.offsetMin = trt.offsetMax = Vector2.zero;
            return btn;
        }

        static void Place(GameObject go, float ax, float ay, float x, float y, float w, float h)
        {
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(ax, ay);
            rt.anchoredPosition = new Vector2(x, y);
            rt.sizeDelta = new Vector2(w, h);
        }
    }
}
