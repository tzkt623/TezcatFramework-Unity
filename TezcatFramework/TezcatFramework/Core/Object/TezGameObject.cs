using tezcat.Framework.Database;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// 基础游戏对象
    /// </summary>
    public abstract class TezGameObject
        : TezBaseObject
    {
        //         protected TezString<TezGameObject> mNID = TezString<TezGameObject>.copyFrom();
        //         public TezString<TezGameObject> NID => mNID;



        protected TezCategory mCategory = null;
        public TezCategory category => mCategory;

        /// <summary>
        /// 生成默认对象
        /// </summary>
        public void init()
        {
            this.preInit();
            this.initDefault();
            this.postInit();
        }

        /// <summary>
        /// 在初始化之前做点什么
        /// </summary>
        protected virtual void preInit()
        {

        }

        /// <summary>
        /// 初始化之后做点什么
        /// </summary>
        protected virtual void postInit()
        {

        }

        /// <summary>
        /// 生成默认对象
        /// </summary>
        protected virtual void initDefault()
        {

        }

        public override void deserialize(TezReader reader)
        {
            mCategory = TezCategorySystem.getCategory(reader.readString(TezReadOnlyString.Category));
        }

        public override void serialize(TezWriter writer)
        {
            writer.write(TezReadOnlyString.CID, this.CID);
            writer.write(TezReadOnlyString.Category, mCategory.name);
        }

        public override void close()
        {
            base.close();
            //             mNID.close();
            // 
            //             mNID = null;
            mCategory = null;
        }


        /*
        #region Override
        public override init GetHashCode()
        {
            return mObjectUID.GetHashCode();
        }

        /// <summary>
        /// 比较的是UID而不是内存地址
        /// </summary>
        public override bool Equals(object other)
        {
            return this.Equals((TezGameObject)other);
        }

        /// <summary>
        /// 比较的是UID而不是内存地址
        /// </summary>
        public bool Equals(TezGameObject other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return mObjectUID == other.mObjectUID;
        }

        /// <summary>
        /// 比较的是UID而不是内存地址
        /// </summary>
        public static bool operator ==(TezGameObject a, TezGameObject b)
        {
            if (object.ReferenceEquals(a, null))
            {
                return object.ReferenceEquals(b, null);
            }

            return a.Equals(b);
        }

        /// <summary>
        /// 比较的是UID而不是内存地址
        /// </summary>
        public static bool operator !=(TezGameObject a, TezGameObject b)
        {
            return !(a == b);
        }
        #endregion
        */
    }
}