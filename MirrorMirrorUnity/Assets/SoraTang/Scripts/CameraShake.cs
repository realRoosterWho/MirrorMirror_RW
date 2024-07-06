using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeAmount = 0.1f; // 晃动的幅度
    public float shakeSpeed = 1f;    // 晃动的速度

    private Vector3 originalPosition;

    void Start()
    {
        // 记录相机的初始位置
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        // 计算Perlin噪声的偏移值
        float offsetX = Mathf.PerlinNoise(Time.time * shakeSpeed , 0f) * 2 - 1;
        float offsetY = Mathf.PerlinNoise(0f, Time.time * shakeSpeed) * 2 - 1;
        float offsetZ = Mathf.PerlinNoise(Time.time * shakeSpeed, Time.time * shakeSpeed) * 2 - 1;

        // 应用偏移值到相机的位置
        Vector3 shakeOffset = new Vector3(offsetX, offsetY, offsetZ) * shakeAmount;
        transform.localPosition = originalPosition + shakeOffset;
    }
}
