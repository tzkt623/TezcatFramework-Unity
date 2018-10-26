using System;
using System.Collections.Generic;
using tezcat.Framework.DataBase;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// 基础Object
    /// </summary>
    public abstract class TezObject
        : ITezCloseable
    {
        public string CID
        {
            get { return this.GetType().Name; }
        }

        /// <summary>
        /// 删除Object时调用
        /// </summary>
        public abstract void close();


        #region 重载操作
        public static bool operator true(TezObject obj)
        {
            return !object.ReferenceEquals(obj, null);
        }

        public static bool operator false(TezObject obj)
        {
            return object.ReferenceEquals(obj, null);
        }

        public static bool operator !(TezObject obj)
        {
            return object.ReferenceEquals(obj, null);
        }
        #endregion
    }

    /// <summary>
    /// 游戏Object
    /// </summary>
    public abstract class TezGameObject
         : TezObject
         , ITezSerializable
         , ITezTagSet
    {
        #region GUID Manager
        static Queue<uint> m_FreeID = new Queue<uint>();
        static uint m_IDGiver = 1;
        static uint giveID()
        {
            if (m_FreeID.Count > 0)
            {
                return m_FreeID.Dequeue();
            }

            return m_IDGiver++;
        }

        static void recycleID(uint id)
        {
            m_FreeID.Enqueue(id);
        }
        #endregion

        /// <summary>
        /// RID
        /// </summary>
        protected TezRID m_RID = null;

        /// <summary>
        /// 全局唯一ID
        /// </summary>
        public uint GUID { get; private set; }

        /// <summary>
        /// 类型分组
        /// </summary>
        public abstract ITezGroup group { get; }

        /// <summary>
        /// 类型次级分组
        /// </summary>
        public abstract ITezSubGroup subgroup { get; }

        /// <summary>
        /// 唯一名称ID
        /// </summary>
        public string NID { get; private set; } = null;

        /// <summary>
        /// 标签
        /// </summary>
        public TezTagSet TAG { get; private set; } = null;

        /// <summary>
        /// 初始化Object
        /// </summary>
        public void initNew()
        {
            if (this.m_RID == null)
            {
                if (this.GUID == 0)
                {
                    this.GUID = giveID();
                }

                this.m_RID = new TezRID(group, subgroup);
                this.NID = string.Empty;
                this.TAG = new TezTagSet();
                this.onInitNew();
            }
            else
            {
                throw new ArgumentException(string.Format("{0} >> This Object is Init Again", this.GetType().Name));
            }
        }

        protected virtual void onInitNew()
        {

        }

        public void initWithData(TezDataBaseGameItem item)
        {
            if(this.GUID == 0)
            {
                this.GUID = giveID();
            }

            this.m_RID?.close();
            this.m_RID = new TezRID(item.RID);

            this.NID = item.NID;
            this.TAG = new TezTagSet();
            this.onInitWithData(item);
        }

        protected virtual void onInitWithData(TezDataBaseGameItem item)
        {

        }

        public bool sameAs(TezGameObject game_object)
        {
            return this.m_RID.sameAs(game_object.m_RID);
        }

        /// <summary>
        /// 更新当前对象的资源ID
        /// </summary>
        public void updateRID()
        {
            this.m_RID?.close();

            this.m_RID = new TezRID(group, subgroup);
            this.m_RID.updateID();
        }

        /// <summary>
        /// 删除Object
        /// </summary>
        public override void close()
        {
            this.TAG.close();
            this.TAG = null;

            this.NID = null;

            this.m_RID?.close();
            this.m_RID = null;

            recycleID(this.GUID);
            this.GUID = 0;
        }

        public virtual void serialize(TezSaveManager manager)
        {
            manager.write(TezReadOnlyString.Database.CID, this.CID);
            manager.write(TezReadOnlyString.Database.NID, this.NID);
        }

        public virtual void deserialize(TezSaveManager manager)
        {
            this.NID = manager.readString(TezReadOnlyString.Database.NID);
        }
    }

    /// <summary>
    /// 工具Object
    /// </summary>
    public abstract class TezToolObject : TezObject
    {

    }
}

