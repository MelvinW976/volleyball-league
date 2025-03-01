using UnityEngine;

public class PassState : PlayerState
{
    private PlayerPass passComponent;
    private float passDelay = 0.5f;
    private float timer = 0f;

    public override void Enter(PlayerController player)
    {
        passComponent = player.GetComponent<PlayerPass>();
        if (passComponent == null)
        {
            Debug.LogError("PassState: No PlayerPass component found!");
            return;
        }
        timer = 0f;
    }

    public override void Update(PlayerController player)
    {
        timer += Time.deltaTime;
        
        if (timer >= passDelay)
        {
            // Execute pass
            passComponent.PerformPass();
            // Return to idle state
            player.StateMachine.ChangeState(new IdleState());
        }
    }

    public override void Exit(PlayerController player)
    {
        passComponent = null;
    }
} 