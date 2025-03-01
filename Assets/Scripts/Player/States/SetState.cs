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
            // Logic for updating the set state
            // You can add any specific behavior for setting here
        }

        public override void Exit(PlayerController player)
        {
            // Logic for exiting the set state
            Debug.Log($"{player.name} has exited Set State.");
        }
    }
} 