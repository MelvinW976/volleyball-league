using PlayerStates; // 新增引用

public abstract class PlayerState
{
    public virtual void Enter(PlayerController player) {}
    public virtual void Update(PlayerController player) {}
    public virtual void Exit(PlayerController player) {}

    public virtual bool CanServe(PlayerController player)
    {
        return !GameplayManager.Instance.served;
    }

    public virtual bool CanSet(PlayerController player)
    {
        return true;
    }
} 