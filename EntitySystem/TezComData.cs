﻿using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.Definition;
using tezcat.Framework.Extension;
using tezcat.Framework.Utility;

namespace tezcat.Framework.ECS
{
    public abstract class TezComData
        : TezComBaseData
        , IEquatable<TezComData>
    {
        /// <summary>
        /// 唯一名称ID
        /// </summary>
        public string NID { get; protected set; }

        /// <summary>
        /// 模板物品
        /// </summary>
        public TezDatabaseGameItem templateItem { get; private set; } = null;

        /// <summary>
        /// 特化型分类系统
        /// 请勿释放此变量
        /// </summary>
        public TezCategory category { get; protected set; } = null;

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
            this.templateItem = data_item;
            this.category = data_item.category;
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

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return this.Equals((TezComData)other);
        }

        public bool Equals(TezComData other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return m_UID.sameAs(other.m_UID);
        }

        public static bool operator ==(TezComData a, TezComData b)
        {
            if (object.ReferenceEquals(a, null))
            {
                return object.ReferenceEquals(b, null);
            }

            return a.Equals(b);
        }

        public static bool operator !=(TezComData a, TezComData b)
        {
            return !(a == b);
        }


        /// <summary>
        /// 与另一个对象的模板相同
        /// 即拥有相同的数据库ID
        /// </summary>
        public bool templateAs(TezComData other)
        {
            return m_UID.dbSameAs(other.m_UID);
        }

        /// <summary>
        /// 删除Object
        /// </summary>
        public override void close()
        {
            m_UID.close();

            this.category = null;
            this.NID = null;
            this.templateItem = null;
            m_UID = null;
        }
    }
}