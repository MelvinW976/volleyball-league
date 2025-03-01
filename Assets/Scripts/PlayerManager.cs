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
    public void OnPassCompleted()
    {
        SwitchPlayer();
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
        // 延迟一帧切换，确保物理计算完成
        StartCoroutine(SwitchPlayerNextFrame());
    }

    private IEnumerator SwitchPlayerNextFrame()
    {
        yield return null;
        SwitchPlayer();
    }

}
