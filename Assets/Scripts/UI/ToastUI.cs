using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AshenCitadel.UI
{
    public class ToastUI : MonoBehaviour
    {
        public CanvasGroup group;
        public Text messageText;
        public TMP_Text messageTmp;
        public float showSeconds = 2.4f;

        void OnEnable()
        {
            if (UINotifier.Instance != null)
                UINotifier.Instance.OnNotify += Show;
        }

        void OnDisable()
        {
            if (UINotifier.Instance != null)
                UINotifier.Instance.OnNotify -= Show;
        }

        public void Show(string msg)
        {
            if (messageTmp != null) messageTmp.text = msg;
            if (messageText != null) messageText.text = msg;
            StopAllCoroutines();
            StartCoroutine(FadeRoutine());
        }

        IEnumerator FadeRoutine()
        {
            if (group == null) yield break;
            group.alpha = 1f;
            group.blocksRaycasts = false;
            yield return new WaitForSeconds(showSeconds * 0.75f);
            float t = 0f;
            while (t < 0.5f)
            {
                t += Time.deltaTime;
                group.alpha = 1f - t / 0.5f;
                yield return null;
            }
            group.alpha = 0f;
        }
    }
}
