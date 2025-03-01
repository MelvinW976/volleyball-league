using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerServe : MonoBehaviour
{
    [Header("Serve Settings")]
    [SerializeField] private float serveForce = 15f;
    [SerializeField] private float minServeAngle = 15f;
    [SerializeField] private float maxServeAngle = 45f;
    [SerializeField] private float lateralLimit = 3f;
    [SerializeField] private Transform ballSpawnPoint;
    
    private Rigidbody ballRb;
    private bool inServeZone;
    private float currentServeAngle;

    void Update()
    {
        if (!inServeZone || ballRb == null) return;

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PerformServe();
        }
    }

    private void PerformServe()
    {
        // 使用旋转后的方向计算实际目标点
        Vector3 serveDirection = Quaternion.Euler(currentServeAngle, transform.eulerAngles.y, 0) * Vector3.forward;
        Vector3 targetPos = transform.position + serveDirection * 10f; // 基础方向
        
        // 结合场地随机位置
        Vector3 courtTarget = GameplayManager.Instance.GetRandomPositionInOpponentCourt(true);
        Vector3 finalDirection = (courtTarget - transform.position).normalized;
        
        // 计算合成方向（50% 基础方向 + 50% 场地随机方向）
        Vector3 blendedDirection = (serveDirection * 0.5f + finalDirection * 0.5f).normalized;
        Vector3 finalTarget = transform.position + blendedDirection * Vector3.Distance(transform.position, courtTarget);

        // 应用物理计算
        Vector3 forceVector = CalculateServeForce(ballSpawnPoint.position, finalTarget);
        ballRb.linearVelocity = forceVector;
        
        // 触发发球完成事件
        GameplayManager.Instance.OnServeCompleted();
    }

    private Vector3 CalculateServeForce(Vector3 startPos, Vector3 targetPos)
    {
        Vector3 direction = (targetPos - startPos).normalized;
        float gravity = Physics.gravity.magnitude;
        float angle = Vector3.Angle(Vector3.forward, direction) * Mathf.Deg2Rad;
        
        float distance = Vector3.Distance(startPos, targetPos);
        float velocity = Mathf.Sqrt(distance * gravity / Mathf.Sin(2 * angle));
        
        return velocity * direction;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ServeArea"))
        {
            inServeZone = true;
            Debug.Log("进入发球区");
        }
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