public static class WorldConstants
{
    public const int LogicalWorldSize = 100000;
    public const int ChunkSize = 128;
    public const int CellsPerChunk = ChunkSize * ChunkSize;
    public const int ChunksPerAxis = (LogicalWorldSize + ChunkSize - 1) / ChunkSize;
}
