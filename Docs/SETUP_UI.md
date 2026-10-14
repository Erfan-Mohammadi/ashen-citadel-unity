# ساخت UI موبایل در Unity — دژ خاکستر

## سلسله‌مراتب پیشنهادی Canvas

```
Canvas (Screen Space Overlay, 1080×1920, Match 0.5)
├── Bootstrap
├── MainHUD
├── TopBar + ResourceBarUI
├── HeatBar + HeatBarUI
├── Toast + ToastUI
├── BottomBar + BottomNavUI
├── BuildingPanel + BuildingPanelUI
├── HeroPanel + HeroPanelUI + HeroCard prefab
├── MissionPanel + MissionPanelUI + MissionCard prefab
├── DefenseOverlay + DefenseUI
└── StartScreen + StartScreenUI
```

## Canvas Scaler
- Reference: **1080 × 1920**
- Match: **0.5**

## تم رنگ
- BG `#0A1220` | Card `#121E32` | Text `#E8EEF6` | Accent `#3D9AE0` | Heat `#FF9A5C`

## تست
Play → Start → منابع تیک می‌خورند → ساخت/دفاع/مأموریت/قهرمان
