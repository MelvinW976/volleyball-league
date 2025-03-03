namespace PlayerStates {
    public class PassState : PlayerState
    {
        private PlayerPass passController;
        private bool hasTriggeredPass;
        
        public override void Enter(PlayerController player)
        {
            base.Enter(player);
            passController = player.GetComponent<PlayerPass>();
            // player.Animator.SetTrigger("Pass");
            hasTriggeredPass = false;
        }

        public override void Update(PlayerController player)
        {
            // if (!hasTriggeredPass && GetNormalizedTime(player.Animator) >= 0.3f)
            // {
            //     passController.PerformPass();
            //     hasTriggeredPass = true;
            // }

            // if (GetNormalizedTime(player.Animator) >= 1f)
            // {
            //     player.StateMachine.ChangeState(new ReceiveState());
            // }
        }

        public override void Exit(PlayerController player)
        {
            // player.Animator.ResetTrigger("Pass");
        }
    }
}