using AshenCitadel.Buildings;
using UnityEngine;

namespace AshenCitadel.World
{
    public static class BuildingSpriteFactory
    {
        const int Size = 128;

        public static Sprite Create(BuildingType type, int level = 1)
        {
            var tex = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;
            var pixels = new Color32[Size * Size];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = new Color32(0, 0, 0, 0);
            switch (type)
            {
                case BuildingType.Citadel: DrawCitadel(pixels, level); break;
                case BuildingType.Woodcamp: DrawWoodcamp(pixels, level); break;
                case BuildingType.Farm: DrawFarm(pixels, level); break;
                case BuildingType.Coalpit: DrawCoalpit(pixels, level); break;
                case BuildingType.Barracks: DrawBarracks(pixels, level); break;
            }
            tex.SetPixels32(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, Size, Size), new Vector2(0.5f, 0.5f), 100f);
        }

        static void Set(Color32[] p, int x, int y, Color32 c)
        {
            if (x < 0 || y < 0 || x >= Size || y >= Size) return;
            int i = y * Size + x;
            if (c.a == 255) p[i] = c;
            else
            {
                float a = c.a / 255f;
                var d = p[i];
                p[i] = new Color32(
                    (byte)(c.r * a + d.r * (1 - a)),
                    (byte)(c.g * a + d.g * (1 - a)),
                    (byte)(c.b * a + d.b * (1 - a)),
                    (byte)Mathf.Min(255, d.a + c.a));
            }
        }

        static void FillRect(Color32[] p, int x0, int y0, int w, int h, Color32 c)
        {
            for (int y = y0; y < y0 + h; y++)
                for (int x = x0; x < x0 + w; x++)
                    Set(p, x, y, c);
        }

        static void FillCircle(Color32[] p, int cx, int cy, int r, Color32 c)
        {
            int r2 = r * r;
            for (int y = -r; y <= r; y++)
                for (int x = -r; x <= r; x++)
                    if (x * x + y * y <= r2)
                        Set(p, cx + x, cy + y, c);
        }

        static void DrawCitadel(Color32[] p, int level)
        {
            var stone = new Color32(90, 100, 120, 255);
            var dark = new Color32(50, 55, 70, 255);
            var roof = new Color32(180, 70, 40, 255);
            var glow = new Color32(255, 140, 40, 180);
            var window = new Color32(255, 200, 80, 255);
            var snow = new Color32(220, 230, 245, 230);
            FillCircle(p, 64, 28, 40 + level * 2, new Color32(255, 120, 30, 40));
            FillRect(p, 28, 20, 72, 55, stone);
            FillRect(p, 30, 22, 68, 8, dark);
            FillRect(p, 18, 30, 22, 50, stone);
            FillRect(p, 88, 30, 22, 50, stone);
            FillRect(p, 16, 78, 26, 12, roof);
            FillRect(p, 86, 78, 26, 12, roof);
            FillRect(p, 24, 72, 80, 14, roof);
            for (int i = 0; i < 5; i++) FillRect(p, 30 + i * 14, 70, 8, 8, dark);
            int wins = 2 + Mathf.Min(level, 3);
            for (int i = 0; i < wins; i++)
            {
                FillRect(p, 38 + i * 16, 40, 8, 12, window);
                FillRect(p, 22, 45 + i * 12, 6, 8, window);
                FillRect(p, 100, 45 + i * 12, 6, 8, window);
            }
            FillRect(p, 16, 88, 26, 6, snow);
            FillRect(p, 86, 88, 26, 6, snow);
            FillRect(p, 30, 84, 68, 5, snow);
            FillRect(p, 54, 20, 20, 22, dark);
            FillRect(p, 58, 28, 4, 4, glow);
        }

        static void DrawWoodcamp(Color32[] p, int level)
        {
            var wood = new Color32(120, 80, 40, 255);
            var dark = new Color32(70, 45, 20, 255);
            var roof = new Color32(90, 55, 25, 255);
            var green = new Color32(50, 120, 50, 255);
            var snow = new Color32(230, 235, 245, 220);
            for (int i = 0; i < 4 + level; i++)
                FillRect(p, 20 + (i % 3) * 28, 18 + (i / 3) * 14, 26, 12, i % 2 == 0 ? wood : dark);
            FillRect(p, 40, 40, 50, 40, wood);
            FillRect(p, 38, 78, 54, 14, roof);
            FillRect(p, 40, 90, 50, 6, snow);
            FillRect(p, 58, 40, 14, 22, dark);
            FillRect(p, 48, 58, 10, 10, new Color32(255, 210, 100, 255));
            FillCircle(p, 30, 55, 10, green);
            FillRect(p, 26, 40, 8, 16, dark);
        }

        static void DrawFarm(Color32[] p, int level)
        {
            var soil = new Color32(100, 75, 40, 255);
            var crop = new Color32(180, 160, 50, 255);
            var barn = new Color32(160, 90, 50, 255);
            var roof = new Color32(140, 50, 40, 255);
            var snow = new Color32(230, 235, 245, 220);
            FillRect(p, 12, 16, 104, 36, soil);
            for (int i = 0; i < 6 + level; i++)
                FillRect(p, 16 + i * 16, 20, 10, 28, crop);
            FillRect(p, 36, 50, 56, 40, barn);
            FillRect(p, 32, 88, 64, 14, roof);
            FillRect(p, 36, 100, 56, 5, snow);
            FillRect(p, 56, 50, 16, 24, new Color32(60, 40, 25, 255));
        }

        static void DrawCoalpit(Color32[] p, int level)
        {
            var stone = new Color32(55, 55, 60, 255);
            var dark = new Color32(30, 30, 35, 255);
            var ember = new Color32(255, 100, 30, 255);
            var smoke = new Color32(80, 80, 90, 120);
            FillCircle(p, 64, 40, 36, stone);
            FillCircle(p, 64, 40, 24, dark);
            for (int i = 0; i < 5 + level; i++)
                FillCircle(p, 50 + (i * 7) % 30, 35 + (i * 5) % 16, 3 + i % 3, ember);
            FillRect(p, 88, 50, 14, 40, stone);
            FillRect(p, 86, 88, 18, 8, dark);
            FillCircle(p, 95, 100, 8, smoke);
            FillCircle(p, 100, 110, 6, smoke);
            FillCircle(p, 64, 42, 12, new Color32(255, 80, 20, 100));
        }

        static void DrawBarracks(Color32[] p, int level)
        {
            var wall = new Color32(100, 105, 120, 255);
            var dark = new Color32(60, 65, 80, 255);
            var banner = new Color32(180, 40, 40, 255);
            var snow = new Color32(225, 230, 240, 220);
            FillRect(p, 20, 24, 88, 50, wall);
            FillRect(p, 18, 72, 92, 12, dark);
            FillRect(p, 22, 82, 84, 6, snow);
            for (int i = 0; i < 4; i++)
                FillRect(p, 28 + i * 22, 24, 6, 50, dark);
            FillRect(p, 60, 70, 6, 30, dark);
            FillRect(p, 66, 88, 18, 12, banner);
            FillRect(p, 54, 24, 20, 22, dark);
            for (int i = 0; i < level; i++)
                FillCircle(p, 30 + i * 18, 18, 5, new Color32(140, 100, 70, 255));
        }
    }
}
