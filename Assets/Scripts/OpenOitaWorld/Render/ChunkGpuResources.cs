using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public sealed class ChunkGpuResources
{
    public GraphicsBuffer MaterialIdBuffer { get; private set; }
    public RenderTexture ColorTexture { get; private set; }

    public void EnsureCreated()
    {
        if (MaterialIdBuffer == null)
        {
            MaterialIdBuffer = new GraphicsBuffer(
                GraphicsBuffer.Target.Structured,
                WorldConstants.CellsPerChunk,
                sizeof(uint));
        }

        if (ColorTexture == null)
        {
            ColorTexture = new RenderTexture(WorldConstants.ChunkSize, WorldConstants.ChunkSize, 0, RenderTextureFormat.ARGB32)
            {
                enableRandomWrite = true,
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            ColorTexture.Create();
        }
    }

    public void UploadAndDispatch(
        NativeArray<ushort> materialIds,
        Texture2DArray materialAtlas,
        ComputeShader computeShader,
        int kernel)
    {
        EnsureCreated();

        var upload = new uint[WorldConstants.CellsPerChunk];
        for (int i = 0; i < materialIds.Length; i++)
            upload[i] = materialIds[i];

        MaterialIdBuffer.SetData(upload);

        computeShader.SetBuffer(kernel, "_MaterialIds", MaterialIdBuffer);
        computeShader.SetTexture(kernel, "_MaterialArray", materialAtlas);
        computeShader.SetTexture(kernel, "_Output", ColorTexture);
        computeShader.SetInt("_ChunkSize", WorldConstants.ChunkSize);
        computeShader.SetInt("_MaterialSliceCount", materialAtlas.depth);

        int groups = WorldConstants.ChunkSize / 8;
        computeShader.Dispatch(kernel, groups, groups, 1);
    }

    public void Dispose()
    {
        MaterialIdBuffer?.Dispose();
        MaterialIdBuffer = null;

        if (ColorTexture != null)
        {
            ColorTexture.Release();
            Object.Destroy(ColorTexture);
            ColorTexture = null;
        }
    }
}
