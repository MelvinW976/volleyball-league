using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    [Header("Score Displays")]
    public TMP_Text playerScoreText;
    public TMP_Text opponentScoreText;

    [Header("Effects")]
    public ParticleSystem playerScoreEffect;
    public ParticleSystem opponentScoreEffect;

    private int playerScore = 0;
    private int opponentScore = 0;

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

    public void AddPlayerScore()
    {
        playerScore++;
        UpdateScoreDisplay();
        
        // 在对方场地播放特效
        if(playerScoreEffect != null)
        {
            playerScoreEffect.transform.position = GetOpponentCourtPosition();
            playerScoreEffect.Play();
        }
        ResetGameState();
    }

    public void AddOpponentScore()
    {
        opponentScore++;
        UpdateScoreDisplay();
        
        // 在我方场地播放特效
        if(opponentScoreEffect != null) 
        {
            opponentScoreEffect.transform.position = GetPlayerCourtPosition();
            opponentScoreEffect.Play();
        }
        ResetGameState();
    }

    private void UpdateScoreDisplay()
    {
        playerScoreText.text = playerScore.ToString();
        opponentScoreText.text = opponentScore.ToString();
    }

    private void ResetGameState()
    {
        // Reset ball and player positions
        BallController ball = FindAnyObjectByType<BallController>();
        if (ball != null) ball.ResetBall();

        // Reset active player
        PlayerManager.Instance?.ResetActivePlayer();
    }

    private Vector3 GetPlayerCourtPosition()
    {
        return GameObject.FindGameObjectWithTag("PlayerCourt").transform.position + Vector3.up * 2f;
    }

    private Vector3 GetOpponentCourtPosition()
    {
        return GameObject.FindGameObjectWithTag("OpponentCourt").transform.position + Vector3.up * 2f;
    }
} 