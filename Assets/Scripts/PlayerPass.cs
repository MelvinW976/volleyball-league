using UnityEngine;

public class PlayerPass : MonoBehaviour
{
    public Rigidbody ballRb;                // Reference to the ball's Rigidbody
    public Transform passTarget; // The teammate or direction to pass to
    private float passRadius = 1f;

    private bool canPass = false;           // Flag to check if the ball is in range
    private float timeToTarget = 2f;
    private PlayerManager playerManager;

    void Start(){
        playerManager = PlayerManager.Instance;
        if (playerManager == null) {
            Debug.LogError("PlayerManager instance not found!");
        }
    }

    [System.Obsolete]
    void Update()
    {
        // Check for pass input
        if (canPass && Input.GetKeyDown(KeyCode.K)) {
            PerformPass();
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


    [System.Obsolete]
    private void PerformPass()
    {
        Vector3 startPoint = ballRb.position;
        Vector3 endPoint = passTarget.position;
        Vector3 initialVelocity = CalculateVelocity(startPoint, endPoint);


        // Apply an upward and forward force to the ball
        ballRb.velocity = Vector3.zero; // Reset the ball's velocity
        ballRb.angularVelocity = Vector3.zero; // Reset the ball's rotation
        ballRb.AddForce(initialVelocity, ForceMode.VelocityChange);
        Debug.Log("Passed the ball!");
        canPass = false; // Prevent multiple passes until the ball re-enters the trigger
        playerManager.OnPassCompleted();
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
}
