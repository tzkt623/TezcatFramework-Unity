using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.Definition;
using UnityEngine;

namespace tezcat.Framework.ECS
{
    public abstract class TezGameObject
        : TezDataObject
        , ITezTagSet
        , ITezDefinitionPathObject
        , ITezGameObjectComparer
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
        public abstract ITezSubgroup subgroup { get; }

        /// <summary>
        /// 唯一名称ID
        /// </summary>
        public string NID { get; set; } = null;

        /// <summary>
        /// 标签
        /// </summary>
        public TezTagSet TAG { get; private set; } = null;


        #region 定义路径
        public TezDefinitionPath definitionPath { get; private set; } = null;

        protected virtual bool buildMainToken { get; } = true;
        protected virtual bool buildPrimaryToken { get; } = true;
        protected virtual bool buildSecondaryToken { get; } = true;

        protected virtual ITezDefinitionToken mainToken { get; } = null;
        protected List<ITezDefinitionToken> primaryTokens
        {
            get
            {
                List<ITezDefinitionToken> list = new List<ITezDefinitionToken>(2);
                this.onBuildPrimaryTokens(ref list);
                return list;
            }
        }
        protected List<ITezDefinitionToken> secondaryTokens
        {
            get
            {
                List<ITezDefinitionToken> list = new List<ITezDefinitionToken>(2);
                this.onBuildSecondaryTokens(ref list);
                return list;
            }
        }

        protected virtual void onBuildPrimaryTokens(ref List<ITezDefinitionToken> list)
        {

        }

        protected virtual void onBuildSecondaryTokens(ref List<ITezDefinitionToken> list)
        {

        }
        #endregion

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

                this.preInit();

                this.onInitNew();
                this.NID = this.NID ?? string.Empty;
                this.TAG = new TezTagSet();
                m_RID = new TezRID(group, subgroup);

                this.postInit();
            }
            else
            {
                throw new ArgumentException(string.Format("{0} >> This Object is Init Again", this.GetType().Name));
            }
        }

        protected virtual void onInitNew()
        {

        }

        public void initWithData(ITezSerializableItem item, bool copy)
        {
            var data_item = (TezDatabaseGameItem)item;

            if (this.GUID == 0)
            {
                this.GUID = giveID();
            }

            this.preInit();

            m_RID?.close();
            if (copy)
            {
                m_RID = new TezRID(data_item.RID);
            }
            else
            {
                m_RID = new TezRID(data_item.RID.group, data_item.RID.subgroup);
            }

            this.NID = data_item.NID;
            this.TAG = new TezTagSet();

            this.onInitWithData(item);

            this.postInit();
        }

        protected virtual void onInitWithData(ITezSerializableItem item)
        {

        }

        /// <summary>
        /// 数据初始化之前
        /// </summary>
        protected virtual void preInit()
        {

        }

        /// <summary>
        /// 数据初始化之后
        /// </summary>
        protected virtual void postInit()
        {

        }

        /// <summary>
        /// 建立类别路径
        /// </summary>
        public void buildDefinitionPath()
        {
            this.definitionPath = new TezDefinitionPath(
                this.buildMainToken ? this.mainToken : null,
                (this.buildPrimaryToken && this.primaryTokens.Count > 0) ? this.primaryTokens.ToArray() : null,
                (this.buildSecondaryToken && this.secondaryTokens.Count > 0) ? this.secondaryTokens.ToArray() : null);

            this.onBuildDefinitionPath();
        }

        /// <summary>
        /// 路径建立完成之后
        /// </summary>
        protected virtual void onBuildDefinitionPath()
        {

        }

        public bool sameAs(TezGameObject other_game_object)
        {
            return m_RID.sameAs(other_game_object.m_RID);
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

            m_RID.updateID();
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
            this.definitionPath.close();
            this.TAG.close();
            m_RID?.close();

            this.TAG = null;
            this.NID = null;
            m_RID = null;

            recycleID(this.GUID);
            this.GUID = 0;
        }

        public override void serialize(TezSaveManager manager)
        {
            manager.write(TezReadOnlyString.CID, this.CID);
            manager.write(TezReadOnlyString.NID, this.NID);
        }

        public override void deserialize(TezSaveManager manager)
        {
            this.NID = manager.readString(TezReadOnlyString.NID);
        }
    }
}

