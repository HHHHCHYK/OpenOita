using System;

[Serializable]
public class TestMaterialDto
{
    public string type;
    public int id;
    public string name;
    public string color;
    public float gravityMultiplier;
    public float mass;
    public float strength;

    public bool IsTestMaterial =>
        string.Equals(type, "TestMaterial", StringComparison.Ordinal);

    public MaterialDefinition ToDefinition()
    {
        return new MaterialDefinition
        {
            id = id,
            name = name,
            color = color,
            gravityMultiplier = gravityMultiplier,
            mass = mass,
            strength = strength
        };
    }
}
