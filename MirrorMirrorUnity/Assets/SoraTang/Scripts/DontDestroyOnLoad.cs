using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    void Awake()
    {
        // 确保该对象在切换场景时不会被销毁
        DontDestroyOnLoad(gameObject);
    }
}
