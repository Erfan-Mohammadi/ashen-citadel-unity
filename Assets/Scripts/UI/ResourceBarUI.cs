using AshenCitadel.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AshenCitadel.UI
{
    public class ResourceBarUI : MonoBehaviour
    {
        public Text woodText, stoneText, foodText, coalText, ironText, crystalText;
        public TMP_Text woodTmp, stoneTmp, foodTmp, coalTmp, ironTmp, crystalTmp;

        void OnEnable()
        {
            if (ResourceManager.Instance != null)
                ResourceManager.Instance.OnResourcesChanged += Refresh;
            Refresh();
        }

        void OnDisable()
        {
            if (ResourceManager.Instance != null)
                ResourceManager.Instance.OnResourcesChanged -= Refresh;
        }

        public void Refresh()
        {
            var r = ResourceManager.Instance;
            if (r == null) return;
            Set(woodText, woodTmp, r.amounts.wood);
            Set(stoneText, stoneTmp, r.amounts.stone);
            Set(foodText, foodTmp, r.amounts.food);
            Set(coalText, coalTmp, r.amounts.coal);
            Set(ironText, ironTmp, r.amounts.iron);
            Set(crystalText, crystalTmp, r.amounts.crystal);
        }

        static void Set(Text legacy, TMP_Text tmp, float v)
        {
            string s = Mathf.FloorToInt(v).ToString();
            if (tmp != null) tmp.text = s;
            if (legacy != null) legacy.text = s;
        }
    }
}
