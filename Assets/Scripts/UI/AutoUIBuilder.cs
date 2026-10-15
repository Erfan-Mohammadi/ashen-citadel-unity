using AshenCitadel.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace AshenCitadel.UI
{
    /// <summary>
    /// Builds the entire mobile HUD at runtime — no manual Canvas wiring.
    /// </summary>
    public class AutoUIBuilder : MonoBehaviour
    {
        static readonly Color Bg = Hex("0A1220");
        static readonly Color Card = Hex("121E32");
        static readonly Color Accent = Hex("3D9AE0");
        static readonly Color Heat = Hex("FF9A5C");
        static readonly Color TextCol = Hex("E8EEF6");
        static readonly Color Muted = Hex("8899AA");
        static readonly Color Danger = Hex("FF6B4A");

        Font _font;
        Canvas _canvas;
        StartScreenUI _start;
        BuildingPanelUI _buildPanel;
        HeroPanelUI _heroPanel;
        MissionPanelUI _missionPanel;
        DefenseUI _defense;
        ToastUI _toast;
        BottomNavUI _nav;
        Text[] _resLabels = new Text[6];
        static readonly string[] Icons = { "🪵", "🪨", "🍖", "🔥", "⚙️", "💎" };

        void Start()
        {
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (_font == null) _font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            EnsureEventSystem();
            BuildCanvas();
            BuildTopBar();
            BuildHeatBar();
            BuildToast();
            BuildBottomBar();
            BuildBuildingPanel();
            BuildHeroPanel();
            BuildMissionPanel();
            BuildDefense();
            BuildStartScreen();
            WireNav();
        }

        void EnsureEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null) return;
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        void BuildCanvas()
        {
            var go = new GameObject("AshenCanvas");
            _canvas = go.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 100;
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;
            go.AddComponent<GraphicRaycaster>();
        }

        void BuildTopBar()
        {
            var bar = Panel("TopBar", _canvas.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0, -20), new Vector2(1040, 140));
            bar.GetComponent<Image>().color = new Color(0, 0, 0, 0.55f);
            var grid = bar.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(330, 56);
            grid.spacing = new Vector2(12, 10);
            grid.padding = new RectOffset(16, 16, 12, 12);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 3;
            for (int i = 0; i < 6; i++)
            {
                var chip = Panel("Res" + i, bar.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
                chip.GetComponent<Image>().color = i == 5 ? new Color(0.2f, 0.15f, 0.35f, 0.9f) : Card;
                _resLabels[i] = Label(chip.transform, Icons[i] + " 0", 28, TextCol, TextAnchor.MiddleCenter);
            }
            var bridge = bar.AddComponent<ResourceBarBridge>();
            bridge.labels = _resLabels;
        }

        void BuildHeatBar()
        {
            var bar = Panel("HeatBar", _canvas.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0, -175), new Vector2(1000, 40));
            bar.GetComponent<Image>().color = new Color(0, 0, 0, 0.25f);
            var label = Label(bar.transform, "گرما", 22, Heat, TextAnchor.MiddleRight);
            var labelRt = label.rectTransform;
            labelRt.anchorMin = new Vector2(1, 0); labelRt.anchorMax = new Vector2(1, 1);
            labelRt.pivot = new Vector2(1, 0.5f); labelRt.sizeDelta = new Vector2(80, 0);
            labelRt.anchoredPosition = new Vector2(-8, 0);

            var track = Panel("Track", bar.transform, new Vector2(0, 0.5f), new Vector2(1, 0.5f),
                new Vector2(-50, 0), new Vector2(-160, 18));
            track.GetComponent<Image>().color = Hex("101820");
            var trackRt = track.GetComponent<RectTransform>();
            trackRt.offsetMin = new Vector2(20, -9); trackRt.offsetMax = new Vector2(-100, 9);

            var fill = Panel("Fill", track.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            var fillImg = fill.GetComponent<Image>();
            fillImg.color = Heat; fillImg.type = Image.Type.Filled;
            fillImg.fillMethod = Image.FillMethod.Horizontal; fillImg.fillAmount = 1f;
            StretchFull(fill.GetComponent<RectTransform>());

            var pct = Label(bar.transform, "100%", 22, Hex("FFD09A"), TextAnchor.MiddleLeft);
            var pctRt = pct.rectTransform;
            pctRt.anchorMin = new Vector2(0, 0); pctRt.anchorMax = new Vector2(0, 1);
            pctRt.pivot = new Vector2(0, 0.5f); pctRt.sizeDelta = new Vector2(70, 0);
            pctRt.anchoredPosition = new Vector2(12, 0);

            var heatUI = bar.AddComponent<HeatBarUI>();
            heatUI.fillImage = fillImg; heatUI.percentText = pct;
        }

        void BuildToast()
        {
            var toast = Panel("Toast", _canvas.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0, -240), new Vector2(800, 56));
            toast.GetComponent<Image>().color = new Color(0.05f, 0.12f, 0.2f, 0.95f);
            var cg = toast.AddComponent<CanvasGroup>(); cg.alpha = 0f;
            var msg = Label(toast.transform, "", 26, TextCol, TextAnchor.MiddleCenter);
            StretchFull(msg.rectTransform);
            _toast = toast.AddComponent<ToastUI>();
            _toast.group = cg; _toast.messageText = msg;
        }

        void BuildBottomBar()
        {
            var bar = Panel("BottomBar", _canvas.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0, 20), new Vector2(1040, 120));
            bar.GetComponent<Image>().color = new Color(0.02f, 0.04f, 0.07f, 0.94f);
            var row = bar.AddComponent<HorizontalLayoutGroup>();
            row.spacing = 10; row.padding = new RectOffset(12, 12, 12, 12);
            row.childAlignment = TextAnchor.MiddleCenter;
            row.childForceExpandWidth = true; row.childForceExpandHeight = true;
            string[] labels = { "ساخت", "قهرمان", "دفاع", "مأموریت", "اتحاد" };
            string[] icons = { "🏗️", "⚔️", "🛡️", "🗺️", "🤝" };
            Button[] buttons = new Button[5];
            for (int i = 0; i < 5; i++)
            {
                var bgo = Panel("Btn" + i, bar.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
                bgo.GetComponent<Image>().color = Card;
                var btn = bgo.AddComponent<Button>();
                var t = Label(bgo.transform, icons[i] + "\n" + labels[i], 24, TextCol, TextAnchor.MiddleCenter);
                StretchFull(t.rectTransform);
                buttons[i] = btn;
            }
            _nav = bar.AddComponent<BottomNavUI>();
            _nav.buildButton = buttons[0]; _nav.heroesButton = buttons[1];
            _nav.defendButton = buttons[2]; _nav.missionsButton = buttons[3];
            _nav.allianceButton = buttons[4];
        }

        void BuildBuildingPanel()
        {
            var root = Panel("BuildingPanel", _canvas.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0, 160), new Vector2(980, 420));
            root.GetComponent<Image>().color = new Color(0.04f, 0.07f, 0.12f, 0.97f);
            root.SetActive(false);
            var title = Label(root.transform, "دژ مرکزی", 34, Accent, TextAnchor.UpperCenter);
            Place(title.rectTransform, 0.5f, 1f, 0, -30, 900, 40);
            var desc = Label(root.transform, "", 26, Muted, TextAnchor.UpperCenter);
            Place(desc.rectTransform, 0.5f, 1f, 0, -80, 900, 36);
            var level = Label(root.transform, "", 26, TextCol, TextAnchor.UpperCenter);
            Place(level.rectTransform, 0.5f, 1f, 0, -130, 900, 36);
            var cost = Label(root.transform, "", 24, Heat, TextAnchor.UpperCenter);
            Place(cost.rectTransform, 0.5f, 1f, 0, -180, 900, 40);
            var upBtn = MakeButton(root.transform, "ارتقاء", Accent, new Vector2(0.3f, 0f), new Vector2(0, 50), new Vector2(280, 70));
            var closeBtn = MakeButton(root.transform, "بستن", Card, new Vector2(0.7f, 0f), new Vector2(0, 50), new Vector2(280, 70));
            _buildPanel = root.AddComponent<BuildingPanelUI>();
            _buildPanel.root = root; _buildPanel.titleText = title; _buildPanel.descText = desc;
            _buildPanel.levelText = level; _buildPanel.costText = cost;
            _buildPanel.upgradeButton = upBtn; _buildPanel.closeButton = closeBtn;
        }

        void BuildHeroPanel()
        {
            var root = Panel("HeroPanel", _canvas.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0, 160), new Vector2(980, 900));
            root.GetComponent<Image>().color = new Color(0.04f, 0.07f, 0.12f, 0.97f);
            root.SetActive(false);
            var title = Label(root.transform, "قهرمانان دژ", 34, Accent, TextAnchor.UpperCenter);
            Place(title.rectTransform, 0.5f, 1f, 0, -28, 900, 40);
            var content = MakeScrollContent(root, out _);
            var closeBtn = MakeButton(root.transform, "بستن", Card, new Vector2(0.5f, 0f), new Vector2(0, 30), new Vector2(280, 64));
            _heroPanel = root.AddComponent<HeroPanelUI>();
            _heroPanel.root = root; _heroPanel.listParent = content;
            _heroPanel.heroCardPrefab = CreateHeroCardPrefab();
            _heroPanel.closeButton = closeBtn;
        }

        void BuildMissionPanel()
        {
            var root = Panel("MissionPanel", _canvas.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0, 160), new Vector2(980, 900));
            root.GetComponent<Image>().color = new Color(0.04f, 0.07f, 0.12f, 0.97f);
            root.SetActive(false);
            var title = Label(root.transform, "مأموریت‌های روزانه", 34, Accent, TextAnchor.UpperCenter);
            Place(title.rectTransform, 0.5f, 1f, 0, -28, 900, 40);
            var content = MakeScrollContent(root, out _);
            var closeBtn = MakeButton(root.transform, "بستن", Card, new Vector2(0.5f, 0f), new Vector2(0, 30), new Vector2(280, 64));
            _missionPanel = root.AddComponent<MissionPanelUI>();
            _missionPanel.root = root; _missionPanel.listParent = content;
            _missionPanel.missionCardPrefab = CreateMissionCardPrefab();
            _missionPanel.closeButton = closeBtn;
        }

        Transform MakeScrollContent(GameObject root, out ScrollRect scroll)
        {
            var scrollGo = new GameObject("Scroll", typeof(RectTransform));
            scrollGo.transform.SetParent(root.transform, false);
            var srt = scrollGo.GetComponent<RectTransform>();
            srt.anchorMin = new Vector2(0, 0); srt.anchorMax = new Vector2(1, 1);
            srt.offsetMin = new Vector2(20, 90); srt.offsetMax = new Vector2(-20, -80);
            scroll = scrollGo.AddComponent<ScrollRect>(); scroll.horizontal = false;
            var viewport = Panel("Viewport", scrollGo.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            viewport.GetComponent<Image>().color = new Color(0, 0, 0, 0.01f);
            viewport.AddComponent<Mask>().showMaskGraphic = false;
            StretchFull(viewport.GetComponent<RectTransform>());
            var content = new GameObject("Content", typeof(RectTransform));
            content.transform.SetParent(viewport.transform, false);
            var crt = content.GetComponent<RectTransform>();
            crt.anchorMin = new Vector2(0, 1); crt.anchorMax = new Vector2(1, 1);
            crt.pivot = new Vector2(0.5f, 1);
            var vlg = content.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 12; vlg.padding = new RectOffset(8, 8, 8, 8);
            vlg.childForceExpandWidth = true; vlg.childForceExpandHeight = false;
            content.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            scroll.viewport = viewport.GetComponent<RectTransform>(); scroll.content = crt;
            return content.transform;
        }

        GameObject CreateHeroCardPrefab()
        {
            var card = Panel("HeroCardPrefab", _canvas.transform, Vector2.zero, Vector2.one, Vector2.zero, new Vector2(900, 180));
            card.GetComponent<Image>().color = Card; card.SetActive(false);
            var le = card.AddComponent<LayoutElement>(); le.minHeight = 180; le.preferredHeight = 180;
            var name = Label(card.transform, "نام", 30, Accent, TextAnchor.UpperRight);
            Place(name.rectTransform, 1f, 1f, -20, -16, 500, 36);
            var role = Label(card.transform, "نقش", 24, Muted, TextAnchor.UpperRight);
            Place(role.rectTransform, 1f, 1f, -20, -56, 500, 30);
            var stats = Label(card.transform, "stats", 22, TextCol, TextAnchor.UpperRight);
            Place(stats.rectTransform, 1f, 1f, -20, -92, 500, 28);
            var owned = new GameObject("Owned"); owned.transform.SetParent(card.transform, false);
            var locked = new GameObject("Locked"); locked.transform.SetParent(card.transform, false);
            var lvlBtn = MakeButton(owned.transform, "ارتقای سطح", Accent, new Vector2(0f, 0f), new Vector2(160, 28), new Vector2(300, 52));
            var starBtn = MakeButton(owned.transform, "ستاره", Card, new Vector2(0f, 0f), new Vector2(480, 28), new Vector2(300, 52));
            var unlockBtn = MakeButton(locked.transform, "باز کردن (۶ کریستال)", Accent, new Vector2(0.5f, 0f), new Vector2(0, 28), new Vector2(400, 56));
            var view = card.AddComponent<HeroCardView>();
            view.nameText = name; view.roleText = role; view.statsText = stats;
            view.levelButton = lvlBtn; view.starButton = starBtn; view.unlockButton = unlockBtn;
            view.ownedRoot = owned; view.lockedRoot = locked;
            card.transform.SetParent(null); DontDestroyOnLoad(card);
            return card;
        }

        GameObject CreateMissionCardPrefab()
        {
            var card = Panel("MissionCardPrefab", _canvas.transform, Vector2.zero, Vector2.one, Vector2.zero, new Vector2(900, 160));
            card.GetComponent<Image>().color = Card; card.SetActive(false);
            var le = card.AddComponent<LayoutElement>(); le.minHeight = 160; le.preferredHeight = 160;
            var title = Label(card.transform, "عنوان", 28, Accent, TextAnchor.UpperRight);
            Place(title.rectTransform, 1f, 1f, -16, -12, 860, 32);
            var prog = Label(card.transform, "پیشرفت", 24, Muted, TextAnchor.UpperRight);
            Place(prog.rectTransform, 1f, 1f, -16, -50, 860, 28);
            var reward = Label(card.transform, "پاداش", 22, Heat, TextAnchor.UpperRight);
            Place(reward.rectTransform, 1f, 1f, -16, -84, 860, 28);
            var claimBtn = MakeButton(card.transform, "دریافت", Accent, new Vector2(0.5f, 0f), new Vector2(0, 16), new Vector2(320, 48));
            var view = card.AddComponent<MissionCardView>();
            view.titleText = title; view.progressText = prog; view.rewardText = reward;
            view.claimButton = claimBtn; view.claimLabel = claimBtn.GetComponentInChildren<Text>();
            card.transform.SetParent(null); DontDestroyOnLoad(card);
            return card;
        }

        void BuildDefense()
        {
            var root = Panel("DefenseOverlay", _canvas.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            root.GetComponent<Image>().color = new Color(0.01f, 0.02f, 0.05f, 0.94f);
            StretchFull(root.GetComponent<RectTransform>()); root.SetActive(false);
            var title = Label(root.transform, "حمله به دژ!", 48, Danger, TextAnchor.UpperCenter);
            Place(title.rectTransform, 0.5f, 1f, 0, -80, 900, 60);
            var wave = Label(root.transform, "موج 1", 36, Accent, TextAnchor.UpperCenter);
            Place(wave.rectTransform, 0.5f, 1f, 0, -160, 900, 44);
            var status = Label(root.transform, "دشمنان نزدیک می‌شوند...", 28, Muted, TextAnchor.UpperCenter);
            Place(status.rectTransform, 0.5f, 1f, 0, -220, 900, 40);
            var startBtn = MakeButton(root.transform, "شروع دفاع", Accent, new Vector2(0.5f, 0.35f), Vector2.zero, new Vector2(360, 80));
            var exitBtn = MakeButton(root.transform, "بازگشت", Card, new Vector2(0.5f, 0.22f), Vector2.zero, new Vector2(360, 70));
            _defense = root.AddComponent<DefenseUI>();
            _defense.root = root; _defense.waveText = wave; _defense.statusText = status;
            _defense.startWaveButton = startBtn; _defense.exitButton = exitBtn;
        }

        void BuildStartScreen()
        {
            var root = Panel("StartScreen", _canvas.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            root.GetComponent<Image>().color = Hex("060C16");
            StretchFull(root.GetComponent<RectTransform>());
            var h1 = Label(root.transform, "ASHEN CITADEL", 52, Accent, TextAnchor.MiddleCenter);
            Place(h1.rectTransform, 0.5f, 0.62f, 0, 0, 1000, 60);
            var h2 = Label(root.transform, "دژ خاکستر", 44, Heat, TextAnchor.MiddleCenter);
            Place(h2.rectTransform, 0.5f, 0.55f, 0, 0, 1000, 50);
            var sub = Label(root.transform, "جهان یخ زده است. تو آخرین امید مردمی...", 28, Muted, TextAnchor.MiddleCenter);
            Place(sub.rectTransform, 0.5f, 0.46f, 0, 0, 900, 80);
            var startBtn = MakeButton(root.transform, "شروع بازی", Accent, new Vector2(0.5f, 0.32f), Vector2.zero, new Vector2(420, 90));
            _start = root.AddComponent<StartScreenUI>();
            _start.root = root; _start.startButton = startBtn;
            _start.subtitleText = sub; _start.buttonLabel = startBtn.GetComponentInChildren<Text>();
        }

        void WireNav()
        {
            if (_nav == null) return;
            _nav.buildingPanel = _buildPanel; _nav.heroPanel = _heroPanel;
            _nav.missionPanel = _missionPanel; _nav.defenseUI = _defense;
        }

        static Color Hex(string h) { ColorUtility.TryParseHtmlString("#" + h, out var c); return c; }

        GameObject Panel(string name, Transform parent, Vector2 aMin, Vector2 aMax, Vector2 pos, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = aMin; rt.anchorMax = aMax; rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos; rt.sizeDelta = size;
            go.GetComponent<Image>().color = Card; return go;
        }

        Text Label(Transform parent, string text, int size, Color color, TextAnchor align)
        {
            var go = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            go.transform.SetParent(parent, false);
            var t = go.GetComponent<Text>();
            t.font = _font; t.text = text; t.fontSize = size; t.color = color;
            t.alignment = align; t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow = VerticalWrapMode.Overflow; t.raycastTarget = false; return t;
        }

        Button MakeButton(Transform parent, string text, Color bg, Vector2 anchor, Vector2 pos, Vector2 size)
        {
            var go = Panel("Btn", parent, anchor, anchor, pos, size);
            go.GetComponent<Image>().color = bg;
            var btn = go.AddComponent<Button>();
            var t = Label(go.transform, text, 28, Color.white, TextAnchor.MiddleCenter);
            StretchFull(t.rectTransform); return btn;
        }

        static void StretchFull(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        }

        static void Place(RectTransform rt, float ax, float ay, float x, float y, float w, float h)
        {
            rt.anchorMin = rt.anchorMax = new Vector2(ax, ay);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(x, y); rt.sizeDelta = new Vector2(w, h);
        }
    }

    public class ResourceBarBridge : MonoBehaviour
    {
        public Text[] labels;
        static readonly ResourceType[] Order =
        {
            ResourceType.Wood, ResourceType.Stone, ResourceType.Food,
            ResourceType.Coal, ResourceType.Iron, ResourceType.Crystal
        };
        static readonly string[] Icons = { "🪵", "🪨", "🍖", "🔥", "⚙️", "💎" };

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

        void Refresh()
        {
            var r = ResourceManager.Instance;
            if (r == null || labels == null) return;
            for (int i = 0; i < labels.Length && i < 6; i++)
            {
                if (labels[i] == null) continue;
                labels[i].text = Icons[i] + " " + Mathf.FloorToInt(r.Get(Order[i]));
            }
        }
    }
}
