using tezcat.Framework.Core;

namespace tezcat.Framework.Database
{
    /// <summary>
    /// 非游戏物品类数据
    /// 比如文本,图片,声音等
    /// </summary>
    public abstract class TezDatabaseAssetItem
        : ITezCloseable
        , ITezSerializableItem
    {
        public string NID { get; set; }

        public void deserialize(TezReader reader)
        {
            this.onDeserialize(reader);
        }

        public void serialize(TezWriter writer)
        {
            this.onSerialize(writer);
        }

        protected virtual void onSerialize(TezWriter writer)
        {

        }

        protected virtual void onDeserialize(TezReader reader)
        {

        }

        public abstract void close();
    }
}