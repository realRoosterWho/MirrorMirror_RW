using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;       // 移动速度
    public float turnSpeed = 5f;       // 转向速度
    public float jumpForce = 5f;       // 跳跃力量
    public string groundTag = "Ground"; // 地面的Tag
    public string ladderTag = "Ladder"; // 梯子的Tag
    public float ladderSpeed = 3f;     // 在梯子上移动的速度

    private Rigidbody rb;
    private bool isGrounded;
    private bool isOnLadder;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true; // 开启重力
    }

    void Update()
    {
        Move();
        Turn();
        Jump();
        ClimbLadder();
    }

    void Move()
    {
        float moveZ = Input.GetAxis("Vertical");

        if (!isOnLadder)
        {
            Vector3 move = transform.forward * moveZ;
            rb.MovePosition(transform.position + move * moveSpeed * Time.deltaTime);
        }
    }

    void Turn()
    {
        float turn = Input.GetAxis("Horizontal");

        if (turn != 0)
        {
            Quaternion targetRotation = Quaternion.Euler(0, turn * turnSpeed, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * targetRotation, Time.deltaTime);
        }
    }

    void Jump()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void ClimbLadder()
    {
        if (isOnLadder)
        {
            float vertical = Input.GetAxis("Vertical");
            rb.useGravity = false; // 关闭重力
            rb.velocity = new Vector3(rb.velocity.x, vertical * ladderSpeed, rb.velocity.z);
        }
        else
        {
            rb.useGravity = true; // 开启重力
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(groundTag))
        {
            isGrounded = true;
        }

        if (collision.collider.CompareTag(ladderTag))
        {
            isOnLadder = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag(groundTag))
        {
            isGrounded = false;
        }

        if (collision.collider.CompareTag(ladderTag))
        {
            isOnLadder = false;
        }
    }
}
