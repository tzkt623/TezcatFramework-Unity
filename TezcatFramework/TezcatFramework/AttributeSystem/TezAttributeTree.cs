using System;
using System.Collections.Generic;

namespace tezcat.Framework.Attribute
{
    /// <summary>
    /// <para>依赖于TezAttributeDef的定义分类树</para>
    /// <para>用于RPG游戏类的加成系统</para>
    /// <para>常见于装备,技能,属性</para>
    /// </summary>
    /// <typeparam name="Container">分类器内部结构 默认提供List和Dict两种任选</typeparam>
    public abstract class TezAttributeTree<Container>
        : ITezAttributeTree
        where Container : TezAttributeTreeContainer, new()
    {
        const int cPrimaryBegin = 0;
        TezAttributeTreeContainer m_Container = new Container();

        /// <summary>
        /// 关闭
        /// </summary>
        public virtual void close()
        {
            m_Container.close();
            m_Container = null;
        }

        protected abstract TezAttributeNode onCreatePrimaryChild(ITezAttributeToken token);
        protected abstract TezAttributeLeaf onCreateSecondaryChild(ITezAttributeToken token);

        protected TezAttributeNode getOrCreatePrimaryNode(ITezAttributeToken token, ref TezAttributePath pre_path_node)
        {
            var id = token.tokenID;

            TezAttributeNode node = null;
            if (!m_Container.tryGetPrimaryNode(id, out node))
            {
                node = this.onCreatePrimaryChild(token);
                m_Container.addPrimaryNode(id, node);
                pre_path_node?.addChild(id);
            }

            if (node.nodeType == TezAttributeNodeType.Path)
            {
                pre_path_node = (TezAttributePath)node;
            }

            return node;
        }

        protected TezAttributeNode getOrCreatePrimaryNode(ITezAttributeToken token)
        {
            var id = token.tokenID;

            TezAttributeNode node = null;
            if (!m_Container.tryGetPrimaryNode(id, out node))
            {
                node = this.onCreatePrimaryChild(token);
                m_Container.addPrimaryNode(id, node);
            }

            return node;
        }

        public TezAttributeNode getPrimaryNode(ITezAttributeToken token)
        {
            return this.getPrimaryNode(token.tokenID);
        }

        public TezAttributeNode getPrimaryNode(int id)
        {
            m_Container.tryGetPrimaryNode(id, out TezAttributeNode node);
            return node;
        }

        protected TezAttributeLeaf getOrCreateSecondaryNode(ITezAttributeToken token)
        {
            var id = token.tokenID;

            TezAttributeLeaf node = null;
            if (!m_Container.tryGetSecondaryNode(id, out node))
            {
                node = this.onCreateSecondaryChild(token);
                m_Container.addSecondaryNode(id, node);
            }

            return node;
        }

        public TezAttributeLeaf getSecondaryNode(ITezAttributeToken token)
        {
            return this.getSecondaryNode(token.tokenID);
        }

        public TezAttributeLeaf getSecondaryNode(int id)
        {
            m_Container.tryGetSecondaryNode(id, out TezAttributeLeaf node);
            return node;
        }

        /// <summary>
        /// 注册Object
        /// </summary>
        public void registerObject(ITezAttributeBuilder builder)
        {
            var definition_path = builder.definition;
            var primary_length = definition_path.primaryLength;
            TezAttributePath pre_path_node = null;
            for (int i = cPrimaryBegin; i < primary_length; i++)
            {
                this.getOrCreatePrimaryNode(definition_path.getPrimaryPathToken(i), ref pre_path_node).onRegisterObject(builder);
            }

            var secondary_length = definition_path.secondaryLength;
            for (int i = 0; i < secondary_length; i++)
            {
                this.getOrCreateSecondaryNode(definition_path.getSecondaryPathToken(i)).onRegisterObject(builder);
            }
        }

        /// <summary>
        /// 单独注册PrimaryPath
        /// </summary>
        public void registerPrimaryPath(ITezAttributeBuilder builder)
        {
            var definition_path = builder.definition;
            var primary_length = definition_path.primaryLength;
            TezAttributePath pre_path_node = null;
            for (int i = cPrimaryBegin; i < primary_length; i++)
            {
                this.getOrCreatePrimaryNode(definition_path.getPrimaryPathToken(i), ref pre_path_node).onRegisterObject(builder);
            }
        }

        /// <summary>
        /// 单独注册SecondaryPath
        /// </summary>
        public void registerSecondaryPath(ITezAttributeBuilder builder)
        {
            var definition_path = builder.definition;
            var secondary_length = definition_path.secondaryLength;
            for (int i = 0; i < secondary_length; i++)
            {
                this.getOrCreateSecondaryNode(definition_path.getSecondaryPathToken(i)).onRegisterObject(builder);
            }
        }

        /// <summary>
        /// 单独注册一个SecondaryToken
        /// </summary>
        /// <param name="builder"></param>
        public void registerSecondaryPath(ITezAttributeBuilder builder, ITezAttributeToken token)
        {
            this.getOrCreateSecondaryNode(token).onRegisterObject(builder);
        }

        /// <summary>
        /// 注销Object
        /// </summary>
        public void unregisterObject(ITezAttributeBuilder builder)
        {
            var definition_path = builder.definition;
            var primary_length = definition_path.primaryLength;
            for (int i = cPrimaryBegin; i < primary_length; i++)
            {
                this.getPrimaryNode(definition_path.getPrimaryPathToken(i)).onUnregisterObject(builder);
            }

            var secondary_length = definition_path.secondaryLength;
            for (int i = 0; i < secondary_length; i++)
            {
                this.getSecondaryNode(definition_path.getSecondaryPathToken(i)).onUnregisterObject(builder);
            }
        }

        /// <summary>
        /// 单独注销PrimaryPath
        /// </summary>
        public void unregisterPrimaryPath(ITezAttributeBuilder builder)
        {
            var definition_path = builder.definition;
            var primary_length = definition_path.primaryLength;
            for (int i = cPrimaryBegin; i < primary_length; i++)
            {
                this.getPrimaryNode(definition_path.getPrimaryPathToken(i)).onUnregisterObject(builder);
            }
        }

        /// <summary>
        /// 单独注销SecondaryPath
        /// </summary>
        public void unregisterSecondaryPath(ITezAttributeBuilder builder)
        {
            var definition_path = builder.definition;
            var secondary_length = definition_path.secondaryLength;
            for (int i = 0; i < secondary_length; i++)
            {
                this.getSecondaryNode(definition_path.getSecondaryPathToken(i)).onUnregisterObject(builder);
            }
        }

        /// <summary>
        /// 单独注销一个SecondaryToken
        /// </summary>
        /// <param name="builder"></param>
        public void unregisterSecondaryPath(ITezAttributeBuilder builder, ITezAttributeToken token)
        {
            this.getSecondaryNode(token).onUnregisterObject(builder);
        }

        /// <summary>
        /// 添加一个带定义的对象
        /// 一般为各种Modifier
        /// 用于属性加成等系统
        /// </summary>
        public void addAttributeDefObject(ITezAttributeDefObject def_object)
        {
            ///如果没有Object被注册到路径上
            ///那么就算此Modifier加入了也不会造成任何影响
            ///
            ///如果路径已经建立好了
            ///Modifier加入时只需要直接加入他定义路径的最后的位置上即可
            var definition = def_object.definition;
            int length = definition.primaryLength;
            if (length > 0)
            {
                var node = this.getOrCreatePrimaryNode(definition.getPrimaryPathToken(length - 1));
                node.addAttributeDefObject(def_object);
            }
            else
            {
                length = definition.secondaryLength;
                for (int i = 0; i < length; i++)
                {
                    var node = this.getOrCreateSecondaryNode(definition.getSecondaryPathToken(i));
                    node.addAttributeDefObject(def_object);
                }
            }
        }

        public void removeAttributeDefObject(ITezAttributeDefObject def_object)
        {
            var defition = def_object.definition;
            int length = defition.primaryLength;
            if (length > 0)
            {
                var node = this.getPrimaryNode(defition.getPrimaryPathToken(length - 1));
                node.removeAttributeDefObject(def_object);
            }
            else
            {
                length = defition.secondaryLength;
                for (int i = 0; i < length; i++)
                {
                    var node = this.getSecondaryNode(defition.getSecondaryPathToken(i));
                    node.removeAttributeDefObject(def_object);
                }
            }
        }
    }
}