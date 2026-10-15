# Build Android APK — دژ خاکستر

## پیش‌نیاز
- Unity 2022.3+ با Android Build Support + SDK/NDK
- JDK 11+

## Player Settings
- Package: `com.erfan.ashencitadel`
- Min API 24, Portrait, IL2CPP, ARM64

## Build
Build Settings → Android → Build → AshenCitadel.apk

```bash
adb install -r AshenCitadel.apk
```
