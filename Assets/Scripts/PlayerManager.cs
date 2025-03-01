using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [Header("Players")]
    public List<GameObject> players;
    private int currentPlayerIndex = 0;

    private GameObject _activePlayer;
    public GameObject ActivePlayer => _activePlayer;  // Public getter for activePlayer

    public enum Possession { Neutral, Player, Opponent }
    public Possession CurrentPossession { get; private set; } = Possession.Neutral;

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
        if (curPlayer == null || !curPlayer.CompareTag("MyPlayer")) 
        {
            Debug.LogError("SetActivePlayer: Invalid player selection!");
            return;
        }
        
        _activePlayer = curPlayer;
        // 只启用己方玩家控制
        _activePlayer.GetComponent<PlayerMovement>().enabled = true;
        _activePlayer.GetComponent<PlayerPass>().enabled = true;
        
        // 禁用其他玩家控制
        foreach (GameObject player in players.Where(p => p != _activePlayer)) 
        {
            player.GetComponent<PlayerMovement>().enabled = false;
            player.GetComponent<PlayerPass>().enabled = false;
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
        Debug.Log($"Current Possession: {CurrentPossession}");
        if (Input.GetKeyDown(KeyCode.Tab)) {
            SwitchPlayer();
        }
    }

    private void SwitchPlayer()
    {
        // 只在自己队伍玩家中切换
        var teamPlayers = players.Where(p => p.CompareTag("MyPlayer")).ToList();
        if (teamPlayers.Count == 0) return;
        
        currentPlayerIndex = (currentPlayerIndex + 1) % teamPlayers.Count;
        SetActivePlayer(teamPlayers[currentPlayerIndex]);
    }
    private void SwitchPossession()
    {
        CurrentPossession = CurrentPossession == Possession.Player ? Possession.Opponent : Possession.Player;
        UpdateControl();
    }
    public void OnPassCompleted()
    {
        SwitchPossession();
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
        }
        ResetActivePlayer(); // 保持原有激活玩家重置
    }

    public void EnablePlayerControl(bool enable)
    {
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerMovement>().enabled = enable;
            player.GetComponent<PlayerPass>().enabled = enable;
        }
    }

    public void OnServeCompleted()
    {
        // 设置球权并更新控制
        CurrentPossession = Possession.Opponent;
        UpdateControl();
        
        // 延迟切换玩家
        StartCoroutine(SwitchPlayerNextFrame());
    }

    private IEnumerator SwitchPlayerNextFrame()
    {
        yield return null;
        SwitchPlayer();
    }

    // 当球被触碰时调用（在BallController中）
    public void OnBallTouched(string team)
    {
        CurrentPossession = team == "Player" ? Possession.Opponent : Possession.Player;
        UpdateControl();
    }

    private void UpdateControl()
    {
        bool playerControl = CurrentPossession == Possession.Player;
        bool opponentControl = CurrentPossession == Possession.Opponent;

        // 控制玩家
        foreach (GameObject player in players.Where(p => p.CompareTag("MyPlayer"))) 
        {
            // player.GetComponent<PlayerMovement>().enabled = playerControl;
            player.GetComponent<PlayerPass>().enabled = playerControl;
        }

        // 控制AI
        foreach (GameObject opponent in players.Where(p => p.CompareTag("MyOpponent"))) 
        {
            opponent.GetComponent<AIPlayerMovement>().canMove = opponentControl;
        }
    }

    // 在得分后重置球权
    public void ResetPossession()
    {
        CurrentPossession = Possession.Neutral;
        UpdateControl();
    }
}
