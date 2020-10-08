using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.Definition;
using tezcat.Framework.Extension;

namespace tezcat.Framework.ECS
{
    public class TezDefinitionHolder
        : ITezDefinitionObjectAndHandler
        , ITezCloseable
    {
        #region Factory
        static List<Tuple<List<ITezDefinitionToken>, List<ITezDefinitionToken>>> m_Factory = new List<Tuple<List<ITezDefinitionToken>, List<ITezDefinitionToken>>>();
        static Queue<int> m_FreeIndex = new Queue<int>();

        public static int alloc()
        {
            if (m_FreeIndex.Count > 0)
            {
                return m_FreeIndex.Dequeue();
            }

            var index = m_Factory.Count;
            var temp = new Tuple<List<ITezDefinitionToken>, List<ITezDefinitionToken>>(new List<ITezDefinitionToken>(),
                 new List<ITezDefinitionToken>());
            m_Factory.Add(temp);
            return index;
        }

        public static void free(int index)
        {
            m_FreeIndex.Enqueue(index);
            var tuple = m_Factory[index];
            tuple.Item1.Clear();
            tuple.Item2.Clear();
        }

        public static void addPrimaryToken(int index, ITezDefinitionToken definitionToken)
        {
            m_Factory[index].Item1.Add(definitionToken);
        }

        public static void addSecondaryToken(int index, ITezDefinitionToken definitionToken)
        {
            m_Factory[index].Item2.Add(definitionToken);
        }

        public static List<ITezDefinitionToken> getPrimaryTokens(int index)
        {
            return m_Factory[index].Item1;
        }

        public static List<ITezDefinitionToken> getSecondaryTokens(int index)
        {
            return m_Factory[index].Item2;
        }
        #endregion

        public TezDefinition definition { get; private set; } = null;

        TezEventExtension.Action<ITezDefinitionObject> m_OnAddDefinitionObject = null;
        TezEventExtension.Action<ITezDefinitionObject> m_OnRemoveDefinitionObject = null;
        int m_Index = -1;

        public void setListener(TezEventExtension.Action<ITezDefinitionObject> onAddDefinitionObject, TezEventExtension.Action<ITezDefinitionObject> onRemoveDefinitionObject)
        {
            m_OnAddDefinitionObject = onAddDefinitionObject;
            m_OnRemoveDefinitionObject = onRemoveDefinitionObject;
        }

        public void addDefinitionObject(ITezDefinitionObject def_object)
        {
            m_OnAddDefinitionObject(def_object);
        }

        public void removeDefinitionObject(ITezDefinitionObject def_object)
        {
            m_OnRemoveDefinitionObject(def_object);
        }

        public void addPrimaryToken(ITezDefinitionToken definitionToken)
        {
            addPrimaryToken(m_Index, definitionToken);
        }

        public void addSecondaryToken(ITezDefinitionToken definitionToken)
        {
            addSecondaryToken(m_Index, definitionToken);
        }

        public void beginPath()
        {
            m_Index = alloc();
        }

        public void endPath()
        {
            var p = getPrimaryTokens(m_Index);
            var s = getSecondaryTokens(m_Index);

            this.definition = new TezDefinition()
            {
                primaryPath = p.Count > 0 ? p.ToArray() : TezDefinition.DefaultPrimaryPath,
                secondaryPath = s.Count > 0 ? s.ToArray() : TezDefinition.DefaultSecondaryPath
            };

            free(m_Index);
            m_Index = -1;
        }

        public void close(bool self_close = true)
        {
            this.definition.close();

            this.definition = null;
            m_OnAddDefinitionObject = null;
            m_OnRemoveDefinitionObject = null;
        }
    }

    public abstract class TezGameObject
        : TezDataObject
        , ITezGameObjectComparer
    {
        /// <summary>
        /// 唯一名称ID
        /// </summary>
        public string NID { get; set; } = null;

        /// <summary>
        /// 对象分类
        /// </summary>
        public TezCategory category { get; private set; } = null;

        /// <summary>
        /// 模板物品
        /// </summary>
        public TezDatabaseGameItem templateItem { get; private set; } = null;

        /// <summary>
        /// 标签
        /// 可选功能
        /// </summary>
        public TezTagSet tagSet { get; protected set; } = null;

        /// <summary>
        /// 定义系统
        /// 可选功能
        /// </summary>
        public TezDefinitionHolder definitionHolder { get; protected set; } = null;

        /// <summary>
        /// UID
        /// </summary>
        private TezUID m_UID = new TezUID();

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
        /// 使用物品模板初始化对象
        /// </summary>
        public void initWithData(ITezSerializableItem item)
        {
            var data_item = (TezDatabaseGameItem)item;
            this.NID = data_item.NID;
            this.category = data_item.category;
            this.templateItem = data_item;
            m_UID.DBID = data_item.DBID;

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
        /// 与另一个对象相同
        /// 即拥有相同的运行时ID
        /// </summary>
        public bool sameAs(TezGameObject other)
        {
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
            m_UID.close();

            this.NID = null;
            this.category = null;
            this.templateItem = null;
            m_UID = null;
        }
    }
}

