using System;
using System.Collections.Generic;
using tezcat.DataBase;
using tezcat.String;

namespace tezcat.Core
{
    /// <summary>
    /// 基础Object
    /// </summary>
    public abstract class TezObject
        : ITezCloseable
    {

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
        static Stack<uint> m_FreeID = new Stack<uint>();
        static uint m_IDGiver = 1;
        static uint giveID()
        {
            if (m_FreeID.Count > 0)
            {
                return m_FreeID.Pop();
            }

            return m_IDGiver++;
        }

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
        /// 物品ID
        /// </summary>
        public TezDataBaseGameItem gameItem { get; protected set; } = null;

        /// <summary>
        /// 唯一名称ID
        /// </summary>
        public TezStaticString NID { get; private set; } = null;

        /// <summary>
        /// 标签
        /// </summary>
        public TezTagSet TAG { get; private set; } = null;

        /// <summary>
        /// 初始化Object
        /// </summary>
        public void init()
        {
            if (GUID == 0)
            {
                this.GUID = giveID();
                this.NID = new TezStaticString();
                this.TAG = new TezTagSet();
                this.onInit();
            }
            else
            {
                throw new ArgumentException(string.Format("{0} >> This Object is Init Again", this.GetType().Name));
            }
        }

        protected virtual void onInit()
        {

        }

        public virtual void setData(TezDataBaseGameItem item)
        {
            this.init();
            this.gameItem = item;
            this.NID = item.NID;
        }

        public bool sameAs(TezGameObject game_object)
        {
            return this.gameItem == game_object.gameItem;
        }

        /// <summary>
        /// 将当前物品ID更新到一个新的ID
        /// </summary>
        public void updateItem()
        {
            var temp = TezService.get<TezDatabase>().updateItem(this.gameItem);
            temp.retain();
        }

        /// <summary>
        /// 删除Object
        /// </summary>
        public override void close()
        {
            this.TAG.close();
            this.TAG = null;

            this.NID.close();
            this.NID = null;

            if(this.gameItem != null && this.gameItem.release())
            {
                TezService.get<TezDatabase>().recycleItem(this.gameItem);
            }
            this.gameItem = null;

            m_FreeID.Push(this.GUID);
            this.GUID = 0;
        }

        public virtual void serialize(TezSaveManager manager)
        {
            manager.write(TezReadOnlyString.Database.CID, this.GetType().Name);
        }

        public virtual void deserialize(TezSaveManager reader)
        {
            this.init();
            this.NID = reader.readString(TezReadOnlyString.Database.NID);
        }
    }

    /// <summary>
    /// 工具Object
    /// </summary>
    public abstract class TezToolObject : TezObject
    {

    }
}

