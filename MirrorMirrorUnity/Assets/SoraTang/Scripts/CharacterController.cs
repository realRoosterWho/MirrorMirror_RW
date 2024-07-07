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
    private float currentTurnSpeed;

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

        if (isOnLadder)
        {
            return;
        }
        Vector3 move = transform.forward * moveZ;
        rb.MovePosition(transform.position + move * moveSpeed * Time.deltaTime);
    }

    void Turn()
    {
        float turn = Input.GetAxis("Horizontal");

        if (turn != 0 && !isOnLadder)
        {
            currentTurnSpeed = turn * turnSpeed;
            Quaternion targetRotation = Quaternion.Euler(0, currentTurnSpeed, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * targetRotation, Time.deltaTime);
        }
        else
        {
            // 如果没有按下转向键，立即停止旋转
            currentTurnSpeed = 0;
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
            float moveY = Input.GetAxis("Vertical");
            float moveX = Input.GetAxis("Horizontal");
            

            rb.useGravity = false; // 关闭重力
            Vector3 climbVelocity = new Vector3(0, moveY * ladderSpeed, 0);
            rb.velocity = climbVelocity;
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ladderTag))
        {
            isOnLadder = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(ladderTag))
        {
            isOnLadder = false;
        }
    }
}
