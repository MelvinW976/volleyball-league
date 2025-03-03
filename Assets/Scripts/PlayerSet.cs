using UnityEngine;

public class PlayerSet : MonoBehaviour
{
    public Rigidbody ballRb;                // Reference to the ball's Rigidbody
    public Transform setTarget;             // The teammate or direction to set to
    private readonly float setRadius = 5f;
    private readonly float timeToTarget = 2f;
    private bool canSet = false;
    private PlayerManager playerManager;
    private BallController currentBall;

    [Header("Landing Indicator")]
    public CircleRenderer circleRenderer;    // Replace the original landingIndicator and currentIndicator
    
    [Header("AI Player Control")]
    [SerializeField] private float passBallRadius = 2f;

    // Add this property to track setting completion
    public bool IsSettingComplete { get; private set; } = false;

    [SerializeField] private float airDrag = 0.1f;
    [SerializeField] private float spinForce = 1f;

    void Start()
    {
        playerManager = PlayerManager.Instance;
        if (playerManager == null) {
            Debug.LogError("PlayerManager instance not found!");
        }
    }

    void Update()
    {
        // Check for set input
        if (canSet && Input.GetKeyDown(KeyCode.K)) {
            PerformSet();
        }
        if (ballRb != null && ballRb.linearVelocity.sqrMagnitude > 0.1f) 
        {
            ShowLandingIndicator();
        }
        else 
        {
            HideLandingIndicator();
        }
    }

    Vector3 AddRandomness(Vector3 targetPoint)
    {
        Vector2 randomPoint = Random.insideUnitCircle * setRadius;
        Vector3 setPoint = targetPoint + new Vector3(randomPoint.x, randomPoint.y, 0.0f);
        return setPoint;
    }

    Vector3 CalculateVelocity(Vector3 startPoint, Vector3 endPoint)
    {
        Vector3 toTarget = AddRandomness(endPoint - startPoint);
        Vector3 toTargetXZ = toTarget;
        toTargetXZ.y = 0;

        // Calculate the distance
        float y = toTarget.y;
        float xz = toTargetXZ.magnitude;

        // Calculate initial velocities
        float velocityY = y / timeToTarget + 0.5f * Mathf.Abs(Physics.gravity.y) * timeToTarget;
        float velocityXZ = xz / timeToTarget;

        Vector3 result = toTargetXZ.normalized; // Direction
        result *= velocityXZ; // Multiply by velocity
        result.y = velocityY;

        return result;
    }

    public void PerformSet()
    {
        // 检查是否可以垫球
        if (GameplayManager.Instance != null && !GameplayManager.Instance.CanSet())
        {
            Debug.Log("未达垫球次数要求，不能垫球");
            return;
        }
        
        // 使用原始逻辑获取球引用
        if (ballRb == null)
        {
            ballRb = GameObject.FindGameObjectWithTag("Ball")?.GetComponent<Rigidbody>();
            if (ballRb == null)
            {
                Debug.LogError("无法找到球的Rigidbody");
                return;
            }
        }
        

        
        bool isPlayer = gameObject.CompareTag("MyPlayer");
        // 使用原始的速度计算逻辑
        Vector3 startPoint = ballRb.position;
        Vector3 targetPosition = GameplayManager.Instance.GetCourtCenter(!isPlayer);
        Vector3 initialVelocity = CalculateVelocity(startPoint, targetPosition);
        
        // 重置球的当前速度，避免叠加效果
        ballRb.linearVelocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
        
        // 应用力 - 使用原始的力度和方向
        ballRb.AddForce(initialVelocity, ForceMode.Impulse);
        
        // 添加旋转力 - 如果原始代码有这部分
        if (spinForce > 0)
        {
            Vector3 spinAxis = Vector3.Cross(Vector3.up, initialVelocity.normalized);
            ballRb.AddTorque(spinAxis * spinForce, ForceMode.Impulse);
        }
        
        // 设置空气阻力 - 如果原始代码有这部分
        ballRb.linearDamping = airDrag;
        
        Debug.Log($"{gameObject.name} 执行垫球，目标: {targetPosition}, 初速度: {initialVelocity}");
        
        // 更新球的最后触碰信息
        if (BallController.Instance != null)
        {
            BallController.Instance.lastTouchedTeam = gameObject.CompareTag("MyPlayer") ? "Player" : "Opponent";
            BallController.Instance.lastTouchedPlayer = gameObject;
        }
        
        // 通知GameplayManager处理触球
        if (GameplayManager.Instance != null)
        {
            GameplayManager.Instance.HandleBallTouch(gameObject);
        }
        
        // 更新状态机
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null && controller.StateMachine != null)
        {
            controller.StateMachine.ChangeState(new PlayerStates.SetState());
        }
        
        // 标记垫球完成
        IsSettingComplete = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the ball
        if (other.gameObject.CompareTag("Ball"))
        {
            canSet = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // When the ball exits the trigger, disallow setting
        if (other.gameObject.CompareTag("Ball"))
        {
            canSet = false;
        }
    }

    private void ShowLandingIndicator()
    {
        if (circleRenderer == null || ballRb == null) return;
        
        // Calculate the landing point of the ball's current trajectory
        Vector3 velocity = ballRb.linearVelocity;
        Vector3 position = ballRb.position;
        
        // Ignore calculation when vertical velocity is too small
        if (Mathf.Abs(velocity.y) < 0.1f && position.y < 0.5f) 
        {
            HideLandingIndicator();
            return;
        }

        // Calculate landing time using projectile motion formula
        float g = Physics.gravity.magnitude;
        float y0 = position.y;
        float discriminant = velocity.y * velocity.y + 2 * g * y0;
        
        if (discriminant < 0) 
        {
            // Hide when no real solution exists (ball won't land)
            HideLandingIndicator();
            return;
        }

        float timeToLand = (velocity.y + Mathf.Sqrt(discriminant)) / g;
        
        // Calculate horizontal displacement
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        Vector3 landingPos = position + horizontalVelocity * timeToLand;
        
        // Ensure landing point is on the ground
        landingPos.y = 0;
        
        // Show indicator
        circleRenderer.ShowCircle(landingPos, 0.5f); // Precise landing point with 0.5m radius
    }

    private void HideLandingIndicator()
    {
        if (circleRenderer != null) 
        {
            circleRenderer.HideCircle();
        }
    }

    public bool InSetRange()
    {
        if (currentBall == null) return false;
        return Vector3.Distance(transform.position, currentBall.transform.position) < passBallRadius;
    }
} 