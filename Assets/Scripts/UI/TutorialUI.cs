using AshenCitadel.Core;
using UnityEngine;
using UnityEngine.UI;

namespace AshenCitadel.UI
{
    public class TutorialUI : MonoBehaviour
    {
        public GameObject root;
        Text _body;
        int _step;
        static readonly string[] Steps =
        {
            "خوش آمدی، فرمانده. جهان یخ زده است — دژ تنها پناهگاه است.",
            "منابع هر ثانیه تولید می‌شوند. گرما را بالا نگه دار تا تولید کم نشود.",
            "ساختمان‌ها را از نقشه یا دکمه «ساخت» ارتقا بده.",
            "با «دفاع» موج دشمن را دفع کن و پاداش بگیر.",
            "قهرمانان را قوی‌تر کن. مأموریت روزانه را فراموش نکن. موفق باشی!"
        };

        public void Build(Transform canvas)
        {
            if (GameManager.Instance != null && GameManager.Instance.tutorialDone) return;

            root = new GameObject("Tutorial", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            root.transform.SetParent(canvas, false);
            var rt = root.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            root.GetComponent<Image>().color = new Color(0, 0, 0, 0.72f);

            var card = new GameObject("Card", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            card.transform.SetParent(root.transform, false);
            var crt = card.GetComponent<RectTransform>();
            crt.anchorMin = crt.anchorMax = new Vector2(0.5f, 0.4f);
            crt.sizeDelta = new Vector2(900, 360);
            card.GetComponent<Image>().color = new Color(0.07f, 0.12f, 0.2f, 0.98f);

            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                       ?? Resources.GetBuiltinResource<Font>("Arial.ttf");

            var bodyGo = new GameObject("Body", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            bodyGo.transform.SetParent(card.transform, false);
            var brt = bodyGo.GetComponent<RectTransform>();
            brt.anchorMin = new Vector2(0.05f, 0.3f); brt.anchorMax = new Vector2(0.95f, 0.9f);
            brt.offsetMin = brt.offsetMax = Vector2.zero;
            _body = bodyGo.GetComponent<Text>();
            _body.font = font; _body.fontSize = 30; _body.color = Color.white;
            _body.alignment = TextAnchor.MiddleCenter;

            var btnGo = new GameObject("Next", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btnGo.transform.SetParent(card.transform, false);
            var btnRt = btnGo.GetComponent<RectTransform>();
            btnRt.anchorMin = btnRt.anchorMax = new Vector2(0.5f, 0.12f);
            btnRt.sizeDelta = new Vector2(280, 64);
            btnGo.GetComponent<Image>().color = new Color(0.24f, 0.6f, 0.88f);
            btnGo.GetComponent<Button>().onClick.AddListener(Next);

            var lbl = new GameObject("L", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            lbl.transform.SetParent(btnGo.transform, false);
            var lrt = lbl.GetComponent<RectTransform>();
            lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one;
            lrt.offsetMin = lrt.offsetMax = Vector2.zero;
            var lt = lbl.GetComponent<Text>();
            lt.font = font; lt.fontSize = 28; lt.color = Color.white;
            lt.alignment = TextAnchor.MiddleCenter; lt.text = "بعدی";

            _step = 0;
            ShowStep();
        }

        void ShowStep()
        {
            if (_body != null) _body.text = Steps[_step];
        }

        void Next()
        {
            _step++;
            if (_step >= Steps.Length)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.tutorialDone = true;
                    SaveSystem.Instance?.Save();
                }
                if (root != null) root.SetActive(false);
                return;
            }
            ShowStep();
        }
    }
}
