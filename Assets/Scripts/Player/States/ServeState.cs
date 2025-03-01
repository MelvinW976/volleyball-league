using UnityEngine;

namespace PlayerStates
{
    public class ServeState : PlayerState
    {
        public override void Enter(PlayerController player)
        {
            // 确保在服务区且持有球
            PlayerServe serveComponent = player.GetComponent<PlayerServe>();
            if (serveComponent != null && serveComponent.InServeZone)
            {
                Debug.Log($"{player.name} 进入发球状态");
                serveComponent.PrepareServe(); // 初始化发球参数
            }
            else
            {
                player.StateMachine.ChangeState(new IdleState());
            }
        }

        public override void Update(PlayerController player)
        {
            // 持续检查服务条件
            if (!CanServe(player))
            {
                player.StateMachine.ChangeState(new IdleState());
                return;
            }

            // 发球执行逻辑
            if (Input.GetKeyDown(KeyCode.Space))
            {
                player.GetComponent<PlayerServe>().PerformServe();
            }
        }

        public override void Exit(PlayerController player)
        {
            Debug.Log($"{player.name} 退出发球状态");
        }

        public override bool CanServe(PlayerController player)
        {
            // 在基类条件基础上增加状态特有条件
            return base.CanServe(player) && 
                   player.GetComponent<PlayerServe>().IsHoldingBall;
        }
    }
} 