using UnityEngine;
using System.Collections;

public class ItemActivator : MonoBehaviour
{
    public string playerTag = "Player";  // 玩家对象的Tag
    public GameObject[] itemsToActivate; // 需要激活的物品
    public GameObject[] itemsToDeactivate; // 需要禁用的物品
    public GameObject[] itemsToScale; // 需要从零缩放到设定大小的物品
    public Vector3[] targetScales; // 每个物体的目标缩放大小
    public float scaleDuration = 1f;  // 缩放持续时间

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            ActivateItems();
            DeactivateItems();
            StartScalingItems();
        }
    }

    void ActivateItems()
    {
        foreach (GameObject item in itemsToActivate)
        {
            if (item != null)
            {
                item.SetActive(true);
            }
        }
    }

    void DeactivateItems()
    {
        foreach (GameObject item in itemsToDeactivate)
        {
            if (item != null)
            {
                item.SetActive(false);
            }
        }
    }

    void StartScalingItems()
    {
        for (int i = 0; i < itemsToScale.Length; i++)
        {
            if (itemsToScale[i] != null)
            {
                StartCoroutine(ScaleItem(itemsToScale[i], Vector3.zero, targetScales[i], scaleDuration));
            }
        }
    }

    IEnumerator ScaleItem(GameObject item, Vector3 fromScale, Vector3 toScale, float duration)
    {
        float elapsedTime = 0f;
        item.transform.localScale = fromScale;

        while (elapsedTime < duration)
        {
            item.transform.localScale = Vector3.Lerp(fromScale, toScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        item.transform.localScale = toScale;
    }
}
