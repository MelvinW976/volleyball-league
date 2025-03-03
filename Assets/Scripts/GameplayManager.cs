using UnityEngine;
using System.Collections;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    [Header("Court References")]
    [SerializeField] private GameObject playerCourt;
    [SerializeField] private GameObject opponentCourt;

    // 球权和触球计数
    public enum Possession { Neutral, Player, Opponent }
    public Possession CurrentPossession { get; set; } = Possession.Neutral;
    private int currentTouchCount = 0;

    public bool isResetting { get; set; } = false; // 新增重置状态标志

    // 分数变量
    [Header("比分")]
    public int playerScore = 0;
    public int opponentScore = 0;
    [SerializeField] private int scoreToWin = 25;
    [SerializeField] private int minPointDifference = 2;

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

    private IEnumerator DelayedReset()
    {
        // 阶段1：立即冻结AI
        PlayerManager.Instance.StopAllAIPlayers();

        // 阶段2：等待期间保持物理模拟
        yield return new WaitForSeconds(3f);

        // 阶段3：执行重置
        BallController.Instance.setKinematic(true);
        ResetGameState();
        BallController.Instance.setKinematic(false);
        isResetting = false;
    }
    
    // 处理触球事件
    public void HandleBallTouch(GameObject touchedPlayer)
    {
        string team = touchedPlayer.CompareTag("MyPlayer") ? "Player" : "Opponent";
        Possession newPossession = team == "Player" ? Possession.Player : Possession.Opponent;
        
        // 检查球权是否变化
        if (newPossession == CurrentPossession)
        {
            currentTouchCount++;
            
            // 检查是否超过三次触球
            if (currentTouchCount > 3)
            {
                // 违规：超过三次触球
                AwardPointToOpponent(newPossession);
                return;
            }
        }
        else
        {
            // 球权转换
            CurrentPossession = newPossession;
            currentTouchCount = 1;
        }
        
        // 通知PlayerManager更新控制
        PlayerManager.Instance.UpdateControlBasedOnPossession(CurrentPossession);
    }
    
    // 获取当前触球次数
    public int GetCurrentTouchCount() => currentTouchCount;
    
    // 在得分后重置球权
    private void ResetPossession()
    {
        CurrentPossession = Possession.Neutral;
        PlayerManager.Instance.UpdateControlBasedOnPossession(CurrentPossession);
        currentTouchCount = 0;
    }
    // 重置比赛状态
    public void ResetGameState()
    {
        // 重置球
        BallController ball = BallController.Instance;
        ResetPossession();
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

    // 判断当前是否可以执行特定操作
    public bool CanPass()
    {
        return currentTouchCount < 3;
    }
    
    // 判断是否可以垫球
    public bool CanSet() => currentTouchCount == 3;

    public string winningTeam;
    // 本回合是否发过球
    public bool served = false;

    public void GameplayReset(){
        StartCoroutine(DelayedReset());
    }

    private void AwardPointToOpponent(Possession currentPossession)
    {
        // 确定得分方
        Possession scoringTeam = (currentPossession == Possession.Player) ? 
                                 Possession.Opponent : Possession.Player;
        
        // 更新分数
        if (scoringTeam == Possession.Player)
        {
            // playerScore++;
            UIController.Instance.AddPlayerScore();
            Debug.Log($"玩家得分! 当前比分: {playerScore}-{opponentScore}");
        }
        else
        {
            // opponentScore++;
            UIController.Instance.AddOpponentScore();

            Debug.Log($"对手得分! 当前比分: {playerScore}-{opponentScore}");
        }
        
        // 更新UI
        // UpdateScoreUI();
        
        // 检查是否达到胜利条件
        if (CheckForVictory())
        {
            EndMatch();
            return;
        }
        
        // 重置回合
        StartCoroutine(DelayedReset());
    }

    // private void UpdateScoreUI()
    // {
    //     if (UIController.Instance != null)
    //     {
    //         UIController.Instance.UpdateScore(playerScore, opponentScore);
    //     }
    // }

    private bool CheckForVictory()
    {
        // 检查是否有一方达到胜利分数且领先至少2分
        if (playerScore >= scoreToWin && (playerScore - opponentScore) >= minPointDifference)
        {
            winningTeam = "Player";
            return true;
        }
        else if (opponentScore >= scoreToWin && (opponentScore - playerScore) >= minPointDifference)
        {
            winningTeam = "Opponent";
            return true;
        }
        
        return false;
    }

    private void EndMatch()
    {
        Debug.Log($"比赛结束! {winningTeam} 获胜!");
        
        // // 显示胜利UI
        // if (victoryUI != null)
        // {
        //     victoryUI.ShowVictory(winningTeam);
        // }
        
        // // 禁用所有玩家控制
        // PlayerManager.Instance.DisableAllPlayers();
        
        // // 可选：延迟几秒后返回主菜单
        // StartCoroutine(ReturnToMenuAfterDelay(5f));
    }

    private IEnumerator ReturnToMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 加载主菜单场景
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    // 添加调试信息
    public void LogTouchStatus()
    {
        Debug.Log($"当前球权: {CurrentPossession}, 触球次数: {currentTouchCount}");
    }
} 