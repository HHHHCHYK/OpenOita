using System.Collections.Generic;

public sealed class ChunkStore
{
    readonly Dictionary<ChunkCoord, WorldChunk> _chunks = new();

    public WorldChunk GetChunk(int chunkX, int chunkY)
    {
        var coord = new ChunkCoord(chunkX, chunkY);

        if (!ChunkCoord.IsInBounds(coord))
            throw new System.ArgumentOutOfRangeException(nameof(coord), coord, "Chunk coordinate is out of world bounds.");

        if (_chunks.TryGetValue(coord, out WorldChunk chunk))
            return chunk;

        chunk = new WorldChunk(coord);
        _chunks.Add(coord, chunk);
        return chunk;
    }

    public WorldChunk GetChunk(ChunkCoord coord) => GetChunk(coord.X, coord.Y);

    public bool TryGetCell(int worldX, int worldY, out CellState cell)
    {
        cell = default;

        if (worldX < 0 || worldY < 0 ||
            worldX >= WorldConstants.LogicalWorldSize ||
            worldY >= WorldConstants.LogicalWorldSize)
        {
            return false;
        }

        ChunkCoord chunkCoord = ChunkCoord.FromCell(worldX, worldY);
        WorldChunk chunk = GetChunk(chunkCoord);

        if (chunk.IsEmpty)
            return true;

        ChunkCoord.LocalFromCell(worldX, worldY, out int localX, out int localY);
        int index = ChunkCoord.CellIndex(localX, localY);
        cell = chunk.Cells[index];
        return true;
    }

    public void Dispose()
    {
        foreach (WorldChunk chunk in _chunks.Values)
            chunk.Dispose();

        _chunks.Clear();
    }
}
