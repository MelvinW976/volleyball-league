using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;

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
    public void ResetActivePlayer()
    {
        currentPlayerIndex = 0;
        SetActivePlayer(players[currentPlayerIndex]);
    }

    public void SetActivePlayer(GameObject curPlayer)
    {
        if (curPlayer == null){
            Debug.LogError("SetActivePlayer: player is null!");
            return;
        }
        _activePlayer = curPlayer;
        _activePlayer.GetComponent<PlayerMovement>().enabled = true;
        _activePlayer.GetComponent<PlayerPass>().enabled = true;
        foreach (GameObject player in players) {
            if (player != _activePlayer){
                player.GetComponent<PlayerMovement>().enabled = false;
                player.GetComponent<PlayerPass>().enabled = false;
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
        if (Input.GetKeyDown(KeyCode.Tab)) {
            SwitchPlayer();
        }
    }

    private void SwitchPlayer() {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        SetActivePlayer(players[currentPlayerIndex]);
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

}
