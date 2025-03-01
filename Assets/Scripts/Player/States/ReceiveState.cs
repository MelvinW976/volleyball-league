using UnityEngine;

public class ReceiveState : PlayerState
{
    public override void Enter(PlayerController player)
    {
        player.Rb.linearVelocity = Vector3.zero;
        player.GetComponent<Animator>().SetTrigger("Ready");
    }

    public override void Update(PlayerController player)
    {
        if (player.IsAI)
        {
            // AI自动移动到接球位置
            Vector3 targetPos = BallController.Instance.CalculateLandingPosition();
            player.transform.position = Vector3.MoveTowards(
                player.transform.position, 
                targetPos, 
                player.MoveSpeed * Time.deltaTime
            );
        }
        else
        {
            // 玩家输入控制
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            player.Rb.linearVelocity = new Vector3(h, 0, v) * player.MoveSpeed;
        }
    }
} 