using UnityEngine;
using UnityEngine.AI;

public class AIPlayerMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 targetPosition;
    [SerializeField] private float stoppingDistance = 0.5f;
    private bool canPass = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        targetPosition = transform.position; // Initial position
    }

    void Update()
    {
        // Continuously update target position
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            agent.SetDestination(targetPosition);
        }
        else if (canPass) // Auto pass when reaching target
        {
            TryPassBall();
        }
    }

    public void SetTargetPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
    }

    // Add trigger detection
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            canPass = true;
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
    private void TryPassBall()
    {
        PlayerPass passComponent = GetComponent<PlayerPass>();
        if (passComponent != null)
        {
            passComponent.PerformPass();
            canPass = false; // Reset pass state
        }
    }
}
