using System;
using tezcat.Framework.Core;

namespace tezcat.Framework.ECS
{
    public abstract class TezDataComponent
        : TezBaseComponent
        , IEquatable<TezDataComponent>
    {
        /// <summary>
        /// 注册ID
        /// </summary>
        public static int SComUID;
        public sealed override int comUID => SComUID;


        private uint m_ObjectUID = 0;

        /// <summary>
        /// UID
        /// 为0则表示没有分配
        /// </summary>
        public uint objectUID => m_ObjectUID;


        /// <summary>
        /// 唯一名称ID
        /// </summary>
        public string NID { get; protected set; }

        /// <summary>
        /// 初始化Object
        /// </summary>
        public void initNew()
        {
            this.preInit();
            this.onInitNew();
            this.postInit();
        }

        /// <summary>
        /// 新建一个白板对象
        /// 不依赖数据模板
        /// </summary>
        protected virtual void onInitNew()
        {

        }

        /// <summary>
        /// 数据初始化之前
        /// </summary>
        protected virtual void preInit()
        {
            m_ObjectUID = TezObjectUID.generateID();
        }

        /// <summary>
        /// 数据初始化之后
        /// </summary>
        protected virtual void postInit()
        {

        }

        /// <summary>
        /// 删除Object
        /// </summary>
        public override void close()
        {
            TezObjectUID.recycleID(m_ObjectUID);
            this.NID = null;
        }

        #region Override
        public override int GetHashCode()
        {
            return m_ObjectUID.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return this.Equals((TezDataComponent)other);
        }

        public bool Equals(TezDataComponent other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return m_ObjectUID == other.m_ObjectUID;
        }

        public static bool operator ==(TezDataComponent a, TezDataComponent b)
        {
            if (object.ReferenceEquals(a, null))
            {
                return object.ReferenceEquals(b, null);
            }

            return a.Equals(b);
        }

        public static bool operator !=(TezDataComponent a, TezDataComponent b)
        {
            return !(a == b);
        }
        #endregion
    }
}