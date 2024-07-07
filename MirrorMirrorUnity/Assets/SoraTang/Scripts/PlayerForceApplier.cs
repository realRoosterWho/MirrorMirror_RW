using UnityEngine;

public class PlayerForceApplier : MonoBehaviour
{
    public string playerTag = "Player";  // 玩家对象的Tag
    public float forceMagnitude = 10f;  // 施加的力的大小
    public float upwardForceMultiplier = 1f;  // 向上的力的倍数

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                Vector3 forwardForce = other.transform.forward * forceMagnitude;
                Vector3 upwardForce = Vector3.up * forceMagnitude * upwardForceMultiplier;
                Vector3 totalForce = forwardForce + upwardForce;

                playerRigidbody.AddForce(totalForce, ForceMode.Impulse);
            }
        }
    }
}
