using UnityEditor;
using UnityEngine;
using System.Collections;

public class GlobalResourceCleaner : EditorWindow
{
    private const int BatchSize = 10; // 每次处理的对象数量
    private IEnumerator cleanupCoroutine;

    [MenuItem("Tools/Global Resource Cleaner")]
    public static void ShowWindow()
    {
        GetWindow<GlobalResourceCleaner>("Global Resource Cleaner");
    }

    void OnGUI()
    {
        if (GUILayout.Button("Clean Up All RenderTextures and Materials"))
        {
            StartCleanup();
        }
    }

    private void StartCleanup()
    {
        cleanupCoroutine = CleanUpCoroutine();
        EditorApplication.update += OnEditorUpdate;
    }

    private void OnEditorUpdate()
    {
        if (cleanupCoroutine != null && !cleanupCoroutine.MoveNext())
        {
            cleanupCoroutine = null;
            EditorApplication.update -= OnEditorUpdate;
            Debug.Log("Cleanup completed.");
        }
    }

    private IEnumerator CleanUpCoroutine()
    {
        // 分批次清理 RenderTextures
        RenderTexture[] allRenderTextures = Resources.FindObjectsOfTypeAll<RenderTexture>();
        for (int i = 0; i < allRenderTextures.Length; i += BatchSize)
        {
            for (int j = i; j < i + BatchSize && j < allRenderTextures.Length; j++)
            {
                RenderTexture rt = allRenderTextures[j];
                if (rt != null)
                {
                    Debug.Log($"Destroying RenderTexture: {rt.name}");
                    rt.Release();
                    DestroyImmediate(rt, true);
                }
            }

            // 每批次后进行垃圾回收
            Resources.UnloadUnusedAssets();
            System.GC.Collect();

            // 等待下一帧
            yield return null;
        }

        // 分批次清理 Materials
        Material[] allMaterials = Resources.FindObjectsOfTypeAll<Material>();
        for (int i = 0; i < allMaterials.Length; i += BatchSize)
        {
            for (int j = i; j < i + BatchSize && j < allMaterials.Length; j++)
            {
                Material mat = allMaterials[j];
                if (mat != null)
                {
                    Debug.Log($"Destroying Material: {mat.name}");
                    DestroyImmediate(mat, true);
                }
            }

            // 每批次后进行垃圾回收
            Resources.UnloadUnusedAssets();
            System.GC.Collect();

            // 等待下一帧
            yield return null;
        }
    }
}