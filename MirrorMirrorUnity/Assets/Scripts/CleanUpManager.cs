using UnityEngine;
using UnityEngine.SceneManagement;

public class CleanUpManager : MonoBehaviour
{
    public bool isGotoNextScene = false; // 是否跳转到下一个场景
    public string targetSceneName; // 目标场景名称

    void Start()
    {
        // 进行内存清理
        CleanMemory();

        if (isGotoNextScene)
        {
            // 加载目标场景
            LoadTargetScene();
        }
    }

    void OnDisable()
    {
        CleanUpResources();
    }

    void OnDestroy()
    {
        CleanUpResources();
    }

    // 释放未使用的资源
    private void CleanUpResources()
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
                renderTexture = null;
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

    // 手动清理内存
    public void CleanMemory()
    {
        CleanUpResources();
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    // 加载目标场景
    private void LoadTargetScene()
    {
        CleanMemory();
        SceneManager.LoadScene(targetSceneName);
    }
}