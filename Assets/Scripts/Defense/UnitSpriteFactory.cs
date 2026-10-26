using UnityEngine;

namespace AshenCitadel.Defense
{
    public static class UnitSpriteFactory
    {
        const int Size = 48;

        public static Sprite Hero(bool destroyer)
        {
            return Make(destroyer
                ? new Color32(60, 140, 220, 255)
                : new Color32(80, 180, 120, 255), true);
        }

        public static Sprite Enemy()
        {
            return Make(new Color32(200, 60, 50, 255), false);
        }

        static Sprite Make(Color32 body, bool isHero)
        {
            var tex = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            var p = new Color32[Size * Size];
            for (int i = 0; i < p.Length; i++) p[i] = new Color32(0, 0, 0, 0);

            FillCircle(p, 24, 10, 12, new Color32(0, 0, 0, 60));
            FillCircle(p, 24, 20, 14, body);
            FillCircle(p, 24, 34, 9, new Color32(
                (byte)Mathf.Min(255, body.r + 30),
                (byte)Mathf.Min(255, body.g + 30),
                (byte)Mathf.Min(255, body.b + 30), 255));
            FillCircle(p, 21, 36, 2, new Color32(255, 255, 255, 255));
            FillCircle(p, 27, 36, 2, new Color32(255, 255, 255, 255));
            if (!isHero)
            {
                FillRect(p, 14, 40, 4, 8, new Color32(80, 20, 20, 255));
                FillRect(p, 30, 40, 4, 8, new Color32(80, 20, 20, 255));
            }
            else
            {
                FillRect(p, 22, 40, 4, 7, new Color32(255, 200, 60, 255));
            }

            tex.SetPixels32(p);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, Size, Size), new Vector2(0.5f, 0.5f), 48f);
        }

        static void FillCircle(Color32[] p, int cx, int cy, int r, Color32 c)
        {
            int r2 = r * r;
            for (int y = -r; y <= r; y++)
                for (int x = -r; x <= r; x++)
                    if (x * x + y * y <= r2)
                    {
                        int px = cx + x, py = cy + y;
                        if (px >= 0 && py >= 0 && px < Size && py < Size)
                            p[py * Size + px] = c;
                    }
        }

        static void FillRect(Color32[] p, int x0, int y0, int w, int h, Color32 c)
        {
            for (int y = y0; y < y0 + h; y++)
                for (int x = x0; x < x0 + w; x++)
                    if (x >= 0 && y >= 0 && x < Size && y < Size)
                        p[y * Size + x] = c;
        }
    }
}
