using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;       // 移动速度
    public float turnSpeed = 5f;       // 转向速度
    public float jumpForce = 5f;       // 跳跃力量
    public LayerMask groundLayer;      // 地面的Layer
    public string ladderTag = "Ladder"; // 梯子的Tag
    public float ladderSpeed = 3f;     // 在梯子上移动的速度

    private Rigidbody rb;
    private bool isGrounded;
    private bool isOnLadder;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true; // 开启重力

        // 永久锁定ZX轴的旋转
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        Move();
        Turn();
        Jump();
        ClimbLadder();

        // 检测是否在地面上
        CheckGrounded();
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

        if (turn != 0 && !isOnLadder)
        {
            Quaternion targetRotation = Quaternion.Euler(0, turn * turnSpeed, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * targetRotation, Time.deltaTime);
        }
    }

    void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; // 跳跃后立即设置为不接触地面
        }
    }

    void ClimbLadder()
    {
        if (isOnLadder)
        {
            float vertical = Input.GetAxis("Vertical");
            rb.useGravity = false; // 关闭重力
            rb.velocity = new Vector3(0, vertical * ladderSpeed, 0); // 只在Y轴上移动
        }
        else
        {
            rb.useGravity = true; // 开启重力
        }
    }

    void CheckGrounded()
    {
        RaycastHit hit;
        float rayDistance = 1.1f; // 射线距离，略大于角色底部到地面的距离

        // 向下发射一条射线，检测是否击中地面
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(ladderTag))
        {
            isOnLadder = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag(ladderTag))
        {
            isOnLadder = false;
        }
    }
}
