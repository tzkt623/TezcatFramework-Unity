namespace tezcat.Framework.BonusSystem
{
    public interface ITezBonusObjectHandler
    {
        void addBonusObject(ITezBonusObject obj);
        void removeBonusObject(ITezBonusObject obj);
    }

    public interface ITezBonusObject
    {
        TezBonusPath bonusPath { get; }
    }

    public interface ITezBonusAgent
        : ITezBonusObject
        , ITezBonusObjectHandler
    {

    }
}