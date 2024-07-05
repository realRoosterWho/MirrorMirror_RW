using UnityEngine;

public class CCTVPlaneLogic : MonoBehaviour
{
    public Camera myCamera; // 公开的摄像机变量
    public GameObject m_Plane; // 公开的目标物体变量
    public bool invertDimensions; // 公开的布尔变量决定是否调转宽高
    private RenderTexture renderTexture; // RenderTexture变量

    // Start is called before the first frame update
    void Start()
    {
        // 获取平面的Renderer组件并读取其尺寸
        Renderer planeRenderer = m_Plane.GetComponent<Renderer>();
        if (planeRenderer == null)
        {
            Debug.LogError("Plane Renderer not found!");
            return;
        }
        float planeWidth = planeRenderer.bounds.size.x;
        float planeHeight = planeRenderer.bounds.size.y;
        float planeDepth = planeRenderer.bounds.size.z;

        // 确定最小轴，并排除该轴，选择其他两个轴作为宽和高
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

        // 根据布尔变量决定是否调转宽高
        if (invertDimensions)
        {
            float temp = planeWidth;
            planeWidth = planeHeight;
            planeHeight = temp;
        }

        Debug.Log("planeWidth: " + planeWidth + ", planeHeight: " + planeHeight);

        // 创建一个新的RenderTexture，使用平面的宽度和高度
        int textureWidth = Mathf.RoundToInt(planeWidth * 100); // 乘以100是为了增加分辨率，可以根据需要调整
        int textureHeight = Mathf.RoundToInt(planeHeight * 100); // 乘以100是为了增加分辨率，可以根据需要调整

        renderTexture = new RenderTexture(textureWidth, textureHeight, 24);
        // 设置摄像机的目标纹理为新创建的RenderTexture
        myCamera.targetTexture = renderTexture;
        // 获取平面的Renderer组件，并将其材质的主纹理设置为新创建的RenderTexture
        planeRenderer.material.mainTexture = renderTexture;

        // 调整摄像机的视野
        // AdjustCameraViewport(planeWidth, planeHeight);
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

    // Update is called once per frame
    void Update()
    {
        
    }
}