using System.Collections.Generic;
using UnityEngine;

public sealed class WorldRender
{
    readonly Dictionary<ChunkCoord, ChunkGpuResources> _gpuResources = new();
    readonly Dictionary<ChunkCoord, ChunkDisplay> _displays = new();

    Transform _displayRoot;
    ComputeShader _computeShader;
    Texture2DArray _materialAtlas;
    Shader _displayShader;
    int _kernel;

    public void Initialize(MaterialRegistry registry, ComputeShader computeShader, Transform displayRoot, Shader displayShader)
    {
        _computeShader = computeShader;
        _displayRoot = displayRoot;
        _displayShader = displayShader;
        _materialAtlas = MaterialAtlasBuilder.Build(registry);
        _kernel = _computeShader.FindKernel("CSMain");
    }

    public void SyncChunk(WorldChunk chunk)
    {
        if (chunk == null || chunk.IsEmpty || !chunk.HasCells)
            return;

        ChunkGpuResources gpu = GetOrCreateGpu(chunk.Coord);
        gpu.UploadAndDispatch(chunk.MaterialIds, _materialAtlas, _computeShader, _kernel);
        chunk.Dirty = false;

        if (_displays.TryGetValue(chunk.Coord, out ChunkDisplay display))
            display.SetTexture(gpu.ColorTexture);
    }

    public void EnsureDisplay(WorldChunk chunk)
    {
        if (chunk == null || chunk.IsEmpty || !chunk.HasCells)
            return;

        if (_displays.ContainsKey(chunk.Coord))
            return;

        ChunkGpuResources gpu = GetOrCreateGpu(chunk.Coord);

        var displayObject = new GameObject($"Chunk_{chunk.Coord.X}_{chunk.Coord.Y}");
        displayObject.transform.SetParent(_displayRoot, false);
        displayObject.transform.position = new Vector3(
            chunk.Coord.X * WorldConstants.ChunkSize,
            chunk.Coord.Y * WorldConstants.ChunkSize,
            0f);
        displayObject.transform.localScale = new Vector3(
            WorldConstants.ChunkSize,
            WorldConstants.ChunkSize,
            1f);

        var display = displayObject.AddComponent<ChunkDisplay>();
        display.Initialize(_displayShader, gpu.ColorTexture);
        _displays.Add(chunk.Coord, display);
    }

    ChunkGpuResources GetOrCreateGpu(ChunkCoord coord)
    {
        if (!_gpuResources.TryGetValue(coord, out ChunkGpuResources gpu))
        {
            gpu = new ChunkGpuResources();
            _gpuResources.Add(coord, gpu);
        }

        return gpu;
    }

    public void Dispose()
    {
        foreach (ChunkGpuResources gpu in _gpuResources.Values)
            gpu.Dispose();

        _gpuResources.Clear();

        foreach (ChunkDisplay display in _displays.Values)
        {
            if (display != null)
                Object.Destroy(display.gameObject);
        }

        _displays.Clear();

        if (_materialAtlas != null)
        {
            Object.Destroy(_materialAtlas);
            _materialAtlas = null;
        }
    }
}
