using UnityEngine;

public class PlayerSet : MonoBehaviour
{
    public Rigidbody ballRb;                // Reference to the ball's Rigidbody
    public Transform setTarget;             // The teammate or direction to set to
    private readonly float setRadius = 5f;
    private readonly float timeToTarget = 2f;
    private bool canSet = false;
    private PlayerManager playerManager;

    [Header("Landing Indicator")]
    public CircleRenderer circleRenderer;    // Replace the original landingIndicator and currentIndicator

    [Header("AI Player Control")]
    [SerializeField] private AIPlayerMovement aiPlayer; 

    void Start()
    {
        playerManager = PlayerManager.Instance;
        if (playerManager == null) {
            Debug.LogError("PlayerManager instance not found!");
        }
    }

    void Update()
    {
        // Check for set input
        if (canSet && Input.GetKeyDown(KeyCode.K)) {
            PerformSet();
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
        Vector2 randomPoint = Random.insideUnitCircle * setRadius;
        Vector3 setPoint = targetPoint + new Vector3(randomPoint.x, randomPoint.y, 0.0f);
        return setPoint;
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

    public void PerformSet()
    {
        Vector3 startPoint = ballRb.position;
        Vector3 endPoint = setTarget.position;
        
        // Validate landing position
        endPoint = AdjustLandingPosition(endPoint);

        Vector3 initialVelocity = CalculateVelocity(startPoint, endPoint);

        // Apply an upward and forward force to the ball
        ballRb.linearVelocity = Vector3.zero; // Reset the ball's velocity
        ballRb.angularVelocity = Vector3.zero; // Reset the ball's rotation
        ballRb.AddForce(initialVelocity, ForceMode.VelocityChange);
        Debug.Log(playerManager.ActivePlayer.name + " set the ball!");
        canSet = false; // Prevent multiple sets until the ball re-enters the trigger
        BallController.Instance.lastTouchedTeam = playerManager.ActivePlayer.CompareTag("MyPlayer") ? "Player" : "Opponent";
        BallController.Instance.lastTouchedPlayer = gameObject;
        playerManager.OnSetCompleted(); // You may want to rename this method as well

        // Update AI target position
        if(aiPlayer != null)
        {
            aiPlayer.SetTargetPosition(endPoint); 
        }
    }

    private Vector3 AdjustLandingPosition(Vector3 originalPos)
    {
        bool isPlayer = gameObject.CompareTag("MyPlayer");
        return GameplayManager.Instance.IsPositionInCourt(originalPos, !isPlayer) ? 
               originalPos : 
               GameplayManager.Instance.GetCourtCenter(!isPlayer);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the ball
        if (other.gameObject.CompareTag("Ball"))
        {
            canSet = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // When the ball exits the trigger, disallow setting
        if (other.gameObject.CompareTag("Ball"))
        {
            canSet = false;
        }
    }

    private void ShowLandingIndicator()
    {
        if (circleRenderer == null || ballRb == null) return;
        
        // Calculate the landing point of the ball's current trajectory
        Vector3 velocity = ballRb.linearVelocity;
        Vector3 position = ballRb.position;
        
        // Ignore calculation when vertical velocity is too small
        if (Mathf.Abs(velocity.y) < 0.1f && position.y < 0.5f) 
        {
            HideLandingIndicator();
            return;
        }

        // Calculate landing time using projectile motion formula
        float g = Physics.gravity.magnitude;
        float y0 = position.y;
        float discriminant = velocity.y * velocity.y + 2 * g * y0;
        
        if (discriminant < 0) 
        {
            // Hide when no real solution exists (ball won't land)
            HideLandingIndicator();
            return;
        }

        float timeToLand = (velocity.y + Mathf.Sqrt(discriminant)) / g;
        
        // Calculate horizontal displacement
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        Vector3 landingPos = position + horizontalVelocity * timeToLand;
        
        // Ensure landing point is on the ground
        landingPos.y = 0;
        
        // Show indicator
        circleRenderer.ShowCircle(landingPos, 0.5f); // Precise landing point with 0.5m radius
    }

    private void HideLandingIndicator()
    {
        if (circleRenderer != null) 
        {
            circleRenderer.HideCircle();
        }
    }
} 