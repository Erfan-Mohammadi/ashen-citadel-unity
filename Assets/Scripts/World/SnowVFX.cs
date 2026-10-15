using UnityEngine;
using UnityEngine.UI;

namespace AshenCitadel.World
{
    public class SnowVFX : MonoBehaviour
    {
        struct Flake
        {
            public RectTransform rt;
            public float speed, drift, phase;
        }

        Flake[] _flakes;

        public void Build(RectTransform parent, int count = 60)
        {
            var go = new GameObject("SnowVFX", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var root = go.GetComponent<RectTransform>();
            root.anchorMin = Vector2.zero;
            root.anchorMax = Vector2.one;
            root.offsetMin = root.offsetMax = Vector2.zero;

            _flakes = new Flake[count];
            for (int i = 0; i < count; i++)
            {
                var f = new GameObject("F" + i, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                f.transform.SetParent(root, false);
                var rt = f.GetComponent<RectTransform>();
                float s = Random.Range(3f, 10f);
                rt.sizeDelta = new Vector2(s, s);
                rt.anchoredPosition = new Vector2(Random.Range(-500f, 500f), Random.Range(-450f, 450f));
                f.GetComponent<Image>().color = new Color(1, 1, 1, Random.Range(0.12f, 0.4f));
                f.GetComponent<Image>().raycastTarget = false;
                _flakes[i] = new Flake
                {
                    rt = rt,
                    speed = Random.Range(20f, 70f),
                    drift = Random.Range(-15f, 15f),
                    phase = Random.Range(0f, Mathf.PI * 2f)
                };
            }
        }

        void Update()
        {
            if (_flakes == null) return;
            float dt = Time.deltaTime;
            for (int i = 0; i < _flakes.Length; i++)
            {
                var f = _flakes[i];
                if (f.rt == null) continue;
                var p = f.rt.anchoredPosition;
                p.y -= f.speed * dt;
                p.x += Mathf.Sin(Time.time + f.phase) * f.drift * dt;
                if (p.y < -480f)
                {
                    p.y = 480f;
                    p.x = Random.Range(-500f, 500f);
                }
                f.rt.anchoredPosition = p;
            }
        }
    }
}
