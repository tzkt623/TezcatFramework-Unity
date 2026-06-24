namespace tezcat.Framework.Core
{
    /// <summary>
    /// 可成为原型的对象
    /// </summary>
    public interface ITezProtoObject
    {
        TezProtoInfoWrapper protoInfo { get; }
        TezProtoObjectData baseProtoData { get; }
        uint protoObjectUID { get; }
        bool isTheSameProtoObjectOf(ITezProtoObject other);
        bool isTheSameProtoDataOf(ITezProtoObject other);
    }

    public interface ITezProtoObjectDataGetter<T> where T : TezProtoObjectData
    {
        T protoData { get; }
    }
}