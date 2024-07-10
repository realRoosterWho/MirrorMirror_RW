#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class CCTVPlaneLogic : MonoBehaviour
{
    public Camera myCamera;
    public GameObject m_Plane;
    public bool invertDimensions;
    private RenderTexture renderTexture;

    public bool isSummonRevert = false;

    void Start()
    {
        AdjustPlaneSizeAndRenderTexture(true);
    }

    void OnDisable()
    {
        CleanUpSpecificRenderTextures();
        ReleaseRenderTexture();
    }

    void OnDestroy()
    {
        CleanUpSpecificRenderTextures();
        ReleaseRenderTexture();
    }

    private void ReleaseRenderTexture()
    {
        if (renderTexture != null)
        {
            if (myCamera.targetTexture == renderTexture)
            {
                myCamera.targetTexture = null;
            }
            renderTexture.Release();
            DestroyImmediate(renderTexture, true);
            renderTexture = null;
        }
    }

    public void AdjustPlaneSizeAndRenderTexture(bool isRender = false)
    {
        Debug.Log("Adjusting plane size and render texture...");
        Renderer planeRenderer = m_Plane.GetComponent<Renderer>();
        if (planeRenderer == null)
        {
            Debug.LogError("Plane Renderer not found!");
            return;
        }

        float planeWidth = planeRenderer.bounds.size.x;
        float planeHeight = planeRenderer.bounds.size.y;
        float planeDepth = planeRenderer.bounds.size.z;

        if (planeWidth < planeHeight && planeWidth < planeDepth)
        {
            planeWidth = planeRenderer.bounds.size.y;
            planeHeight = planeRenderer.bounds.size.z;
        }
        else if (planeHeight < planeWidth && planeHeight < planeDepth)
        {
            planeWidth = planeRenderer.bounds.size.x;
            planeHeight = planeRenderer.bounds.size.z;
        }
        else
        {
            planeWidth = planeRenderer.bounds.size.x;
            planeHeight = planeRenderer.bounds.size.y;
        }

        if (invertDimensions)
        {
            float temp = planeWidth;
            planeWidth = planeHeight;
            planeHeight = temp;
        }

        int textureWidth = Mathf.RoundToInt(planeWidth * 15);
        int textureHeight = Mathf.RoundToInt(planeHeight * 15);

        if (renderTexture == null || renderTexture.width != textureWidth || renderTexture.height != textureHeight)
        {
            ReleaseRenderTexture();

            renderTexture = new RenderTexture(textureWidth, textureHeight, 24);
            renderTexture.Create();
            myCamera.targetTexture = renderTexture;
        }

        if (Application.isEditor)
        {
            // 编辑模式下，创建一个新的材质实例
            if (isRender)
            {
                Material newMaterial = new Material(planeRenderer.sharedMaterial);
                newMaterial.mainTexture = renderTexture;
                planeRenderer.sharedMaterial = newMaterial;
            }
            else
            {
                planeRenderer.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            }
        }
        else
        {
            // 运行时使用 material
            if (isRender)
            {
                planeRenderer.material.mainTexture = renderTexture;
            }
            else
            {
                planeRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            }
        }
    }

    public void AddCCTVPlaneOnSurface(GameObject targetObject, Vector3 hitPoint, Vector3 hitNormal)
    {
        Renderer targetRenderer = targetObject.GetComponent<Renderer>();
        if (targetRenderer == null)
        {
            Debug.LogError("Target object does not have a Renderer component!");
            return;
        }

        Renderer planeRenderer = m_Plane.GetComponent<Renderer>();
        if (planeRenderer == null)
        {
            Debug.LogError("Plane Renderer not found!");
            return;
        }

#if UNITY_EDITOR
        Undo.RecordObject(m_Plane.transform, "Adjust CCTV Plane");
#endif

        float planeWidth, planeHeight;
        Vector3 planeCenter;
        Quaternion rotation = Quaternion.identity;
        float offset = 0.01f; // 微小的偏移量

        if (Mathf.Abs(hitNormal.y) > Mathf.Abs(hitNormal.x) && Mathf.Abs(hitNormal.y) > Mathf.Abs(hitNormal.z))
        {
            planeWidth = targetRenderer.bounds.size.x;
            planeHeight = targetRenderer.bounds.size.z;
            planeCenter = targetRenderer.bounds.center + new Vector3(0, hitNormal.y * targetRenderer.bounds.extents.y, 0);
            rotation = Quaternion.LookRotation(hitNormal, Vector3.up) * Quaternion.Euler(90, 0, 0);
        }
        else if (Mathf.Abs(hitNormal.x) > Mathf.Abs(hitNormal.y) && Mathf.Abs(hitNormal.x) > Mathf.Abs(hitNormal.z))
        {
            planeWidth = targetRenderer.bounds.size.z;
            planeHeight = targetRenderer.bounds.size.y;
            planeCenter = targetRenderer.bounds.center + new Vector3(hitNormal.x * targetRenderer.bounds.extents.x, 0, 0);
            rotation = Quaternion.LookRotation(hitNormal, Vector3.up) * Quaternion.Euler(90, 0, 0);
        }
        else
        {
            planeWidth = targetRenderer.bounds.size.x;
            planeHeight = targetRenderer.bounds.size.y;
            planeCenter = targetRenderer.bounds.center + new Vector3(0, 0, hitNormal.z * targetRenderer.bounds.extents.z);
            rotation = Quaternion.LookRotation(hitNormal, Vector3.up) * Quaternion.Euler(-90, 0, 0);
        }

        float standardPlaneSize = 10.0f;
        m_Plane.transform.localScale = new Vector3(planeWidth / standardPlaneSize, 1.0f, planeHeight / standardPlaneSize);

        m_Plane.transform.position = planeCenter + hitNormal * offset;
        m_Plane.transform.rotation = rotation;

        if (isSummonRevert)
        {
            invertDimensions = true;
        }

        // 如果需要调整大小和渲染纹理
        // AdjustPlaneSizeAndRenderTexture(true);
    }

    // 静态方法清理特定模式的 RenderTexture
    public static void CleanUpSpecificRenderTextures()
    {
        string[] patterns = { "_CameraDepthAttachment_", "_CameraDepthTexture_", "_Camera", "_SSAO", "_AfterPost" };
        RenderTexture[] allRenderTextures = Resources.FindObjectsOfTypeAll<RenderTexture>();

        foreach (var rt in allRenderTextures)
        {
            if (rt != null && MatchesAnyPattern(rt.name, patterns))
            {
                Debug.Log($"Destroying RenderTexture: {rt.name}");
                rt.Release();
                DestroyImmediate(rt, true);
            }
        }

        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    private static bool MatchesAnyPattern(string name, string[] patterns)
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