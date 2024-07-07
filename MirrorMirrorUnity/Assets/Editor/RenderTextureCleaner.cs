using UnityEditor;
using UnityEngine;

public class RenderTextureCleaner : EditorWindow
{
    [MenuItem("Tools/RenderTexture Cleaner")]
    public static void ShowWindow()
    {
        GetWindow<RenderTextureCleaner>("RenderTexture Cleaner");
    }

    void OnGUI()
    {
        if (GUILayout.Button("Clean Up RenderTextures"))
        {
            CleanUpRenderTextures();
        }
    }

    private void CleanUpRenderTextures()
    {
        // 获取所有相机
        Camera[] allCameras = Camera.allCameras;

        foreach (Camera camera in allCameras)
        {
            Debug.Log($"Cleaning camera: {camera.name}");

            // 释放相机的目标 RenderTexture
            if (camera.targetTexture != null)
            {
                RenderTexture renderTexture = camera.targetTexture;

                Debug.Log($"Releasing target texture for camera: {camera.name}");
                camera.targetTexture = null;
                renderTexture.Release();
                DestroyImmediate(renderTexture);
            }

            // 确认释放相机的深度纹理
            if (camera.depthTextureMode != DepthTextureMode.None)
            {
                Debug.Log($"Releasing depth texture for camera: {camera.name}");
                camera.depthTextureMode = DepthTextureMode.None;
            }
        }

        // 强制清理未使用的资源和进行垃圾回收
        Debug.Log("Unloading unused assets and forcing garbage collection...");
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}