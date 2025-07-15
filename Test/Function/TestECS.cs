using tezcat.Framework.Core;
using tezcat.Framework.ECS;
using tezcat.Framework.Game;

namespace tezcat.Framework.Test
{
    class MyData1
        : TezDataComponent
        , ITezCategoryObject
    {
        public int dbUID { get; private set; }
        public TezCategory category { get; private set; }

        /// <summary>
        /// 使用物品模板初始化对象
        /// </summary>
        public void initWithData(ITezSerializable item)
        {
//             var data_item = (ITezDBItemObject)item;
//             this.NID = data_item.NID;
//             this.dbUID = data_item.DBID;
//             this.category = data_item.category;

            this.preInit();
            this.onInitWithData(item);
            this.postInit();
        }

        protected virtual void onInitWithData(ITezSerializable item)
        {

        }

        public override void serialize(TezSaveController.Writer writer)
        {
            throw new System.NotImplementedException();
        }

        public override void deserialize(TezSaveController.Reader reader)
        {
            throw new System.NotImplementedException();
        }
    }

    class MyRenderer : TezRendererComponent
    {
        public override void reset() { }
        protected override void initObject() { }
        protected override void onHide() { }
        protected override void onShow() { }
        protected override void preInit() { }
    }

    class MyPhysics : ITezComponent
    {
        public static int SComUID;
        public int comUID => SComUID;

        public TezEntity entity { get; private set; }

        void ITezCloseable.closeThis()
        {
            this.entity = null;
        }

        public void onAdd(TezEntity entity)
        {
            this.entity = entity;
        }

        public void onOtherComponentAdded(ITezComponent component, int comID)
        {

        }

        public void onOtherComponentRemoved(ITezComponent component, int comID)
        {

        }

        public void onRemove(TezEntity entity)
        {

        }
    }

    class TestECS : TezBaseTest
    {
        public TestECS() : base("ECS-System")
        {
        }

        /// <summary>
        /// must first invoke
        /// </summary>
        public void registerComponent()
        {
            MyPhysics.SComUID = TezComponentManager.register<MyPhysics>();
        }

        /// <summary>
        /// and then
        /// </summary>
        public void create()
        {
            var entity = TezEntity.create();
            entity.addComponent(new MyData1());
            entity.addComponent(new MyRenderer());
            entity.addComponent(new MyPhysics());
        }

        public override void run()
        {

        }

        public override void init()
        {

        }

        protected override void onClose()
        {

        }
    }
}
