using Unity.VisualScripting;
using UnityEngine;

public class BallController : MonoBehaviour
{
    // 移除原有的Y轴检测
    //[SerializeField] private float outOfBoundsY = -10f;

    private Rigidbody rb;
    private Vector3 startPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
    }

    // 使用碰撞检测替代Y轴位置判断
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && ! collision.gameObject.CompareTag("InnerCourt"))
        {
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
    }
} 