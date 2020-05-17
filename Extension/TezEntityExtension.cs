using tezcat.Framework.Database;
using tezcat.Framework.ECS;

namespace tezcat.Framework.Extension
{
    public static class TezEntityExtension
    {
        public static ObjectCom createObject<ObjectCom>(this TezDatabaseGameItem item) where ObjectCom : TezGameObject
        {
            var entity = item.createObject();
            var obj = entity.getComponent<TezDataObject, ObjectCom>();
            obj.buildDefinition();

            return obj;
        }

        /// <summary>
        /// 用Item数据创建一个
        /// </summary>
        public static ObjectCom create<ObjectCom>(TezDatabaseGameItem item) where ObjectCom : TezGameObject, new()
        {
            ObjectCom data = new ObjectCom();
            data.initWithData(item);

            var entity = TezEntity.create();
            entity.addComponent(data);

            return data;
        }

        /// <summary>
        /// 用Item数据创建一个 并且构建DefinitionPath
        /// </summary>
        public static ObjectCom createWithDefinitionPath<ObjectCom>(TezDatabaseGameItem item) where ObjectCom : TezGameObject, new()
        {
            ObjectCom data = new ObjectCom();
            data.initWithData(item);
            data.buildDefinition();

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
            data.buildDefinition();

            var entity = TezEntity.create();
            entity.addComponent(data);

            return data;
        }
    }
}