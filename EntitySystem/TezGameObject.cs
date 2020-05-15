using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.Definition;

namespace tezcat.Framework.ECS
{
    public abstract class TezGameObject
        : TezDataObject
        , ITezTagSet
        , ITezDefinitionPathObject
        , ITezGameObjectComparer
    {
        private TezUID m_UID = new TezUID();

        /// <summary>
        /// 对象分类
        /// </summary>
        public TezCategory category { get; private set; }

        /// <summary>
        /// 模板物品
        /// </summary>
        public TezDatabaseGameItem templateItem { get; private set; }

        /// <summary>
        /// 唯一名称ID
        /// </summary>
        public string NID { get; set; } = null;

        /// <summary>
        /// 标签
        /// </summary>
        public TezTagSet TAG { get; private set; } = null;


        #region 定义路径
        /// <summary>
        /// 属性定义路径
        /// 用于属性系统
        /// </summary>
        public TezDefinitionPath definitionPath { get; private set; } = null;

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

        /// <summary>
        /// 建立主顺序路径
        /// </summary>
        protected virtual void onBuildPrimaryTokens(ref List<ITezDefinitionToken> list)
        {

        }

        /// <summary>
        /// 建立副平行路径
        /// </summary>
        protected virtual void onBuildSecondaryTokens(ref List<ITezDefinitionToken> list)
        {

        }
        #endregion

        /// <summary>
        /// 初始化Object
        /// </summary>
        public void initNew()
        {
            this.TAG = new TezTagSet();

            this.preInit();
            this.onInitNew();
            this.postInit();
        }

        protected virtual void onInitNew()
        {

        }

        public void initWithData(ITezSerializableItem item)
        {
            var data_item = (TezDatabaseGameItem)item;
            m_UID.DBID = data_item.DBID;
            this.NID = data_item.NID;
            this.TAG = new TezTagSet();
            this.category = data_item.category;
            this.templateItem = data_item;

            this.preInit();
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
            this.definitionPath = new TezDefinitionPath((this.buildPrimaryToken && this.primaryTokens.Count > 0) ? this.primaryTokens.ToArray() : null,(this.buildSecondaryToken && this.secondaryTokens.Count > 0) ? this.secondaryTokens.ToArray() : null);

            this.onBuildDefinitionPath();
        }

        /// <summary>
        /// 路径建立完成之后
        /// </summary>
        protected virtual void onBuildDefinitionPath()
        {

        }

        /// <summary>
        /// 与另一个对象相同
        /// 即拥有相同的运行时ID
        /// </summary>
        public bool sameAs(TezGameObject other)
        {
            //            return m_RID.sameAs(other_game_object.m_RID);
            return m_UID.sameAs(other.m_UID);
        }

        /// <summary>
        /// 与另一个对象的模板相同
        /// 即拥有相同的数据库ID
        /// </summary>
        public bool templateAs(TezGameObject other)
        {
            return m_UID.DBID.sameAs(other.m_UID.DBID);
        }

        /// <summary>
        /// 更新当前对象的资源ID
        /// </summary>
        public void updateRID()
        {
            //             if (m_RID != null)
            //             {
            //                 var g = m_RID.group;
            //                 var sg = m_RID.subgroup;
            //                 m_RID.close();
            //                 m_RID = new TezRID(g, sg);
            //             }
            //             else
            //             {
            //                 m_RID = new TezRID(group, subgroup);
            //             }
            // 
            //             m_RID.updateID();
        }

        public void debugRID()
        {
            //            Debug.Log(m_RID.ToString());
        }

        /// <summary>
        /// 删除Object
        /// </summary>
        public override void close(bool self_close = true)
        {
            this.definitionPath?.close(false);
            this.TAG.close(false);
            m_UID.close();

            this.TAG = null;
            this.NID = null;
            this.templateItem = null;
            m_UID = null;
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

