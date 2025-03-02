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
        
        if(playerScoreEffect != null)
        {
            playerScoreEffect.transform.position = GameplayManager.Instance.GetOpponentCourtPosition();
            playerScoreEffect.Play();
        }
        GameplayManager.Instance.winningTeam = "Player";
    }

    public void AddOpponentScore()
    {
        opponentScore++;
        UpdateScoreDisplay();
        
        if(opponentScoreEffect != null) 
        {
            opponentScoreEffect.transform.position = GameplayManager.Instance.GetPlayerCourtPosition();
            opponentScoreEffect.Play();
        }
        GameplayManager.Instance.winningTeam = "Opponent";
    }

    private void UpdateScoreDisplay()
    {
        playerScoreText.text = playerScore.ToString();
        opponentScoreText.text = opponentScore.ToString();
    }
} 