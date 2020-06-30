using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.Definition;

namespace tezcat.Framework.ECS
{
    public abstract class TezGameObject
        : TezDataObject
        , ITezTagSet
        , ITezDefinitionObjectAndHandler
        , ITezGameObjectComparer
    {
        /// <summary>
        /// UID
        /// </summary>
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
        public TezDefinition definition { get; private set; } = null;

        protected virtual bool buildPrimaryToken { get; } = true;
        protected virtual bool buildSecondaryToken { get; } = true;

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

        /// <summary>
        /// 新建一个白板对象
        /// 不依赖数据模板
        /// </summary>
        protected virtual void onInitNew()
        {

        }

        /// <summary>
        /// 使用物品模板初始化对象
        /// </summary>
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
        public void buildDefinition()
        {
            this.definition = new TezDefinition((this.buildPrimaryToken && this.primaryTokens.Count > 0) ? this.primaryTokens.ToArray() : null, (this.buildSecondaryToken && this.secondaryTokens.Count > 0) ? this.secondaryTokens.ToArray() : null);

            this.onBuildDefinition();
        }

        /// <summary>
        /// 路径建立完成之后
        /// </summary>
        protected virtual void onBuildDefinition()
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
            if (m_UID.DBID == null || other.m_UID.DBID == null)
            {
                return false;
            }

            return m_UID.DBID.sameAs(other.m_UID.DBID);
        }

        /// <summary>
        /// 删除Object
        /// </summary>
        public override void close(bool self_close = true)
        {
            this.definition?.close(false);
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

        public virtual void addDefinitionObject(ITezDefinitionObject def_object) { }
        public virtual void removeDefinitionObject(ITezDefinitionObject def_object) { }
    }
}

