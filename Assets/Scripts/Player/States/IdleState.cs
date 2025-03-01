using UnityEngine;

public class IdleState : PlayerState
{
    public override void Update(PlayerController player)
    {
        // if (BallController.Instance.lastTouchedPlayer == player.gameObject)
        // {
        //     player.StateMachine.ChangeState(new AttackState());
        // }
        // else if (Vector3.Distance(
        //     player.transform.position, 
        //     BallController.Instance.transform.position) < 2f)
        // {
        //     PlayerController lastTouchedPlayer = BallController.Instance.lastTouchedPlayer.GetComponent<PlayerController>();
        //     if (lastTouchedPlayer != null && lastTouchedPlayer.IsTeammate(player))
        //     {
        //         player.StateMachine.ChangeState(new PassState());
        //     }
        // }
    }
} 