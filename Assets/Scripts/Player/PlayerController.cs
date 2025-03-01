using UnityEngine;

[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerController : MonoBehaviour
{
    // 公共属性
    public bool IsAI { get; set; }
    // public PlayerRole Role { get; set; } // 可定义枚举: Setter,Spiker,Libero等
    
    // 组件引用
    [HideInInspector] public PlayerStateMachine StateMachine;
    [HideInInspector] public Rigidbody Rb;
    
    // 状态参数
    public float MoveSpeed = 5f;
    public float PassForce = 8f;

    void Awake()
    {
        StateMachine = GetComponent<PlayerStateMachine>();
        Rb = GetComponent<Rigidbody>();
    }

    // 判断是否为队友
    public bool IsTeammate(PlayerController otherPlayer)
    {
        return otherPlayer != this && CompareTag(otherPlayer.tag); // Use CompareTag for efficiency
    }
} 