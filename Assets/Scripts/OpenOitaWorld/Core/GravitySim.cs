using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

// Not wired in MVP v0. Kept for the next simulation iteration.
public struct GravitySim : IJobFor
{
    public NativeArray<CellState> Cells;
    public float WorldGravity;
    public float DeltaTime;

    public void Execute(int index)
    {
        CellState cell = Cells[index];
        cell.Velocity += new float2(0f, WorldGravity * DeltaTime);
        Cells[index] = cell;
    }
}
