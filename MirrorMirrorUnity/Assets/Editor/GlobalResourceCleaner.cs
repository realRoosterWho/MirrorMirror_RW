using UnityEditor;
using UnityEngine;
using System.Collections;

public class GlobalResourceCleaner : EditorWindow
{
    private const int BatchSize = 100; // 每次处理的对象数量
    private IEnumerator cleanupCoroutine;

    [MenuItem("Tools/Global Resource Cleaner")]
    public static void ShowWindow()
    {
        GetWindow<GlobalResourceCleaner>("Global Resource Cleaner");
    }

    void OnGUI()
    {
        if (GUILayout.Button("Clean Up Specific RenderTextures"))
        {
            StartCleanup(SpecificRenderTextureCleanupCoroutine);
        }
    }

    private void StartCleanup(System.Func<IEnumerator> coroutineFunc)
    {
        cleanupCoroutine = coroutineFunc();
        EditorApplication.update += OnEditorUpdate;
    }

    private void OnEditorUpdate()
    {
        try
        {
            if (cleanupCoroutine != null && !cleanupCoroutine.MoveNext())
            {
                cleanupCoroutine = null;
                EditorApplication.update -= OnEditorUpdate;
                Debug.Log("Cleanup completed.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Exception during cleanup: {ex}");
            cleanupCoroutine = null;
            EditorApplication.update -= OnEditorUpdate;
        }
    }

    private IEnumerator SpecificRenderTextureCleanupCoroutine()
    {
        string[] patterns = { "_CameraDepthAttachment_", "_CameraDepthTexture_","Camera","_SSAO" }; // 模式列表
        RenderTexture[] allRenderTextures = Resources.FindObjectsOfTypeAll<RenderTexture>();

        for (int i = 0; i < allRenderTextures.Length; i++)
        {
            RenderTexture rt = allRenderTextures[i];
            if (rt != null && !EditorUtility.IsPersistent(rt) && MatchesAnyPattern(rt.name, patterns))
            {
                Debug.Log($"Destroying RenderTexture: {rt.name}");
                try
                {
                    rt.Release();
                    DestroyImmediate(rt, true);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Failed to destroy RenderTexture {rt.name}: {ex}");
                }
            }

            // 每个对象后进行垃圾回收并等待
            if (i % BatchSize == 0)
            {
                Resources.UnloadUnusedAssets();
                System.GC.Collect();
                yield return null;
            }
        }
    }

    private bool MatchesAnyPattern(string name, string[] patterns)
    {
        foreach (string pattern in patterns)
        {
            if (name.Contains(pattern))
            {
                return true;
            }
        }
        return false;
    }
}