using UnityEngine;

public class CCTVPlaneLogic : MonoBehaviour
{
    public Camera myCamera; // 公开的摄像机变量
    public GameObject m_Plane; // 公开的目标物体变量
    public bool invertDimensions; // 公开的布尔变量决定是否调转宽高
    private RenderTexture renderTexture; // RenderTexture变量
    public GameObject cctvPrefab; // CCTV Prefab

    public bool isSummonRevert = false;

    // Start is called before the first frame update
    void Start()
    {
        AdjustPlaneSizeAndRenderTexture(true);
    }

    // 调整摄像机的视野以匹配RenderTexture的宽高比
    void AdjustCameraViewport(float planeWidth, float planeHeight)
    {
        float targetAspect = planeWidth / planeHeight;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            Rect rect = myCamera.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            myCamera.rect = rect;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = myCamera.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            myCamera.rect = rect;
        }
    }

    // 提取的公共方法
    public void AdjustPlaneSizeAndRenderTexture(bool isRender = false)
    {
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

        Debug.Log("planeWidth: " + planeWidth + ", planeHeight: " + planeHeight);

        int textureWidth = Mathf.RoundToInt(planeWidth * 100); // 乘以100是为了增加分辨率，可以根据需要调整
        int textureHeight = Mathf.RoundToInt(planeHeight * 100); // 乘以100是为了增加分辨率，可以根据需要调整

        renderTexture = new RenderTexture(textureWidth, textureHeight, 24);
        myCamera.targetTexture = renderTexture;

        planeRenderer.material = null; // 清空主材质

        if (Application.isPlaying)
        {
            planeRenderer.material.mainTexture = renderTexture;
        }

        if(isRender)
        {
            planeRenderer.material.mainTexture = renderTexture;
        }
        else
        {
            planeRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit")); // 占位材质
        }

        // 调整摄像机的视野
        // AdjustCameraViewport(planeWidth, planeHeight);
    }

    public void UpdateRenderTexture(bool isRender)
    {
        AdjustPlaneSizeAndRenderTexture(isRender);
    }

    public void AddCCTVPlaneOnSurface(GameObject targetObject, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (cctvPrefab == null)
        {
            Debug.LogError("CCTV Prefab is not assigned!");
            return;
        }

        Renderer targetRenderer = targetObject.GetComponent<Renderer>();
        if (targetRenderer == null)
        {
            Debug.LogError("Target object does not have a Renderer component!");
            return;
        }

        GameObject cctvInstance = Instantiate(cctvPrefab);
        Transform planeTransform = cctvInstance.GetComponent<CCTVPlaneLogic>().m_Plane.transform;
        if (planeTransform == null)
        {
            Debug.LogError("Plane child object not found in the prefab!");
            return;
        }

        Renderer cctvRenderer = planeTransform.GetComponent<Renderer>();
        if (cctvRenderer == null)
        {
            Debug.LogError("Plane child object does not have a Renderer component!");
            return;
        }

        float planeWidth, planeHeight;
        Vector3 planeCenter;
        Quaternion rotation = Quaternion.identity;
        float offset = 0.01f; // 微小的偏移量

        if (Mathf.Abs(hitNormal.y) > Mathf.Abs(hitNormal.x) && Mathf.Abs(hitNormal.y) > Mathf.Abs(hitNormal.z))
        {
            planeWidth = targetRenderer.bounds.size.x;
            planeHeight = targetRenderer.bounds.size.z;
            planeCenter = targetRenderer.bounds.center + new Vector3(0, hitNormal.y * targetRenderer.bounds.extents.y, 0);
            rotation = Quaternion.LookRotation(hitNormal, Vector3.up) * Quaternion.Euler(90, 0, 0); // 旋转
            Debug.Log("x,z");
        }
        else if (Mathf.Abs(hitNormal.x) > Mathf.Abs(hitNormal.y) && Mathf.Abs(hitNormal.x) > Mathf.Abs(hitNormal.z))
        {
            planeWidth = targetRenderer.bounds.size.z;
            planeHeight = targetRenderer.bounds.size.y;
            planeCenter = targetRenderer.bounds.center + new Vector3(hitNormal.x * targetRenderer.bounds.extents.x, 0, 0);
            rotation = Quaternion.LookRotation(hitNormal, Vector3.up) * Quaternion.Euler(90, 0, 0);
            Debug.Log("z,y");
        }
        else
        {
            planeWidth = targetRenderer.bounds.size.x;
            planeHeight = targetRenderer.bounds.size.y;
            planeCenter = targetRenderer.bounds.center + new Vector3(0, 0, hitNormal.z * targetRenderer.bounds.extents.z);
            rotation = Quaternion.LookRotation(hitNormal, Vector3.up) * Quaternion.Euler(-90, 0, 0);
            Debug.Log("x,y");
        }

        float standardPlaneSize = 10.0f;
        planeTransform.localScale = new Vector3(planeWidth / standardPlaneSize, 1.0f, planeHeight / standardPlaneSize); // Adjust scale based on standard size

        planeTransform.position = planeCenter + hitNormal * offset; // 添加微小的偏移量
        planeTransform.rotation = rotation;

        if (isSummonRevert)
        {
            cctvInstance.GetComponent<CCTVPlaneLogic>().invertDimensions = true;
        }

        cctvInstance.GetComponent<CCTVPlaneLogic>().m_Plane = planeTransform.gameObject; // 确保引用正确的 plane 对象
        cctvInstance.GetComponent<CCTVPlaneLogic>().AdjustPlaneSizeAndRenderTexture();
    }
}