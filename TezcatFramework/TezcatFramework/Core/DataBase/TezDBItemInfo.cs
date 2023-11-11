using tezcat.Framework.Core;

namespace tezcat.Framework.Database
{
    public abstract class TezDBItemInfo : ITezCloseable
    {
        public string path { get; }

        public TezDBItemInfo(string path)
        {
            this.path = path;
        }

        public abstract void close();
    }


    /*
     * 物品对象的元数据
     * 每一个由物品的生成的对象都会拥有一个此对象
     * 此对象包含一个物品数据库ID,一个运行时ID
     * 
     * 如果一个物品在运行时被修改,变得与数据库物品不同,那它将被赋予一个运行时ID
     * 一个共享物品在被复制之后会有一个引用计数来确定总共生成了多少个
     * 
     * 记录物品名称,是否可以堆叠,堆叠数量
     * 
     * 如果在游戏运行时生成了一个物品
     * 谁持有这个物品,谁负责管理他,并不会加入物品数据库
     * 
     * 例如
     * 无主之地爆了一地的装备
     * 这些装备都是根据模板数据实时生成的
     * 没有被玩家捡起来的装备应该在某一时刻销毁
     * 爆装备管理器应该负责这堆未拾取的装备的生命周期
     * 
     */

    public class TezGameItemInfo : TezDBItemInfo
    {
        class Ref
        {
            public int Count;
        }

        private Ref mRefCount = null;

        protected TezItemID mItemID = null;
        public TezItemID itemID => mItemID;

        public string NID { get; set; } = "$ErrorItem$";

        /*
         * #堆叠性质
         * 
         * 如果一个Item不可堆叠,在游戏运行时,每一个都是独立的Object
         * 例如EVE
         * 一艘飞船虽然数据都相同(同一艘船体),但是不同玩家,不同NPC操控的飞船,数据都是独立的
         * 一艘船受伤不会影响另一艘的数据
         * 
         * 如果一个Item可以堆叠,在游戏运行时,他是一个共享对象,必须是只读数据
         * 例如暗黑破坏神2
         * 血瓶不管有多少瓶,本质上都是共享一个数据
         * 不同大小的血瓶分别是不同的数据对象
         * 
         * 以上面的例子作为基础,规定
         * 1.所有可堆叠物品,在运行时都是共享对象,数据必须只读
         * 2.所有不可堆叠物品,在运行时都是独立对象,数据可读可写
         * 
         * ==0 : 初始化
         * ==-1 : 物品不可堆叠,物品生成的对象数据独立,必须克隆模板数据
         * >=1 : 物品可以堆叠,物品生成的对象数据共享,仅对模板数据只读
         */
        public int stackCount { get; set; } = 0;
        public bool isShared { get; }
        public TezCategory category => mPrototype.category;

        protected TezItemableObject mPrototype = null;

        /// <summary>
        /// 来源--运行时生成
        /// </summary>
        public TezGameItemInfo(string NID, int stackCount, int FDID, int MDID) : base(null)
        {
            this.NID = NID;
            this.stackCount = stackCount;
            mRefCount = new Ref()
            {
                Count = 1
            };

            //FDID来源于写死的数据库
            //MDID来源于运行时生成
            mItemID = TezItemID.create(FDID, MDID);
        }

        /// <summary>
        /// 来源--数据库信息
        /// </summary>
        public TezGameItemInfo(string path, string NID, int stackCount, int FDID) : base(path)
        {
            this.NID = NID;
            this.stackCount = stackCount;
            mRefCount = new Ref()
            {
                Count = 1
            };

            mItemID = TezItemID.create(FDID);
        }

        /// <summary>
        /// 来源--复制
        /// </summary>
        public TezGameItemInfo(TezGameItemInfo itemInfo)
            : base(itemInfo.path)
        {
            this.NID = itemInfo.NID;
            this.stackCount = itemInfo.stackCount;
            mRefCount = itemInfo.mRefCount;
            mRefCount.Count++;

            mItemID = itemInfo.mItemID.copy();
        }

        public bool isPrototype(TezItemableObject other)
        {
            return object.ReferenceEquals(mPrototype, other);
        }

        public override int GetHashCode()
        {
            return mItemID.GetHashCode();
        }

        public void setPrototype(TezItemableObject prototype)
        {
            mPrototype = prototype;
        }

        /// <summary>
        /// 获得当前数据所提供的对象
        /// 此模板数据
        /// 要么是克隆数据
        /// 要么是共享数据
        /// </summary>
        public TezItemableObject getObject()
        {
            return mPrototype.duplicate();
        }

        public T getObject<T>() where T : TezItemableObject
        {
            return (T)mPrototype.duplicate();
        }

        public void addRef()
        {
            mRefCount.Count++;
        }

        public TezGameItemInfo remodify()
        {
            var mdid = TezcatFramework.runtimeDB.generateID();

            var obj = new TezGameItemInfo(this.NID, this.stackCount, mItemID.fixedID, mdid);

            TezcatFramework.runtimeDB.registerItem(obj);

            return obj;
        }

        public static bool isError(TezGameItemInfo info)
        {
            return info.itemID.fixedID == -1;
        }

        public override void close()
        {
            if (mRefCount.Count-- > 0)
            {
                return;
            }

            if (mItemID.modifiedID > -1)
            {
                TezcatFramework.runtimeDB.unregisterItem(mItemID.modifiedID);
            }

            mItemID.close();
            mPrototype.close();
            mPrototype = null;
            this.NID = null;
            mRefCount = null;
        }

        public static TezGameItemInfo createFixedItemInfo(TezReader reader, string path)
        {
            string nid = reader.readString(TezBuildInName.NameID);
            if (!reader.tryRead(TezBuildInName.StackCount, out int stack_count))
            {
                stack_count = -1;
            }

            var fdid = reader.readInt(TezBuildInName.FixedID);

            return new TezGameItemInfo(path, nid, stack_count, fdid);
        }

        public static TezGameItemInfo createRuntimeItemInfo(TezReader reader)
        {
            string nid = reader.readString(TezBuildInName.NameID);
            if (!reader.tryRead(TezBuildInName.StackCount, out int stack_count))
            {
                stack_count = -1;
            }

            var fdid = reader.readInt(TezBuildInName.FixedID);
            var mdid = reader.readInt(TezBuildInName.ModifiedID);

            return new TezGameItemInfo(nid, stack_count, fdid, mdid);
        }
    }
}