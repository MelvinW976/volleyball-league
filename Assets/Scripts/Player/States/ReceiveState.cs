using UnityEngine;

namespace PlayerStates
{
    public class ReceiveState : PlayerState
    {
        public override void Enter(PlayerController player)
        {
            // 接发球准备逻辑
            Debug.Log($"{player.name} 进入接发球状态");
        }

        public override void Update(PlayerController player)
        {
            // 持续移动控制
            HandleMovement(player);
            
            // 接球判定
            if (IsReceivingBall(player))
            {
                HandleReceiveAction(player);
            }
        }

        private void HandleMovement(PlayerController player)
        {
            // 自由移动实现（示例）
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveX, 0, moveZ) * player.MoveSpeed;
            player.Rb.linearVelocity = movement;
        }

        private bool IsReceivingBall(PlayerController player)
        {
            // 接球条件判断
            return Vector3.Distance(player.transform.position, 
                                  BallController.Instance.transform.position) < 2f;
        }

        private void HandleReceiveAction(PlayerController player)
        {
            // 接球后的状态转换
            player.StateMachine.ChangeState(new IdleState());
        }

        public override void Exit(PlayerController player)
        {
            Debug.Log($"{player.name} 退出接发球状态");
        }
    }
} 