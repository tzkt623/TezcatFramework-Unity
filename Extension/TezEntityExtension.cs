using tezcat.Framework.Database;
using tezcat.Framework.ECS;

namespace tezcat.Framework.Extension
{
    public static class TezEntityExtension
    {
        /// <summary>
        /// 用Item数据创建一个
        /// </summary>
        public static ObjectCom create<ObjectCom>(this TezDatabaseGameItem item) where ObjectCom : TezComData
        {
            var entity = item.createObject();
            return entity.getComponent<TezComBaseData, ObjectCom>();
        }

        /// <summary>
        /// 创建一个新的
        /// </summary>
        public static ObjectCom create<ObjectCom>() where ObjectCom : TezComData, new()
        {
            ObjectCom data = new ObjectCom();
            data.initNew();

            var entity = TezEntity.create();
            entity.addComponent(data);

            return data;
        }
    }
}