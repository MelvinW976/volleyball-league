using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour
{
    // 移除原有的Y轴检测
    //[SerializeField] private float outOfBoundsY = -10f;

    private Rigidbody rb;
    private Vector3 startPosition;
    public string lastTouchedTeam = "";
    public GameObject lastTouchedPlayer;
    public static BallController Instance { get; private set; }
    private bool isResetting; // 新增重置状态标志
    [SerializeField] private float airDrag = 0.2f;
    [SerializeField] private float angularDrag = 0.5f;
    [SerializeField] private float bounciness = 0.8f;
    [SerializeField] private float dynamicFriction = 0.4f;

    private PhysicsMaterial ballMaterial;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        rb.linearDamping = airDrag;
        rb.angularDamping = angularDrag;

        // 创建物理材质（仅一次）
        ballMaterial = new PhysicsMaterial();
        ballMaterial.bounciness = bounciness;
        ballMaterial.dynamicFriction = dynamicFriction;
        GetComponent<Collider>().material = ballMaterial;
    }

    void FixedUpdate()
    {
        // 统一处理空气阻力
        if (!IsGrounded())
        {
            // 动态调整阻力系数
            float speedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / 20f);
            rb.linearDamping = airDrag * speedFactor;
        }
        else
        {
            rb.linearDamping = 0f;
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.1f);
    }

    // 使用碰撞检测替代Y轴位置判断
    private void OnCollisionEnter(Collision collision)
    {
        if (isResetting || GameplayManager.Instance == null) return;

        // 统一处理所有地面碰撞
        if (collision.gameObject.CompareTag("PlayerCourt") || 
            collision.gameObject.CompareTag("OpponentCourt") ||
            collision.gameObject.CompareTag("Ground"))
        {
            HandleLanding(collision.gameObject);
        }
    }

    private void HandleLanding(GameObject surface)
    {
        if (isResetting) return;
        isResetting = true;

        // 立即处理得分逻辑
        if (surface.CompareTag("PlayerCourt"))
        {
            UIController.Instance?.AddOpponentScore();
        }
        else if (surface.CompareTag("OpponentCourt"))
        {
            UIController.Instance?.AddPlayerScore();
        }
        else
        {
            HandleOutOfBounds();
        }
        
        StartCoroutine(DelayedReset());
    }

    private IEnumerator DelayedReset()
    {
        // 阶段1：立即冻结AI
        PlayerManager.Instance.StopAllAIPlayers();

        // 阶段2：等待期间保持物理模拟
        yield return new WaitForSeconds(3f);

        // 阶段3：执行重置
        rb.isKinematic = true;
        ResetBall();
        GameplayManager.Instance.ResetGameState();
        rb.isKinematic = false;
        
        isResetting = false;
    }

    public void ResetBall()
    {
        if (rb == null) 
        {
            rb = GetComponent<Rigidbody>();
            startPosition = transform.position;
        }
        
        if (rb != null)
        {
            rb.position = startPosition;
            rb.angularVelocity = Vector3.zero;
            if (!rb.isKinematic){
                rb.linearVelocity = Vector3.zero;
            }
        }
    }

    // 计算球的落点
    public Vector3 CalculateLandingPosition()
    {
        Vector3 velocity = rb.linearVelocity;
        Vector3 pos = transform.position;
        
        float g = Physics.gravity.magnitude;
        float y0 = pos.y;
        float discriminant = velocity.y * velocity.y + 2 * g * y0;
        
        if (discriminant < 0) return pos;

        float timeToLand = (velocity.y + Mathf.Sqrt(discriminant)) / g;
        Vector3 horizontalVel = new Vector3(velocity.x, 0, velocity.z);
        return pos + horizontalVel * timeToLand;
    }

    // 出界处理逻辑
    private void HandleOutOfBounds()
    {
        if (string.IsNullOrEmpty(lastTouchedTeam))
        {
            Debug.Log("Ball out with no last touch");
            return;
        }

        if (lastTouchedTeam == "Player")
        {
            UIController.Instance?.AddOpponentScore();
        }
        else if (lastTouchedTeam == "Opponent")
        {
            UIController.Instance?.AddPlayerScore();
        }

        // 重置触球记录
        lastTouchedTeam = "";
    }
} 