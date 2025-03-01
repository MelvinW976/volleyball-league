using UnityEngine;
using UnityEngine.AI;

public class AIPlayerMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 targetPosition;
    [SerializeField] private float passBallRadius = 2f;
    private bool canPass = false;
    public bool canMove = false;

    void Start()
    {
        canMove = false;
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 0.5f; // Stop threshold
        agent.autoBraking = true;      // Enable auto-brake
        agent.acceleration = 8f;       // Movement smoothness
        agent.speed = 5f;
        targetPosition = transform.position; // Initial position
    }

    void Update()
    {
        Debug.Log("canMove: " + canMove);
        if (!canMove) return;

        // 获取球的预测落点
        Vector3 ballLandingPos = GetBallLandingPosition();
        
        // 检查落点是否在对方场地（XZ平面）
        // bool isLandingInOpponentCourt = GameplayManager.Instance.IsPositionInCourt(ballLandingPos, false);
        // if (!isLandingInOpponentCourt) 
        // {
        //     Debug.Log("落点不在对手场地");
        //     agent.isStopped = true;
        //     return;
        // }

        // 如果AI球员在场上 并且AI球员与球的距离大于1米，则移动AI球员到球的落点
        if (Vector3.Distance(ballLandingPos, transform.position) < 1f){
            agent.isStopped = true;
            if (canPass) TrySetBall();
        }
        else if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
            Vector3 finalPos = ballLandingPos;
            finalPos.y = 0; 
            agent.SetDestination(finalPos);
        }
    }

    public void SetTargetPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
    }

    // Add trigger detection
    private void OnTriggerStay(Collider other)
    {
        // 持续检测球是否在触发范围内
        if (other.CompareTag("Ball"))
        {
            // 添加距离校验
            float distanceToBall = Vector3.Distance(transform.position, other.transform.position);
            canPass = distanceToBall < passBallRadius;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            canPass = false;
        }
    }

    // Add pass method
    private void TrySetBall()
    {
        // 停止移动并执行垫球
        PlayerSet setComponent = GetComponent<PlayerSet>();
        if (setComponent != null)
        {
            setComponent.PerformSet();
            canPass = false; // Reset set state
        }
    }

    private Vector3 GetBallLandingPosition()
    {
        BallController ball = FindAnyObjectByType<BallController>();
        if (ball != null) 
        {
            return ball.CalculateLandingPosition();
        }
        return transform.position; // Fallback to own position
    }
}
