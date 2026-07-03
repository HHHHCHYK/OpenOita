using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public sealed class ChunkDisplay : MonoBehaviour
{
    static Mesh _sharedMesh;

    MeshRenderer _meshRenderer;

    public void Initialize(Shader displayShader, RenderTexture colorTexture)
    {
        var meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();

        meshFilter.sharedMesh = GetSharedMesh();

        var material = new Material(displayShader);
        material.mainTexture = colorTexture;
        _meshRenderer.sharedMaterial = material;
    }

    public void SetTexture(RenderTexture colorTexture)
    {
        if (_meshRenderer != null && _meshRenderer.sharedMaterial != null)
            _meshRenderer.sharedMaterial.mainTexture = colorTexture;
    }

    static Mesh GetSharedMesh()
    {
        if (_sharedMesh != null)
            return _sharedMesh;

        _sharedMesh = new Mesh
        {
            vertices = new[]
            {
                new Vector3(0f, 0f, 0f),
                new Vector3(1f, 0f, 0f),
                new Vector3(1f, 1f, 0f),
                new Vector3(0f, 1f, 0f)
            },
            uv = new[]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 1f),
                new Vector2(0f, 1f)
            },
            triangles = new[] { 0, 1, 2, 0, 2, 3 }
        };
        _sharedMesh.RecalculateBounds();
        return _sharedMesh;
    }
}
