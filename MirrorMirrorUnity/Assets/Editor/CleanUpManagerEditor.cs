using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CleanUpManager))]
public class CleanUpManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CleanUpManager myScript = (CleanUpManager)target;
        if (GUILayout.Button("Clean Memory"))
        {
            myScript.CleanMemory();
        }
    }
}