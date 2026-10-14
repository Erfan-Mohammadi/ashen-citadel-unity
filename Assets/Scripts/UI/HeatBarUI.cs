using AshenCitadel.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AshenCitadel.UI
{
    public class HeatBarUI : MonoBehaviour
    {
        public Image fillImage;
        public Text percentText;
        public TMP_Text percentTmp;

        static readonly Color Hot = new Color(1f, 0.7f, 0.12f);
        static readonly Color Warm = new Color(1f, 0.5f, 0.1f);
        static readonly Color Cold = new Color(1f, 0.25f, 0.15f);

        void OnEnable()
        {
            if (HeatSystem.Instance != null)
                HeatSystem.Instance.OnHeatChanged += Refresh;
            Refresh();
        }

        void OnDisable()
        {
            if (HeatSystem.Instance != null)
                HeatSystem.Instance.OnHeatChanged -= Refresh;
        }

        public void Refresh()
        {
            var h = HeatSystem.Instance;
            if (h == null) return;
            float p = h.Percent;
            if (fillImage != null)
            {
                fillImage.fillAmount = p;
                fillImage.color = p > 0.6f ? Hot : p > 0.3f ? Warm : Cold;
            }
            string s = Mathf.FloorToInt(p * 100f) + "%";
            if (percentTmp != null) percentTmp.text = s;
            if (percentText != null) percentText.text = s;
        }
    }
}
