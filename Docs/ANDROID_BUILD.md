# Build Android APK — دژ خاکستر (فاز A)

## پیش‌نیاز
Unity 2022.3 LTS + Android Build Support (SDK/NDK/OpenJDK)

## پروژه
1. پروژه 2D
2. کپی Assets/Scripts
3. صحنه Main + Bootstrap component
4. Scenes In Build: فقط Main

## Player Settings
- Package: com.erfan.ashencitadel
- Version 0.1.0 / code 1
- Min API 24, IL2CPP, ARM64, Portrait

## Build
Build Settings → Android → Build → AshenCitadel_v0.1.apk

```bash
adb install -r AshenCitadel_v0.1.apk
```

## تست پذیرش
باز شدن، شروع، ارتقا، یک موج، سیو، Mute، ۳۰ دقیقه بدون کرش
