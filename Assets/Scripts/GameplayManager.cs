using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    [Header("Court References")]
    [SerializeField] private GameObject playerCourt;
    [SerializeField] private GameObject opponentCourt;

    public string winningTeam;
    // 本回合是否发过球
    public bool served = false;

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
        // 重置球
        BallController ball = FindAnyObjectByType<BallController>();
        PlayerManager.Instance?.ResetPossession();
        if (ball != null) ball.ResetBall();

        // Reset players
        PlayerManager.Instance?.ResetAllPlayers();
        served = false;
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
        return courtCollider != null ? courtCollider.bounds.center : Vector3.zero;
    }

    public Vector3 GetRandomPositionInOpponentCourt(bool isPlayerSide)
    {
        GameObject targetCourt = isPlayerSide ? opponentCourt : playerCourt;
        Collider courtCollider = targetCourt.GetComponent<Collider>();
        Bounds bounds = courtCollider.bounds;

        return new Vector3(
            Random.Range(bounds.min.x + 2f, bounds.max.x - 2f),
            bounds.max.y,
            isPlayerSide ? bounds.min.z + 1f : bounds.max.z - 1f
        );
    }
} 