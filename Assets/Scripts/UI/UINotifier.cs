using System;
using UnityEngine;

namespace AshenCitadel.UI
{
    public class UINotifier : MonoBehaviour
    {
        public static UINotifier Instance { get; private set; }
        public event Action<string> OnNotify;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void Show(string messageFa)
        {
            Debug.Log($"[Ashen] {messageFa}");
            OnNotify?.Invoke(messageFa);
        }
    }
}
