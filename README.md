# Ashen Citadel — Unity (دژ خاکستر)

نسخه موبایل حرفه‌ای — **یک‌کلیک Play** بدون ساخت دستی UI.

## سریع‌ترین شروع (۳۰ ثانیه)

1. Unity Hub → پروژه **2D** جدید (2022.3+ LTS)
2. کل پوشه `Assets/Scripts` این ریپو را داخل `Assets` پروژه کپی کن
3. صحنه خالی → Create Empty → نام `Bootstrap`
4. Add Component → `AshenCitadel.UI.Bootstrap`
5. **Play**

خودکار ساخته می‌شود:
- همه مدیران بازی (منابع، گرما، ساختمان، قهرمان، دفاع، مأموریت، ذخیره)
- کل HUD موبایل (منابع، گرما، دکمه‌ها، پنل‌ها، شروع، دفاع)

## ریپوها
- Unity: https://github.com/Erfan-Mohammadi/ashen-citadel-unity
- پروتوتایپ وب: https://github.com/Erfan-Mohammadi/ashen-citadel

```bash
git clone https://github.com/Erfan-Mohammadi/ashen-citadel.git
cd ashen-citadel && npx serve .
```

## معماری
```
Scripts/Core Buildings Heroes Defense Missions UI(AutoUIBuilder)
```
