using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Animations;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [Header("Players")]
    public List<GameObject> players;
    private int currentPlayerIndex = 0;

    private GameObject _activePlayer;
    public GameObject ActivePlayer => _activePlayer;  // Public getter for activePlayer

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }
    private void ResetActivePlayer()
    {
        currentPlayerIndex = 0;
        SetActivePlayer(players[currentPlayerIndex]);
    }

    public void SetActivePlayer(GameObject curPlayer)
    {
        if (curPlayer == null) 
        {
            Debug.LogError("SetActivePlayer: Invalid player selection!");
            return;
        }
        
        _activePlayer = curPlayer;
        // 只启用己方玩家控制
        _activePlayer.GetComponent<PlayerMovement>().enabled = true;
        _activePlayer.GetComponent<PlayerSet>().enabled = true;
        
        // 禁用其他玩家控制
        foreach (GameObject player in players.Where(p => p != _activePlayer)) 
        {
            // 禁用AI球员控制
            if (player.GetComponent<AIPlayerMovement>() != null){
                player.GetComponent<PlayerMovement>().enabled = false;
                player.GetComponent<PlayerSet>().enabled = false;
            }
        }
    
        HighlightActivePlayer();
    }

    private void HighlightActivePlayer()
    {
        foreach (GameObject player in players){
            MeshRenderer mr = player.GetComponent<MeshRenderer>();
            if (mr != null && mr.materials.Length > 1){
                Material outlinerMat = mr.materials[1];
                if (outlinerMat.HasProperty("_Scale")){
                    outlinerMat.SetFloat("_Scale", (player == _activePlayer) ? 1.12f : 0f);
                }
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        SetActivePlayer(players[currentPlayerIndex]);
    }

    void Update() {
    }

    private void SwitchPlayer(string tagName)
    {
        var teamPlayers = players.Where(p => p.CompareTag(tagName)).ToList();
        if (teamPlayers.Count == 0) return;
        
        currentPlayerIndex = (currentPlayerIndex + 1) % teamPlayers.Count;
        SetActivePlayer(teamPlayers[currentPlayerIndex]);
        Debug.Log(teamPlayers[currentPlayerIndex].name + " is now active");
    }

    public void OnPassCompleted()
    {
        SwitchPlayer(ActivePlayer.CompareTag("MyPlayer") ? "MyPlayer": "MyOpponent");
        // SwitchAllToReceiveState(false); // 不重置位置
    }

    public void ResetAllPlayers()
    {
        foreach (GameObject player in players)
        {
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.ResetToInitialPosition();
            }
            if (player.GetComponent<AIPlayerMovement>() != null){
                player.GetComponent<AIPlayerMovement>().ResetAI();
            }
        }
        ResetActivePlayer(); // 保持原有激活玩家重置
    }

    public void EnablePlayerControl(bool enable)
    {
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerMovement>().enabled = enable;
            player.GetComponent<PlayerSet>().enabled = enable;
        }
    }

    public void OnServeCompleted()
    {
        GameplayManager.Instance.served = true;

        // 设置球权并更新控制
        GameplayManager.Instance.CurrentPossession = GameplayManager.Possession.Opponent;
        UpdateControlBasedOnPossession(GameplayManager.Instance.CurrentPossession);
        SwitchAllToReceiveState(resetPosition: false);
        SwitchPlayerToOtherSide();
    }

    private void SwitchPlayerToOtherSide(){
        var tagName = ActivePlayer.CompareTag("MyPlayer") ? "MyOpponent" : "MyPlayer";
        SwitchPlayer(tagName);
    }

    public void UpdateControlBasedOnPossession(GameplayManager.Possession possession)
    {
        bool playerControl = possession == GameplayManager.Possession.Player;
        bool opponentControl = possession == GameplayManager.Possession.Opponent;
        
        // 控制玩家
        foreach (GameObject player in players.Where(p => p.CompareTag("MyPlayer"))) 
        {
            player.GetComponent<PlayerPass>().enabled = playerControl;
            player.GetComponent<PlayerSet>().enabled = playerControl;
        }
        
        // 控制AI
        foreach (GameObject opponent in players.Where(p => p.CompareTag("MyOpponent"))) 
        {
            opponent.GetComponent<AIPlayerMovement>().canMove = opponentControl;
        }
    }

    public void OnSetCompleted()
    {
        GameplayManager.Instance.CurrentPossession = GameplayManager.Possession.Opponent;
        UpdateControlBasedOnPossession(GameplayManager.Instance.CurrentPossession);
        SwitchAllToReceiveState(false); // 不重置位置
    }

    public void StopAllAIPlayers()
    {
        foreach (var player in players)
        {
            AIPlayerMovement ai = player.GetComponent<AIPlayerMovement>();
            if (ai != null){
                ai.EmergencyStop(); // 新增紧急停止方法
            }
        }
    }

    public void SwitchAllToReceiveState(bool resetPosition = true)
    {
        foreach (var player in players)
        {
            // 添加安全校验
            if (player == null || player.GetComponent<PlayerController>() == null) continue;
            
            // 添加位置重置
            if (resetPosition) 
            {
                player.GetComponent<PlayerMovement>().ResetToInitialPosition();
            }
            
            // 所有玩家状态转换
            player.GetComponent<PlayerController>().StateMachine.ChangeState(new PlayerStates.ReceiveState());
        }
        Debug.Log($"已切换{players.Count}名玩家到接发球状态");
    }
}
