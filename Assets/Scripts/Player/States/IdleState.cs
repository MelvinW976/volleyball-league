using UnityEngine;

namespace PlayerStates
{
    public class IdleState : PlayerState
    {
        public override void Enter(PlayerController player)
        {
            // 移除了Animator相关代码
        }

        public override void Update(PlayerController player)
        {
            // 检查输入以转换到其他状态
            if (Input.GetKeyDown(KeyCode.Space) && base.CanServe(player)) // 空格键发球
            {
                player.StateMachine.ChangeState(new ServeState());
            }
            else if (Input.GetKeyDown(KeyCode.K) && base.CanSet(player)) // K键设置
            {
                player.StateMachine.ChangeState(new SetState());
            }
        }

        public override void Exit(PlayerController player)
        {
            // 保持空实现以维持结构
        }
    }
}
