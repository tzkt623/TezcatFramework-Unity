namespace tezcat.Framework.Core
{
    /// <summary>
    /// 可成为原型的对象
    /// </summary>
    public interface ITezProtoObject : ITezSerializable
    {
        /// <summary>
        /// 生成对象
        /// </summary>
        ITezProtoObject spawnObject();
    }
}