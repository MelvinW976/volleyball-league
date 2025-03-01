using UnityEngine;

public abstract class PlayerState
{
    public virtual void Enter(PlayerController player) {}
    public virtual void Update(PlayerController player) {}
    public virtual void Exit(PlayerController player) {}
}

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerController player;
    private PlayerState currentState;

    void Awake() => player = GetComponent<PlayerController>();

    void Update() => currentState?.Update(player);

    public void ChangeState(PlayerState newState)
    {
        currentState?.Exit(player);
        currentState = newState;
        currentState?.Enter(player);
    }
} 