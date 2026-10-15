using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AshenCitadel.Defense
{
    public class DefenseView : MonoBehaviour
    {
        public RectTransform arenaRoot;
        readonly List<GameObject> _spawned = new List<GameObject>();
        DefenseCombat _combat;

        public void Attach(RectTransform parent)
        {
            var go = new GameObject("DefenseArena", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            arenaRoot = go.GetComponent<RectTransform>();
            arenaRoot.anchorMin = arenaRoot.anchorMax = new Vector2(0.5f, 0.5f);
            arenaRoot.sizeDelta = new Vector2(900, 700);
            arenaRoot.anchoredPosition = new Vector2(0, -40);
            go.GetComponent<Image>().color = new Color(0.04f, 0.06f, 0.1f, 0.85f);
            go.SetActive(false);
        }

        void OnEnable()
        {
            _combat = DefenseCombat.Instance;
            if (_combat == null) return;
            _combat.OnWaveStarted += Show;
            _combat.OnWaveWon += Hide;
            _combat.OnWaveLost += Hide;
            _combat.OnCombatUpdated += Redraw;
        }

        void OnDisable()
        {
            if (_combat == null) return;
            _combat.OnWaveStarted -= Show;
            _combat.OnWaveWon -= Hide;
            _combat.OnWaveLost -= Hide;
            _combat.OnCombatUpdated -= Redraw;
        }

        void Show()
        {
            if (arenaRoot != null) arenaRoot.gameObject.SetActive(true);
            Redraw();
        }

        void Hide()
        {
            Clear();
            if (arenaRoot != null) arenaRoot.gameObject.SetActive(false);
        }

        void Clear()
        {
            foreach (var g in _spawned)
                if (g != null) Destroy(g);
            _spawned.Clear();
        }

        void Redraw()
        {
            if (_combat == null || arenaRoot == null) return;
            Clear();
            foreach (var h in _combat.heroes)
            {
                if (h.hp <= 0) continue;
                float nx = (h.x / 320f - 0.5f) * 800f;
                float ny = (h.y / 400f - 0.5f) * 600f;
                SpawnUnit(nx, ny, new Color(0.3f, 0.7f, 1f), h.hp / Mathf.Max(1f, h.maxHp));
            }
            foreach (var e in _combat.enemies)
            {
                if (e.hp <= 0) continue;
                float nx = (e.x / 320f - 0.5f) * 800f;
                float ny = (e.y / 400f - 0.5f) * 600f;
                SpawnUnit(nx, ny, new Color(1f, 0.35f, 0.3f), e.hp / Mathf.Max(1f, e.maxHp));
            }
        }

        void SpawnUnit(float x, float y, Color c, float hpPct)
        {
            var go = new GameObject("Unit", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(arenaRoot, false);
            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(48, 48);
            rt.anchoredPosition = new Vector2(x, y);
            go.GetComponent<Image>().color = c;

            var barBg = new GameObject("HpBg", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            barBg.transform.SetParent(go.transform, false);
            var brt = barBg.GetComponent<RectTransform>();
            brt.sizeDelta = new Vector2(56, 8);
            brt.anchoredPosition = new Vector2(0, 32);
            barBg.GetComponent<Image>().color = new Color(0, 0, 0, 0.6f);

            var bar = new GameObject("Hp", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            bar.transform.SetParent(barBg.transform, false);
            var frt = bar.GetComponent<RectTransform>();
            frt.anchorMin = new Vector2(0, 0);
            frt.anchorMax = new Vector2(Mathf.Clamp01(hpPct), 1);
            frt.offsetMin = Vector2.zero; frt.offsetMax = Vector2.zero;
            bar.GetComponent<Image>().color = hpPct > 0.4f ? Color.green : Color.red;

            _spawned.Add(go);
        }
    }
}
