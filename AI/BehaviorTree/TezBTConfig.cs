namespace tezcat.Framework.AI
{
    public enum TezBTResult : byte
    {
        Empty = 0,
        Fail,
        Success,
        Running,
    }

    public enum TezBTNodeType : byte
    {
        Action = 0,
        Condition,
        Parallel,
        Selector,
        Sequence
    }
}