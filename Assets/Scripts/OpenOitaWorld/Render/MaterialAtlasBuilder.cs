using UnityEngine;

public static class MaterialAtlasBuilder
{
    const int TileSize = 16;

    public static Texture2DArray Build(MaterialRegistry registry)
    {
        int sliceCount = Mathf.Max(registry.MaxMaterialId + 1, 1);
        var atlas = new Texture2DArray(TileSize, TileSize, sliceCount, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };

        var clearPixels = new Color[TileSize * TileSize];
        for (int i = 0; i < clearPixels.Length; i++)
            clearPixels[i] = new Color(0f, 0f, 0f, 0f);

        atlas.SetPixels(clearPixels, 0);

        foreach (MaterialDefinition material in registry.Materials)
        {
            if (material.id <= 0 || material.id >= sliceCount)
                continue;

            Color color = material.ParseColor();
            var pixels = new Color[TileSize * TileSize];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = color;

            atlas.SetPixels(pixels, material.id);
        }

        atlas.Apply(false, false);
        return atlas;
    }
}
