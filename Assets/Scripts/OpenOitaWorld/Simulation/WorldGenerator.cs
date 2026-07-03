using UnityEngine;

public static class WorldGenerator
{
    public static void FillCoreChunk(ChunkStore store, MaterialRegistry registry, string fillMaterialName)
    {
        if (!registry.TryGetByName(fillMaterialName, out MaterialDefinition material))
        {
            Debug.LogError($"Core chunk fill material '{fillMaterialName}' was not found in MaterialRegistry.");
            return;
        }

        WorldChunk chunk = store.GetChunk(0, 0);

        if (chunk.IsEmpty)
            chunk.Allocate();

        ushort materialId = (ushort)material.id;
        var cell = new CellState
        {
            MaterialId = materialId,
            Velocity = Unity.Mathematics.float2.zero
        };

        for (int i = 0; i < WorldConstants.CellsPerChunk; i++)
        {
            chunk.Cells[i] = cell;
            chunk.MaterialIds[i] = materialId;
        }

        chunk.Dirty = true;
    }
}
