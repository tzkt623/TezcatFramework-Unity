namespace tezcat.Framework.Core
{
    /// <summary>
    /// 可成为原型的对象
    /// </summary>
    public interface ITezProtoObject : ITezProtoLoader
    {
        TezProtoInfoWrapper protoInfo { get; }
        TezProtoObjectData baseProtoData { get; }
    }

    public interface ITezProtoObjectDataGetter<T> where T : TezProtoObjectData
    {
        T protoData { get; }
    }
}