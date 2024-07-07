using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CCTVPlaneLogic))]
public class CCTVPlaneLogicEditor : Editor
{
    private bool isAddingCCTVPlane = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CCTVPlaneLogic cctvLogic = (CCTVPlaneLogic)target;

        if (GUILayout.Button("Add CCTV Plane"))
        {
            isAddingCCTVPlane = true;
            SceneView.duringSceneGui += OnSceneGUI;
        }

		if (GUILayout.Button("Adjust Plane Size And Render Texture"))
        {
            cctvLogic.AdjustPlaneSizeAndRenderTexture(true);
        }
		if (GUILayout.Button("Adjust Plane Size And Render Texture but False"))
        {
            cctvLogic.AdjustPlaneSizeAndRenderTexture(false);
        }
        
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (isAddingCCTVPlane)
        {
            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    CCTVPlaneLogic cctvLogic = (CCTVPlaneLogic)target;
                    cctvLogic.AddCCTVPlaneOnSurface(hit.collider.gameObject, hit.point, hit.normal);
                    isAddingCCTVPlane = false;
                    SceneView.duringSceneGui -= OnSceneGUI;
                }
                e.Use();
            }
        }
    }
}