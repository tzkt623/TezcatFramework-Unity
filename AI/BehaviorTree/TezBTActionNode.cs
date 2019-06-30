namespace tezcat.Framework.AI
{
    public abstract class TezBTActionNode<Data>
        : TezBTNode<Data>
        where Data : ITezBTData
    {
        public sealed override TezBTNodeType nodeType => TezBTNodeType.Action;
    }
}