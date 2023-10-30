namespace tezcat.Framework.BonusSystem
{
    public interface ITezBonusObjectHandler
    {
        void addBonusObject(ITezBonusCarrier carrier);
        void removeBonusObject(ITezBonusCarrier carrier);
    }

    /// <summary>
    /// 加成值携带对象
    /// 携带路径用于找到加成对象
    /// </summary>
    public interface ITezBonusCarrier
    {
        TezBonusPath bonusPath { get; }
    }

    /// <summary>
    /// 加成代理对象
    /// 携带路径用于标记自身位置
    /// </summary>
    public interface ITezBonusAgent
        : ITezBonusObjectHandler
    {
        TezBonusPath bonusPath { get; }
    }
}