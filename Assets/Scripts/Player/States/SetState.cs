using UnityEngine;

namespace PlayerStates
{
    public class SetState : PlayerState
    {
        public override void Enter(PlayerController player)
        {
            // Logic for entering the set state
            Debug.Log($"{player.name} is now in Set State.");
        }

        public override void Update(PlayerController player)
        {
            // 直接调用现有Set组件
            if (player.GetComponent<PlayerSet>().IsSettingComplete)
            {
                // 直接返回接发球状态
                player.StateMachine.ChangeState(new ReceiveState());
            }
        }

        public override void Exit(PlayerController player)
        {
            // Logic for exiting the set state
            Debug.Log($"{player.name} has exited Set State.");
        }
    }
} 