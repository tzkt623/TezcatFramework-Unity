using tezcat.Framework.Database;
using tezcat.Framework.ECS;

namespace tezcat.Framework.Extension
{
    public static class TezEntityExtension
    {
        /// <summary>
        /// 用Item数据创建一个
        /// </summary>
        public static ObjectCom create<ObjectCom>(TezDatabaseGameItem item, bool copy = false) where ObjectCom : TezGameObject, new()
        {
            ObjectCom data = new ObjectCom();
            data.initWithData(item, copy);

            var entity = TezEntity.create();
            entity.addComponent(data);

            return data;
        }

        /// <summary>
        /// 用Item数据创建一个 并且构建DefinitionPath
        /// </summary>
        public static ObjectCom createWithDefinitionPath<ObjectCom>(TezDatabaseGameItem item, bool copy = false) where ObjectCom : TezGameObject, new()
        {
            ObjectCom data = new ObjectCom();
            data.initWithData(item, copy);
            data.buildDefinitionPath();

            var entity = TezEntity.create();
            entity.addComponent(data);

            return data;
        }

        /// <summary>
        /// 创建一个新的
        /// </summary>
        public static ObjectCom create<ObjectCom>() where ObjectCom : TezGameObject, new()
        {
            ObjectCom data = new ObjectCom();
            data.initNew();

            var entity = TezEntity.create();
            entity.addComponent(data);

            return data;
        }

        /// <summary>
        /// 创建一个新的 并且构建DefinitionPath
        /// </summary>
        public static ObjectCom createWithDefinitionPath<ObjectCom>() where ObjectCom : TezGameObject, new()
        {
            ObjectCom data = new ObjectCom();
            data.initNew();
            data.buildDefinitionPath();

            var entity = TezEntity.create();
            entity.addComponent(data);

            return data;
        }
    }
}