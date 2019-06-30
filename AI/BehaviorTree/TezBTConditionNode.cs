namespace tezcat.Framework.AI
{
    public abstract class TezBTConditionNode<Data>
        : TezBTNode<Data>
        where Data : ITezBTData
    {
        public sealed override TezBTNodeType nodeType => TezBTNodeType.Condition;
    }
}