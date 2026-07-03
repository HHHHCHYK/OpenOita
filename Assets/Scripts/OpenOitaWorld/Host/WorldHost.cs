using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public sealed class WorldHost : MonoBehaviour
{
    [SerializeField] string materialsFolder = "Assets/Data/Materials";
    [SerializeField] string coreChunkFillMaterial = "stone";
    [SerializeField] ComputeShader chunkColorCompute;
    [SerializeField] Shader chunkDisplayShader;

    readonly MaterialRegistry _materialRegistry = new();
    readonly WorldSimulation _simulation = new();
    readonly WorldRender _worldRender = new();

    Transform _chunkRoot;

    void Awake()
    {
        ResolveEditorAssets();

        _chunkRoot = new GameObject("ChunkRoot").transform;
        _chunkRoot.SetParent(transform, false);

        _materialRegistry.LoadFromDirectory(materialsFolder);
        WorldGenerator.FillCoreChunk(_simulation.Chunks, _materialRegistry, coreChunkFillMaterial);

        Shader displayShader = chunkDisplayShader != null
            ? chunkDisplayShader
            : Shader.Find("OpenOita/ChunkDisplay");

        if (chunkColorCompute == null)
        {
            Debug.LogError("WorldHost is missing chunkColorCompute.");
            return;
        }

        if (displayShader == null)
        {
            Debug.LogError("WorldHost could not resolve ChunkDisplay shader.");
            return;
        }

        _worldRender.Initialize(_materialRegistry, chunkColorCompute, _chunkRoot, displayShader);

        WorldChunk coreChunk = _simulation.Chunks.GetChunk(0, 0);
        _worldRender.SyncChunk(coreChunk);
        _worldRender.EnsureDisplay(coreChunk);
    }

    void OnDestroy()
    {
        _worldRender.Dispose();
        _simulation.Dispose();
    }

    void ResolveEditorAssets()
    {
#if UNITY_EDITOR
        if (chunkColorCompute == null)
        {
            chunkColorCompute = AssetDatabase.LoadAssetAtPath<ComputeShader>(
                "Assets/Shaders/WorldChunkColor.compute");
        }

        if (chunkDisplayShader == null)
        {
            chunkDisplayShader = Shader.Find("OpenOita/ChunkDisplay");
        }
#endif
    }
}
