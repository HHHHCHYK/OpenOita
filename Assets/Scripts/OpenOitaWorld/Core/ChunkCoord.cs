using System;

public readonly struct ChunkCoord : IEquatable<ChunkCoord>
{
    public readonly int X;
    public readonly int Y;

    public ChunkCoord(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static ChunkCoord FromCell(int worldX, int worldY)
    {
        return new ChunkCoord(
            FloorDiv(worldX, WorldConstants.ChunkSize),
            FloorDiv(worldY, WorldConstants.ChunkSize));
    }

    public static void LocalFromCell(int worldX, int worldY, out int localX, out int localY)
    {
        localX = FloorMod(worldX, WorldConstants.ChunkSize);
        localY = FloorMod(worldY, WorldConstants.ChunkSize);
    }

    public static int CellIndex(int localX, int localY) =>
        localY * WorldConstants.ChunkSize + localX;

    public static bool IsInBounds(ChunkCoord coord) =>
        coord.X >= 0 && coord.Y >= 0 &&
        coord.X < WorldConstants.ChunksPerAxis &&
        coord.Y < WorldConstants.ChunksPerAxis;

    public bool Equals(ChunkCoord other) => X == other.X && Y == other.Y;

    public override bool Equals(object obj) => obj is ChunkCoord other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(X, Y);

    public override string ToString() => $"({X}, {Y})";

    static int FloorDiv(int value, int divisor)
    {
        int result = value / divisor;
        int remainder = value % divisor;
        if (remainder != 0 && ((remainder < 0) ^ (divisor < 0)))
            result--;

        return result;
    }

    static int FloorMod(int value, int divisor)
    {
        int remainder = value % divisor;
        if (remainder < 0)
            remainder += divisor;

        return remainder;
    }
}
