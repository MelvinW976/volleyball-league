using UnityEngine;

namespace PlayerStates
{
    public class ReceiveState : PlayerState
    {
        public override void Enter(PlayerController player)
        {
            // player.Animator.SetBool("IsSetting", false);
        }

        public override void Update(PlayerController player)
        {
            if (base.CanSet(player) && 
               (Input.GetKeyDown(KeyCode.K) || ShouldAutoSet(player)))
            {
                player.StateMachine.ChangeState(new SetState());
            }
        }

        private bool ShouldAutoSet(PlayerController player)
        {
            return player.IsAI && 
                   player.GetComponent<PlayerSet>().InSetRange();
        }

        public override void Exit(PlayerController player)
        {
            Debug.Log($"{player.name} 退出接发球状态");
        }
    }
} 