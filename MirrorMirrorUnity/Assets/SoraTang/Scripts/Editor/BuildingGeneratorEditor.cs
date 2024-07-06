using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingGenerator))]
public class BuildingGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BuildingGenerator generator = (BuildingGenerator)target;
        if (GUILayout.Button("Generate Building"))
        {
            generator.GenerateBuilding();
        }
    }
}
