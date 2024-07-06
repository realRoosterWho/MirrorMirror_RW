using UnityEngine;

public class FloatAndRotate : MonoBehaviour
{
    public float floatAmplitude = 0.5f;  // 漂浮的幅度
    public float floatFrequency = 1f;    // 漂浮的频率
    public Vector3 rotationSpeed = new Vector3(15f, 30f, 45f); // 每轴的旋转速度

    private Vector3 startPos;

    void Start()
    {
        // 记录物体的初始位置
        startPos = transform.localPosition;
    }

    void Update()
    {
        // 漂浮效果
        float floatOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.localPosition = startPos + new Vector3(0f, floatOffset, 0f);

        // 随机旋转效果
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
