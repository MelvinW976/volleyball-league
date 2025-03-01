using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    [Header("Court References")]
    [SerializeField] private GameObject playerCourt;
    [SerializeField] private GameObject opponentCourt;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetGameState()
    {
        // Reset ball
        BallController ball = FindAnyObjectByType<BallController>();
        if (ball != null) ball.ResetBall();

        // Reset players
        PlayerManager.Instance?.ResetAllPlayers();
    }

    public Vector3 GetPlayerCourtPosition()
    {
        return playerCourt.transform.position + Vector3.up * 2f;
    }

    public Vector3 GetOpponentCourtPosition()
    {
        return opponentCourt.transform.position + Vector3.up * 2f;
    }

    // 新增场地有效性检查
    public bool IsPositionInCourt(Vector3 position, bool isPlayerCourt)
    {
        GameObject targetCourt = isPlayerCourt ? playerCourt : opponentCourt;
        if (targetCourt == null) return false;

        Collider courtCollider = targetCourt.GetComponent<Collider>();
        return courtCollider != null && courtCollider.bounds.Contains(position);
    }

    public Vector3 GetCourtCenter(bool isPlayerCourt)
    {
        GameObject targetCourt = isPlayerCourt ? playerCourt : opponentCourt;
        if (targetCourt == null) return Vector3.zero;

        Collider courtCollider = targetCourt.GetComponent<Collider>();
        if (courtCollider == null) return Vector3.zero;

        return courtCollider.bounds.center;
    }
} 