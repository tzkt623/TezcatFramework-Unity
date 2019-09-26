using tezcat.Framework.Database;
using tezcat.Framework.ECS;

namespace tezcat.Framework.Extension
{
    public static class TezEntityExtension
    {
        public static ObjectCom createEntityWithData<ObjectCom>(TezDatabaseGameItem item, bool copy = false) where ObjectCom : TezGameObject, new()
        {
            ObjectCom data = new ObjectCom();
            data.initWithData(item, copy);

            var entity = TezEntity.create();
            entity.addComponent(data);

            return data;
        }

        public static ObjectCom createEntityWithNew<ObjectCom>() where ObjectCom : TezGameObject, new()
        {
            ObjectCom data = new ObjectCom();
            data.initNew();

            var entity = TezEntity.create();
            entity.addComponent(data);

            return data;
        }

    }
}