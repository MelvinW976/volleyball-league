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

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
    }

    // 使用碰撞检测替代Y轴位置判断
    private void OnCollisionEnter(Collision collision)
    {
        // Court scoring logic
        if (collision.gameObject.CompareTag("PlayerCourt"))
        {
            UIController.Instance?.AddOpponentScore();
            ResetBall();
        }
        else if (collision.gameObject.CompareTag("OpponentCourt"))
        {
            UIController.Instance?.AddPlayerScore();
            ResetBall();
        }
        else if (collision.gameObject.CompareTag("Ground") && !isResetting)
        {
            Debug.Log("球已落地，3秒后重置");
            isResetting = true;
            StartCoroutine(DelayedReset());
        }
    }

    private IEnumerator DelayedReset()
    {
        // 禁用球物理
        rb.isKinematic = true;
        
        // 等待3秒
        yield return new WaitForSeconds(3f);
        
        // 执行重置
        ResetBall();
        GameplayManager.Instance.ResetGameState();
        
        // 恢复物理
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
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
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