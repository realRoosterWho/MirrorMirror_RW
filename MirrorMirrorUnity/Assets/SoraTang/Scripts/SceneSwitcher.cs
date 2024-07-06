using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public string playerTag = "Player";  // 玩家对象的Tag

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // 检查是否存在下一个场景
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }

    }
}
