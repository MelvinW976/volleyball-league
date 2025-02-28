using UnityEngine;
using UnityEngine.AI;

public class AIPlayerMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 targetPosition;
    [SerializeField] private float stoppingDistance = .5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        targetPosition = transform.position; // 初始位置
    }

    void Update()
    {
        // 持续更新目标位置
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)

        {
            agent.SetDestination(targetPosition);
        }
    }

    public void SetTargetPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
    }
}
