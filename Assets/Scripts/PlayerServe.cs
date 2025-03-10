using UnityEngine;
using System.Collections;
using PlayerStates;

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
    [SerializeField] private Transform ballHolder;

    [Header("Random Range")]
    [SerializeField] private UnityEngine.Vector2 horizontalRange = new (-0.2f, 0.2f);
    [SerializeField] private UnityEngine.Vector2 verticalRange = new (0.8f, 1.2f);
    [SerializeField] private UnityEngine.Vector2 forceVariationRange = new (0.9f, 1.1f);
    [SerializeField] private UnityEngine.Vector2 spinIntensityRange = new (0f, 1f);
    
    private Rigidbody ballRb;
    private float currentServeAngle;
    public bool InServeZone { get; private set; }
    public bool IsHoldingBall { get; private set; }

    void Update()
    {
        if (!InServeZone || ballRb == null) return;

        HandleBallPosition();
        HandleMovement();
        HandleServeAngle();
        TryPerformServe();
    }

    private void HandleBallPosition()
    {
        if (IsHoldingBall)
        {
            ballRb.MovePosition(ballHolder.position);
        }
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        Vector3 newPos = transform.position + new Vector3(moveX * Time.deltaTime * 5f, 0, 0);
        newPos.x = Mathf.Clamp(newPos.x, -lateralLimit, lateralLimit);
        transform.position = newPos;
    }

    private void HandleServeAngle()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            currentServeAngle = Mathf.Min(maxServeAngle, currentServeAngle + 1f);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            currentServeAngle = Mathf.Max(minServeAngle, currentServeAngle - 1f);
        }
    }

    private void TryPerformServe()
    {
        if (Input.GetKeyDown(KeyCode.Space) && CanServe())
        {
            PerformServe();
        }
    }

    public bool CanServe()
    {
        GameObject activePlayerObj = PlayerManager.Instance.ActivePlayer;
        if (activePlayerObj == null) return false;
        PlayerController activePlayer = activePlayerObj.GetComponent<PlayerController>();
        if (activePlayer?.StateMachine?.CurrentState == null) return false;
        return activePlayer.StateMachine.CurrentState.CanServe(activePlayer) &&
               InServeZone &&
               IsHoldingBall;
    }

    public void PrepareServe()
    {
        IsHoldingBall = true;
        ballRb.isKinematic = true;
        ballRb.position = ballHolder.position;
    }

    public void PerformServe()
    {
        IsHoldingBall = false;
        ballRb.isKinematic = false;
        
        Vector3 courtTarget = GameplayManager.Instance.GetCourtCenter(false);
        Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        Vector3 finalTarget = courtTarget + randomOffset;
        Vector3 forceVector = CalculatePassForce(ballSpawnPoint.position, finalTarget);
        ballRb.linearVelocity = forceVector;

        PlayerManager.Instance.EnablePlayerControl(true);
        PlayerManager.Instance.OnServeCompleted();
    }

    private Vector3 CalculatePassForce(Vector3 startPos, Vector3 targetPos)
    {
        float timeToTarget = 2f;
        Vector3 toTarget = targetPos - startPos;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z);

        float y = toTarget.y;
        float xz = toTargetXZ.magnitude;

        float velocityY = y / timeToTarget + 0.5f * Mathf.Abs(Physics.gravity.y) * timeToTarget;
        float velocityXZ = xz / timeToTarget;

        return toTargetXZ.normalized * velocityXZ + Vector3.up * velocityY;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ServeArea") && !GameplayManager.Instance.served)
        {
            InServeZone = true;
            InitializeBall();
            PlayerManager.Instance.EnablePlayerControl(false);
        }
    }

    private void InitializeBall()
    {
        ballRb = BallController.Instance.GetComponent<Rigidbody>();
        ballRb.transform.position = ballHolder.position;
        ballRb.linearVelocity = Vector3.zero;
        StartCoroutine(SetupServeBall());
    }

    private IEnumerator SetupServeBall()
    {
        yield return new WaitUntil(() => BallController.Instance != null);
        PrepareServe();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ServeArea"))
        {
            InServeZone = false;
        }
    }
} 