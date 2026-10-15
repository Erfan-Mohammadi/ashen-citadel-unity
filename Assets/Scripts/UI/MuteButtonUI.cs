using AshenCitadel.Core;
using UnityEngine;
using UnityEngine.UI;

namespace AshenCitadel.UI
{
    public class MuteButtonUI : MonoBehaviour
    {
        public static void Create(Transform canvas)
        {
            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                       ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            var go = new GameObject("MuteBtn", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            go.transform.SetParent(canvas, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(1f, 1f);
            rt.anchoredPosition = new Vector2(-24, -210);
            rt.sizeDelta = new Vector2(72, 72);
            go.GetComponent<Image>().color = new Color(0.1f, 0.14f, 0.22f, 0.9f);

            var label = new GameObject("L", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            label.transform.SetParent(go.transform, false);
            var lrt = label.GetComponent<RectTransform>();
            lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one;
            lrt.offsetMin = lrt.offsetMax = Vector2.zero;
            var t = label.GetComponent<Text>();
            t.font = font; t.fontSize = 28; t.color = Color.white;
            t.alignment = TextAnchor.MiddleCenter;
            t.text = "🔊";
            t.raycastTarget = false;

            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                var am = AudioManager.Instance;
                if (am == null) return;
                am.SetMuted(!am.muted);
                t.text = am.muted ? "🔇" : "🔊";
                if (!am.muted) am.Play("click");
            });
        }
    }
}
