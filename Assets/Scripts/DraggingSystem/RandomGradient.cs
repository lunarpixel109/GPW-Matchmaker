using UnityEngine;
using UnityEngine.UI;

public static class RandomGradient
{
    public enum Direction { Vertical, Horizontal, Diagonal }

    // Two nice-looking random colours (second hue is offset so they pair well)
    public static (Color a, Color b) RandomPair(float hueOffset = 0.15f)
    {
        float h = Random.value;
        Color a = Color.HSVToRGB(h, Random.Range(0.6f, 1f), Random.Range(0.8f, 1f));
        Color b = Color.HSVToRGB((h + hueOffset) % 1f, Random.Range(0.6f, 1f), Random.Range(0.8f, 1f));
        return (a, b);
    }

    public static Texture2D MakeTexture(Color a, Color b, Direction dir = Direction.Vertical, int size = 128)
    {
        int w = dir == Direction.Vertical ? 1 : size;
        int h = dir == Direction.Horizontal ? 1 : size;

        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float t = dir switch
                {
                    Direction.Vertical => y / (float)(h - 1),
                    Direction.Horizontal => x / (float)(w - 1),
                    _ => (x + y) / (float)(w + h - 2)
                };
                tex.SetPixel(x, y, Color.Lerp(a, b, t));
            }
        }

        tex.Apply();
        return tex;
    }

    public static Sprite MakeSprite(Color a, Color b, Direction dir = Direction.Vertical, int size = 128)
    {
        var tex = MakeTexture(a, b, dir, size);
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    // --- One-liner helpers ---

    public static void Apply(RawImage target, Direction dir = Direction.Vertical)
    {
        if (target.texture != null) Object.Destroy(target.texture); // avoid leaking old textures
        var (a, b) = RandomPair();
        target.texture = MakeTexture(a, b, dir);
    }

    public static void Apply(Image target, Direction dir = Direction.Vertical)
    {
        if (target.sprite != null)
        {
            Object.Destroy(target.sprite.texture);
            Object.Destroy(target.sprite);
        }
        var (a, b) = RandomPair();
        target.sprite = MakeSprite(a, b, dir);
    }
}
