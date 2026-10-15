using System;
using UnityEngine;

namespace AshenCitadel.Core
{
    public class ShopSystem : MonoBehaviour
    {
        public static ShopSystem Instance { get; private set; }
        public event Action OnPurchase;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public struct Pack
        {
            public string id;
            public string nameFa;
            public int crystals;
            public string priceLabel;
        }

        public static readonly Pack[] Packs =
        {
            new Pack { id = "c_small", nameFa = "بسته کوچک", crystals = 50, priceLabel = "۰٫۹۹$" },
            new Pack { id = "c_mid", nameFa = "بسته متوسط", crystals = 300, priceLabel = "۴٫۹۹$" },
            new Pack { id = "c_big", nameFa = "بسته بزرگ", crystals = 1200, priceLabel = "۱۴٫۹۹$" },
        };

        public bool GrantDevPack(string packId)
        {
            foreach (var p in Packs)
            {
                if (p.id != packId) continue;
                ResourceManager.Instance?.Add(ResourceType.Crystal, p.crystals);
                UI.UINotifier.Instance?.Show(p.crystals + " کریستال اضافه شد!");
                OnPurchase?.Invoke();
                SaveSystem.Instance?.Save();
                return true;
            }
            return false;
        }
    }
}
