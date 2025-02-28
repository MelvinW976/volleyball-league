using UnityEngine;

public class PlayerPass : MonoBehaviour
{
    public Rigidbody ballRb;                // Reference to the ball's Rigidbody
    public Transform passTarget;             // The teammate or direction to pass to
    private readonly float passRadius = 1f;
    private readonly float timeToTarget = 2f;
    private bool canPass = false;
    private PlayerManager playerManager;

    [Header("Landing Indicator")]
    public CircleRenderer circleRenderer;    // 替换原来的 landingIndicator 和 currentIndicator

    [Header("AI Player Control")]
    [SerializeField] private AIPlayerMovement aiPlayer; 

    void Start(){
        playerManager = PlayerManager.Instance;
        if (playerManager == null) {
            Debug.LogError("PlayerManager instance not found!");
        }
    }

    void Update()
    {
        // Check for pass input
        if (canPass && Input.GetKeyDown(KeyCode.K)) {
            PerformPass();
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
        Vector2 randomPoint = Random.insideUnitCircle * passRadius;
        Vector3 passPoint = targetPoint + new Vector3(randomPoint.x, randomPoint.y, 0.0f);
        return passPoint;
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

    public void PerformPass()
    {
        Vector3 startPoint = ballRb.position;
        Vector3 endPoint = passTarget.position;
        Vector3 initialVelocity = CalculateVelocity(startPoint, endPoint);

        // Apply an upward and forward force to the ball
        ballRb.linearVelocity = Vector3.zero; // Reset the ball's velocity
        ballRb.angularVelocity = Vector3.zero; // Reset the ball's rotation
        ballRb.AddForce(initialVelocity, ForceMode.VelocityChange);
        Debug.Log("Passed the ball!");
        canPass = false; // Prevent multiple passes until the ball re-enters the trigger
        playerManager.OnPassCompleted();

        // 添加AI目标更新
        if(aiPlayer != null)
        {
            aiPlayer.SetTargetPosition(endPoint); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the ball
        if (other.gameObject.CompareTag("Ball"))
        {
            canPass = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // When the ball exits the trigger, disallow passing
        if (other.gameObject.CompareTag("Ball"))
        {
            canPass = false;
        }
    }

    private void ShowLandingIndicator()
    {
        if (circleRenderer == null || ballRb == null) return;
        
        // 计算球的当前运动轨迹落点
        Vector3 velocity = ballRb.linearVelocity;
        Vector3 position = ballRb.position;
        
        // 忽略垂直速度很小的计算
        if (Mathf.Abs(velocity.y) < 0.1f && position.y < 0.5f) 
        {
            HideLandingIndicator();
            return;
        }

        // 使用抛物线运动公式计算落地时间
        float g = Physics.gravity.magnitude;
        float y0 = position.y;
        float discriminant = velocity.y * velocity.y + 2 * g * y0;
        
        if (discriminant < 0) 
        {
            // 无实数解时（球不会落地）不显示
            HideLandingIndicator();
            return;
        }

        float timeToLand = (velocity.y + Mathf.Sqrt(discriminant)) / g;
        
        // 计算水平位移
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        Vector3 landingPos = position + horizontalVelocity * timeToLand;
        
        // 确保落点在地面上
        landingPos.y = 0;
        
        // 显示指示器
        circleRenderer.ShowCircle(landingPos, 0.5f); // 0.5m半径的精确落点
    }

    private void HideLandingIndicator()
    {
        if (circleRenderer != null) 
        {
            circleRenderer.HideCircle();
        }
    }
}
