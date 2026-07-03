using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class MaterialJsonLoader
{
    public static List<string> EnumerateJsonFiles(string materialsFolderPath)
    {
        var results = new List<string>();

#if UNITY_EDITOR
        string folder = NormalizeFolderPath(materialsFolderPath);
        if (!Directory.Exists(folder))
        {
            Debug.LogError($"Materials folder not found: {folder}");
            return results;
        }

        foreach (string file in Directory.GetFiles(folder, "*.json", SearchOption.TopDirectoryOnly))
            results.Add(file.Replace('\\', '/'));
#else
        Debug.LogError("MaterialJsonLoader only supports Editor Play in this MVP build.");
#endif

        return results;
    }

    public static bool TryLoadText(string assetPath, out string text)
    {
        text = null;

#if UNITY_EDITOR
        string projectRoot = Directory.GetParent(Application.dataPath).FullName.Replace('\\', '/');
        string fullPath = Path.Combine(projectRoot, assetPath).Replace('\\', '/');

        if (!File.Exists(fullPath))
            return false;

        text = File.ReadAllText(fullPath);
        return true;
#else
        return false;
#endif
    }

    static string NormalizeFolderPath(string path)
    {
        path = path.Replace('\\', '/');
        if (path.StartsWith("Assets/"))
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName.Replace('\\', '/');
            return Path.Combine(projectRoot, path).Replace('\\', '/');
        }

        return path;
    }
}
