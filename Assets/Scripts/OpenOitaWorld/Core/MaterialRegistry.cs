using System;
using System.Collections.Generic;
using UnityEngine;

public class MaterialRegistry
{
    readonly Dictionary<int, MaterialDefinition> _byId = new();
    readonly Dictionary<string, MaterialDefinition> _byName = new(StringComparer.OrdinalIgnoreCase);
    readonly List<MaterialDefinition> _sorted = new();

    public IReadOnlyList<MaterialDefinition> Materials => _sorted;

    public void LoadFromDirectory(string materialsFolderPath)
    {
        _byId.Clear();
        _byName.Clear();
        _sorted.Clear();

        foreach (string filePath in MaterialJsonLoader.EnumerateJsonFiles(materialsFolderPath))
        {
            if (!MaterialJsonLoader.TryLoadText(ToAssetRelativePath(filePath), out string json))
                continue;

            TestMaterialDto dto;
            try
            {
                dto = JsonUtility.FromJson<TestMaterialDto>(json);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to parse material JSON '{filePath}': {ex.Message}");
                continue;
            }

            if (dto == null || !dto.IsTestMaterial)
                continue;

            if (string.IsNullOrWhiteSpace(dto.name))
            {
                Debug.LogWarning($"Skipping material JSON without name: {filePath}");
                continue;
            }

            if (_byId.ContainsKey(dto.id) || _byName.ContainsKey(dto.name))
            {
                Debug.LogWarning($"Duplicate material id or name in '{filePath}', skipping.");
                continue;
            }

            MaterialDefinition definition = dto.ToDefinition();
            _byId[definition.id] = definition;
            _byName[definition.name] = definition;
            _sorted.Add(definition);
        }

        _sorted.Sort((a, b) => a.id.CompareTo(b.id));

        if (_sorted.Count == 0)
            Debug.LogWarning($"No TestMaterial definitions loaded from '{materialsFolderPath}'.");
    }

    public bool TryGetById(int id, out MaterialDefinition definition) =>
        _byId.TryGetValue(id, out definition);

    public bool TryGetByName(string name, out MaterialDefinition definition) =>
        _byName.TryGetValue(name, out definition);

    public Color GetColor(int materialId)
    {
        if (TryGetById(materialId, out MaterialDefinition definition))
            return definition.ParseColor();

        return Color.magenta;
    }

    public int MaxMaterialId
    {
        get
        {
            int max = 0;
            foreach (MaterialDefinition material in _sorted)
                max = Math.Max(max, material.id);

            return max;
        }
    }

    static string ToAssetRelativePath(string fullPath)
    {
        fullPath = fullPath.Replace('\\', '/');
        int assetsIndex = fullPath.IndexOf("Assets/", StringComparison.Ordinal);
        return assetsIndex >= 0 ? fullPath.Substring(assetsIndex) : fullPath;
    }
}
