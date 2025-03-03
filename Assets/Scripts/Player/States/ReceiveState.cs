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
            // 获取当前触球次数
            int touchCount = GameplayManager.Instance?.GetCurrentTouchCount() ?? 0;
            
            // 根据触球次数决定可用操作
            if (touchCount < 3)
            {
                // 前两次触球允许传球
                if (Input.GetKeyDown(KeyCode.K) && base.CanPass(player))
                {
                    player.StateMachine.ChangeState(new PassState());
                }
            }
            else if (touchCount == 3)
            {
                // 第三次触球只允许垫球
                if (Input.GetKeyDown(KeyCode.J) && base.CanSet(player))
                {
                    player.StateMachine.ChangeState(new SetState());
                }
                
                // 如果按了传球键，提示玩家
                if (Input.GetKeyDown(KeyCode.K))
                {
                    Debug.Log("第三次触球必须垫球! 请按J键垫球");
                }
            }
        }

        private bool ShouldAutoSet(PlayerController player)
        {
            return player.IsAI && 
                   player.GetComponent<PlayerSet>().InSetRange();
        }

        public override void Exit(PlayerController player)
        {
        }
    }
} 