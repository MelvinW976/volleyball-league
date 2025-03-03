using UnityEngine;

[CreateAssetMenu(fileName = "NewTactics", menuName = "Volleyball/Tactics")]
public class TacticsData : ScriptableObject
{
    [Header("触球规则")]
    public int maxTouchesPerPossession = 3;
    public bool allowSamePlayerConsecutiveTouch = false;
    
    [Header("阵型设置")]
    public Vector3[] defaultPositions = new Vector3[2]; // 默认站位
    public float defensiveSpread = 2.5f; // 防守分散度
    
    [Header("进攻参数")]
    [Range(0.5f, 3f)] public float passHeight = 1.5f;
    [Range(0.5f, 2f)] public float passForceMultiplier = 1f;
    
    [Header("AI行为")]
    [Range(0f, 1f)] public float aggressiveness = 0.5f; // 影响AI冒险决策
    [Range(0f, 1f)] public float passFrequency = 0.7f; // 传球频率
    public bool prioritizeTeammate = true; // 是否优先传给队友
}

public class TacticsManager : MonoBehaviour
{
    public static TacticsManager Instance { get; private set; }
    
    [SerializeField] private TacticsData currentTactics;
    
    // 战术预设
    [SerializeField] private TacticsData defensiveTactics;
    [SerializeField] private TacticsData offensiveTactics;
    [SerializeField] private TacticsData balancedTactics;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        if (currentTactics == null)
            currentTactics = balancedTactics;
    }
    
    public void SwitchTactics(TacticsType type)
    {
        switch (type)
        {
            case TacticsType.Defensive:
                currentTactics = defensiveTactics;
                break;
            case TacticsType.Offensive:
                currentTactics = offensiveTactics;
                break;
            default:
                currentTactics = balancedTactics;
                break;
        }
        
        // 通知其他系统战术已更改
        OnTacticsChanged();
    }
    
    private void OnTacticsChanged()
    {
        // 更新GameplayManager
        // GameplayManager.Instance.UpdateRules(currentTactics.maxTouchesPerPossession);
        
        // // 更新AI行为
        // UpdateAIBehavior();
        
        // // 更新玩家参数
        // UpdatePlayerParameters();
    }
    
    // 获取当前战术参数
    public int GetMaxTouches() => currentTactics.maxTouchesPerPossession;
    public float GetPassHeight() => currentTactics.passHeight;
    public float GetPassForce() => currentTactics.passForceMultiplier;
    public bool ShouldPrioritizeTeammate() => currentTactics.prioritizeTeammate;
    
    // 其他方法...
}

public enum TacticsType
{
    Defensive,
    Balanced,
    Offensive
} 