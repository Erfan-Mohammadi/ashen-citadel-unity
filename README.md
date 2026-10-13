# Ashen Citadel — Unity (دژ خاکستر)

اسکلت Unity برای نسخه موبایل حرفه‌ای. منطق از پروتوتایپ HTML5 (v1.0) پورت شده است.

## نیازمندی
- **Unity 2022.3 LTS** یا جدیدتر
- ماژول‌ها: Android / iOS Build Support
- (اختیاری) TextMesh Pro برای UI فارسی

## راه‌اندازی (۵ دقیقه)

1. Unity Hub → **New project** → 2D (یا 3D URP)
2. نام: `AshenCitadel`
3. پوشه `Assets/Scripts` این ریپو را داخل پروژه کپی کن
4. صحنه جدید: `Assets/Scenes/Main.unity`
5. Empty بساز → `Bootstrap` → کامپوننت `AshenCitadel.UI.Bootstrap`
6. Play
7. دکمه UI → `OnClickStartGame`

## معماری

```
Assets/Scripts/
  Core/       GameManager, ResourceManager, HeatSystem, SaveSystem
  Buildings/  BuildingManager
  Heroes/     HeroManager
  Defense/    DefenseCombat
  Missions/   MissionManager
  UI/         Bootstrap, UINotifier
```

## مراحل بعد
1. UI Canvas موبایل
2. اسپریت ساختمان‌ها
3. VFX دفاع
4. Tutorial فارسی
5. Build Android APK

پروتوتایپ وب: https://github.com/Erfan-Mohammadi/ashen-citadel
