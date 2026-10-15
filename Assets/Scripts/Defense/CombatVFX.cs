using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AshenCitadel.Defense
{
    public class CombatVFX : MonoBehaviour
    {
        public RectTransform root;
        readonly List<(RectTransform rt, float life, Vector2 vel)> _fx = new List<(RectTransform, float, Vector2)>();
        Font _font;
        DefenseCombat _combat;
        float _pulse;

        public void Attach(RectTransform parent)
        {
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                    ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            var go = new GameObject("CombatVFX", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            root = go.GetComponent<RectTransform>();
            root.anchorMin = Vector2.zero;
            root.anchorMax = Vector2.one;
            root.offsetMin = root.offsetMax = Vector2.zero;
        }

        void OnEnable()
        {
            _combat = DefenseCombat.Instance;
            if (_combat != null)
            {
                _combat.OnWaveStarted += () => { _pulse = 0; };
                _combat.OnCombatUpdated += OnTick;
            }
        }

        void OnDisable()
        {
            if (_combat != null) _combat.OnCombatUpdated -= OnTick;
        }

        void OnTick()
        {
            _pulse += Time.deltaTime;
            if (_pulse < 0.35f || _combat == null || root == null) return;
            _pulse = 0;
            foreach (var e in _combat.enemies)
            {
                if (e.hp <= 0) continue;
                if (Random.value > 0.3f) continue;
                float nx = (e.x / 320f - 0.5f) * 800f;
                float ny = (e.y / 400f - 0.5f) * 600f;
                SpawnFloat(nx, ny, "-" + Random.Range(8, 25), new Color(1f, 0.4f, 0.3f));
                break;
            }
        }

        public void SpawnFloat(float x, float y, string text, Color c)
        {
            var go = new GameObject("Dmg", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            go.transform.SetParent(root, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(x, y);
            rt.sizeDelta = new Vector2(80, 36);
            var t = go.GetComponent<Text>();
            t.font = _font; t.fontSize = 26; t.color = c;
            t.alignment = TextAnchor.MiddleCenter; t.text = text;
            t.raycastTarget = false;
            _fx.Add((rt, 0.9f, new Vector2(Random.Range(-20f, 20f), 60f)));
        }

        void Update()
        {
            float dt = Time.deltaTime;
            for (int i = _fx.Count - 1; i >= 0; i--)
            {
                var item = _fx[i];
                item.life -= dt;
                if (item.rt != null)
                {
                    item.rt.anchoredPosition += item.vel * dt;
                    var t = item.rt.GetComponent<Text>();
                    if (t != null)
                    {
                        var col = t.color;
                        col.a = Mathf.Clamp01(item.life / 0.9f);
                        t.color = col;
                    }
                }
                if (item.life <= 0f)
                {
                    if (item.rt != null) Destroy(item.rt.gameObject);
                    _fx.RemoveAt(i);
                }
                else _fx[i] = item;
            }
        }
    }
}
