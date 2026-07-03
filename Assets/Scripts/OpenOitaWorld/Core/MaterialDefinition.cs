using System;
using UnityEngine;

[Serializable]
public class MaterialDefinition
{
    public int id;
    public string name;
    public string color;
    public float gravityMultiplier;
    public float mass;
    public float strength;

    public Color ParseColor()
    {
        if (string.IsNullOrEmpty(color))
            return Color.magenta;

        if (ColorUtility.TryParseHtmlString(color, out Color parsed))
            return parsed;

        Debug.LogWarning($"Invalid color '{color}' for material '{name}', using magenta.");
        return Color.magenta;
    }
}
