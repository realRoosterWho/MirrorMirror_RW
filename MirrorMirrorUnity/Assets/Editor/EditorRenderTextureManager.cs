using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class EditorRenderTextureManager
{
    static EditorRenderTextureManager()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            CleanUpRenderTextures();
        }
    }

    [MenuItem("Tools/Clean Up RenderTextures")]
    public static void CleanUpRenderTexturesMenu()
    {
        CleanUpRenderTextures();
    }

    private static void CleanUpRenderTextures()
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

                Debug.Log($"Releasing and destroying target texture for camera: {camera.name}");
                camera.targetTexture = null; // 解除相机的 RenderTexture 绑定
                renderTexture.Release(); // 释放 RenderTexture
                UnityEngine.Object.DestroyImmediate(renderTexture); // 销毁 RenderTexture
                renderTexture = null; // 确保 RenderTexture 设为 null
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