using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [Header("Players")]
    public List<GameObject> players;
    private int currentPlayerIndex = 0;

    private GameObject activePlayer;

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

    public void SetActivePlayer(GameObject curPlayer)
    {
        if (curPlayer == null){
            Debug.LogError("SetActivePlayer: player is null!");
            return;
        }
        activePlayer = curPlayer;
        activePlayer.GetComponent<PlayerMovement>().enabled = true;
        activePlayer.GetComponent<PlayerPass>().enabled = true;
        foreach (GameObject player in players) {
            if (player!=activePlayer){
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
                    outlinerMat.SetFloat("_Scale", (player == activePlayer) ? 1.12f : 0f);
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

}
