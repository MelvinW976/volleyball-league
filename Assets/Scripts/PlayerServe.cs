using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerServe : MonoBehaviour
{
    [Header("Serve Settings")]
    [SerializeField] private float serveForce = 15f;
    [SerializeField] private float minServeAngle = 15f;
    [SerializeField] private float maxServeAngle = 45f;
    [SerializeField] private float lateralLimit = 3f;
    [SerializeField] private Transform ballSpawnPoint;
    [Header("Ball Handling")]
    [SerializeField] private Transform ballHolder; // 在玩家头顶的空对象
    
    private Rigidbody ballRb;
    private bool inServeZone;
    private float currentServeAngle;
    private bool isHoldingBall = false;

    void Update()
    {
        if (!inServeZone || ballRb == null) return; // 检查是否可以发球

        if (isHoldingBall && ballRb != null)
        {
            // 球跟随玩家左右移动
            ballRb.MovePosition(ballHolder.position);
        }

        // 左右移动限制
        float moveX = Input.GetAxis("Horizontal");
        Vector3 newPos = transform.position + new Vector3(moveX * Time.deltaTime * 5f, 0, 0);
        newPos.x = Mathf.Clamp(newPos.x, -lateralLimit, lateralLimit);
        transform.position = newPos;

        // 发球角度调整
        if (Input.GetKey(KeyCode.UpArrow))
        {
            currentServeAngle = Mathf.Min(maxServeAngle, currentServeAngle + 1f);
            Debug.Log("currentServeAngle: " + currentServeAngle);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            currentServeAngle = Mathf.Max(minServeAngle, currentServeAngle - 1f);
            Debug.Log("currentServeAngle: " + currentServeAngle);
        }

        // 执行发球
        if (!GameplayManager.Instance.served && Input.GetKeyDown(KeyCode.Space))
        {
            PerformServe();
        }
    }

    private void PerformServe()
    {
        isHoldingBall = false;
        ballRb.isKinematic = false;
        
        // 获取对方场地中心点
        Vector3 courtTarget = GameplayManager.Instance.GetCourtCenter(false);
        
        // 使用传球算法
        Vector3 forceVector = CalculatePassForce(ballSpawnPoint.position, courtTarget);
        ballRb.linearVelocity = forceVector;

        PlayerManager.Instance.EnablePlayerControl(true);
        PlayerManager.Instance.OnServeCompleted();
    }

    private Vector3 CalculatePassForce(Vector3 startPos, Vector3 targetPos)
    {
        float timeToTarget = 1.5f; // 发球时间
        Vector3 toTarget = targetPos - startPos;
        Vector3 toTargetXZ = toTarget;
        toTargetXZ.y = 0;

        float y = toTarget.y;
        float xz = toTargetXZ.magnitude;

        float velocityY = y / timeToTarget + 0.5f * Mathf.Abs(Physics.gravity.y) * timeToTarget;
        float velocityXZ = xz / timeToTarget;

        Vector3 result = toTargetXZ.normalized * velocityXZ;
        result.y = velocityY;
        
        return result;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ServeArea") && !GameplayManager.Instance.served)
        {
            inServeZone = true;
            Debug.Log("进入发球区");
            // 禁用玩家自由移动
            PlayerManager.Instance.EnablePlayerControl(false);

            // 通过单例获取球实例
            ballRb = BallController.Instance.GetComponent<Rigidbody>();
            
            // 重置球的位置到玩家头顶
            ballRb.transform.position = ballHolder.position;
            ballRb.linearVelocity = Vector3.zero;
            ballRb.isKinematic = true; // 新增关键设置
            
            StartCoroutine(SetupServeBall());
        }
    }

    private IEnumerator SetupServeBall()
    {
        // 等待球复位
        yield return new WaitUntil(() => BallController.Instance != null);
        
        ballRb = BallController.Instance.GetComponent<Rigidbody>();
        ballRb.isKinematic = true; // 禁用物理
        ballRb.position = ballHolder.position;
        isHoldingBall = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ServeArea"))
        {
            inServeZone = false;
            Debug.Log("离开发球区");
        }
    }

} 