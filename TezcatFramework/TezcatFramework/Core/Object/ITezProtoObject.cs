namespace tezcat.Framework.Core
{
    /// <summary>
    /// 可成为原型的对象
    /// </summary>
    public interface ITezProtoObject : ITezSerializable
    {
        TezProtoItemInfo itemInfo { get; }
        TezProtoObjectData basicProtoData { get; }
    }

    public interface ITezProtoObject<T> where T : TezProtoObjectData
    {
        T protoData { get; }
    }
}