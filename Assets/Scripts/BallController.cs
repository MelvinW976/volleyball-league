using Unity.VisualScripting;
using UnityEngine;

public class BallController : MonoBehaviour
{
    // 移除原有的Y轴检测
    //[SerializeField] private float outOfBoundsY = -10f;

    private Rigidbody rb;
    private Vector3 startPosition;
    private GameObject lastTouchedPlayer;
    private string lastTouchedTeam = "";

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
    }

    // 使用碰撞检测替代Y轴位置判断
    private void OnCollisionEnter(Collision collision)
    {
        // 检测玩家触球
        if (collision.gameObject.CompareTag("MyPlayer"))
        {
            lastTouchedPlayer = collision.gameObject;
            lastTouchedTeam = "Player"; // 根据实际队伍标签设置
        }
        else if (collision.gameObject.CompareTag("MyOpponent"))
        {
            lastTouchedPlayer = collision.gameObject;
            lastTouchedTeam = "Opponent";
        }

        // 场地得分逻辑
        if (collision.gameObject.CompareTag("PlayerCourt"))
        {
            Debug.Log("Ball in player court");
            UIController.Instance?.AddOpponentScore();
            ResetBall();
        }
        else if (collision.gameObject.CompareTag("OpponentCourt"))
        {
            Debug.Log("Ball in opponent court");
            UIController.Instance?.AddPlayerScore();
            ResetBall();
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Ball out of bounds");
            HandleOutOfBounds();
            ResetBall();
        }
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
        
        // 重置当前激活玩家
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.ResetActivePlayer();
        }
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
        lastTouchedPlayer = null;
        lastTouchedTeam = "";
    }
} 