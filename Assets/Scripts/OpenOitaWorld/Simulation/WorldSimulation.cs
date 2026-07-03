public sealed class WorldSimulation
{
    public ChunkStore Chunks { get; } = new();

    public void Dispose()
    {
        Chunks.Dispose();
    }
}
