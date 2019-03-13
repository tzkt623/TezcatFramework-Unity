using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.DataBase;
using tezcat.Framework.Wrapper;
using UnityEngine;

namespace tezcat.Framework.ECS
{
    public abstract class TezDataObject
        : TezObject
        , ITezComponent
        , ITezSerializable
    {
        #region Component
        public TezEntity entity { get; private set; }

        void ITezComponent.onAdd(TezEntity entity)
        {
            this.entity = entity;
            this.onAddComponent(entity);
        }

        void ITezComponent.onRemove(TezEntity entity)
        {
            this.onRemoveComponent(entity);
            this.entity = null;
        }

        void ITezComponent.onOtherComponentAdded(ITezComponent component, int com_id)
        {
            this.onOtherComponentAdded(component, com_id);
        }

        void ITezComponent.onOtherComponentRemoved(ITezComponent component, int com_id)
        {
            this.onOtherComponentRemoved(component, com_id);
        }

        protected virtual void onAddComponent(TezEntity entity) { }

        protected virtual void onRemoveComponent(TezEntity entity) { }

        protected virtual void onOtherComponentAdded(ITezComponent com, int com_id) { }

        protected virtual void onOtherComponentRemoved(ITezComponent com, int com_id) { }

        public virtual void serialize(TezSaveManager manager) { }

        public virtual void deserialize(TezSaveManager manager) { }
        #endregion
    }

    /// <summary>
    /// 游戏Object
    /// </summary>
    public abstract class TezGameObject
         : TezDataObject
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

        #region Component
        public Wrapper getWrapper<Wrapper>() where Wrapper : ITezWrapper
        {
            return (Wrapper)this.getWrapper();
        }

        public ITezWrapper getWrapper()
        {
            var wrapper = this.entity.getComponent<TezWrapper>();
            if (!wrapper)
            {
                wrapper = this.onCreateWrapper();
                if (wrapper)
                {
                    this.entity.addComponent<TezWrapper>(wrapper);
                }
                else
                {
                    Debug.LogError(string.Format("{0} >> Get a Null wrapper", this.GetType().Name));
                }
            }
            return wrapper;
        }

        protected virtual TezWrapper onCreateWrapper()
        {
            return null;
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
        public abstract ITezSubgroup subgroup { get; }

        /// <summary>
        /// 唯一名称ID
        /// </summary>
        public string NID { get; set; } = null;

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

                this.onInitNew();
                this.m_RID = new TezRID(group, subgroup);
                this.NID = this.NID ?? string.Empty;
                this.TAG = new TezTagSet();
            }
            else
            {
                throw new ArgumentException(string.Format("{0} >> This Object is Init Again", this.GetType().Name));
            }
        }

        protected virtual void onInitNew()
        {

        }

        public void initWithData(ITezSerializableItem item)
        {
            var data_item = item as TezDataBaseGameItem;

            if (this.GUID == 0)
            {
                this.GUID = giveID();
            }

            this.m_RID?.close();
            this.m_RID = new TezRID(data_item.RID);

            this.NID = data_item.NID;
            this.TAG = new TezTagSet();
            this.onInitWithData(item);
        }

        protected virtual void onInitWithData(ITezSerializableItem item)
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
            if (m_RID != null)
            {
                var g = m_RID.group;
                var sg = m_RID.subgroup;
                m_RID.close();
                m_RID = new TezRID(g, sg);
            }
            else
            {
                m_RID = new TezRID(group, subgroup);
            }

            this.m_RID.updateID();
        }

        public void debugRID()
        {
            Debug.Log(m_RID.ToString());
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

        public override void serialize(TezSaveManager manager)
        {
            manager.write(TezReadOnlyString.Database.CID, this.CID);
            manager.write(TezReadOnlyString.Database.NID, this.NID);
        }

        public override void deserialize(TezSaveManager manager)
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

