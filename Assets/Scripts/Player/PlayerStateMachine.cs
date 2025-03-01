using UnityEngine;
using PlayerStates;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerController player;
    private PlayerState currentState;

    public PlayerState CurrentState => currentState;

    void Awake() => player = GetComponent<PlayerController>();

    void Start()
    {
        // 初始化默认状态
        ChangeState(new IdleState());
    }

    void Update() => currentState?.Update(player);

    public void ChangeState(PlayerState newState)
    {
        currentState?.Exit(player);
        currentState = newState;
        currentState?.Enter(player);
    }
} 