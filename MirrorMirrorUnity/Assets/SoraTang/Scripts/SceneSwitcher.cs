using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public string playerTag = "Player";  // 玩家对象的Tag
    public bool loadSpecificScene = false;  // 是否加载指定场景
    public string sceneName;  // 指定场景名称

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            LoadScene(loadSpecificScene);
        }
    }

    void LoadScene(bool isSpecific)
    {
        if (isSpecific)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
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
