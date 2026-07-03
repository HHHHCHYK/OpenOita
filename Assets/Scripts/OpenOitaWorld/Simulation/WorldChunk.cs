using System;
using Unity.Collections;

public sealed class WorldChunk
{
    public ChunkCoord Coord { get; }
    public ChunkAllocationState State { get; private set; }
    public NativeArray<CellState> Cells;
    public NativeArray<ushort> MaterialIds;
    public bool Dirty { get; set; }

    public bool IsEmpty => State == ChunkAllocationState.Empty;
    public bool HasCells => Cells.IsCreated;

    public WorldChunk(ChunkCoord coord, ChunkAllocationState state = ChunkAllocationState.Empty)
    {
        Coord = coord;
        State = state;
    }

    public void Allocate()
    {
        if (State == ChunkAllocationState.Allocated)
            return;

        Cells = new NativeArray<CellState>(WorldConstants.CellsPerChunk, Allocator.Persistent);
        MaterialIds = new NativeArray<ushort>(WorldConstants.CellsPerChunk, Allocator.Persistent);
        State = ChunkAllocationState.Allocated;
    }

    public void Dispose()
    {
        if (Cells.IsCreated)
            Cells.Dispose();

        if (MaterialIds.IsCreated)
            MaterialIds.Dispose();

        State = ChunkAllocationState.Empty;
    }
}
