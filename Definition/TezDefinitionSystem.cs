using System;
using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Definition
{
    public interface ITezDefinitionSystem
        : ITezCloseable
        , ITezDefinitionHandler
    {
        TezDefinitionNode getPrimaryNode(int id);
        TezDefinitionLeaf getSecondaryNode(int id);
    }

    /// <summary>
    /// 定义分类系统
    /// 用于RPG游戏类的加成系统
    /// 常见于装备,技能,属性
    /// </summary>
    /// <typeparam name="Container">分类器内部结构 默认提供List和Hash两种任选</typeparam>
    public abstract class TezDefinitionSystem<Container>
        : ITezDefinitionSystem
        where Container : TezDefinitionSystemContainer, new()
    {
        const int PrimaryBegin = 0;
        TezDefinitionSystemContainer m_Container = new Container();

        /// <summary>
        /// 关闭
        /// </summary>
        public virtual void close()
        {
            m_Container.close();
            m_Container = null;
        }

        protected abstract TezDefinitionNode onCreatePrimaryChild(ITezDefinitionToken token);
        protected abstract TezDefinitionLeaf onCreateSecondaryChild(ITezDefinitionToken token);

        protected TezDefinitionNode getOrCreatePrimaryNode(ITezDefinitionToken token, ref TezDefinitionPath pre_path_node)
        {
            var id = token.tokenID;

            TezDefinitionNode node = null;
            if (!m_Container.tryGetPrimaryNode(id, out node))
            {
                node = this.onCreatePrimaryChild(token);
                m_Container.addPrimaryNode(id, node);
                pre_path_node?.addChild(id);
            }

            if (node.nodeType == TezDefinitionNodeType.Path)
            {
                pre_path_node = (TezDefinitionPath)node;
            }

            return node;
        }

        protected TezDefinitionNode getOrCreatePrimaryNode(ITezDefinitionToken token)
        {
            var id = token.tokenID;

            TezDefinitionNode node = null;
            if (!m_Container.tryGetPrimaryNode(id, out node))
            {
                node = this.onCreatePrimaryChild(token);
                m_Container.addPrimaryNode(id, node);
            }

            return node;
        }

        public TezDefinitionNode getPrimaryNode(ITezDefinitionToken token)
        {
            return this.getPrimaryNode(token.tokenID);
        }

        public TezDefinitionNode getPrimaryNode(int id)
        {
            m_Container.tryGetPrimaryNode(id, out TezDefinitionNode node);
            return node;
        }

        protected TezDefinitionLeaf getOrCreateSecondaryNode(ITezDefinitionToken token)
        {
            var id = token.tokenID;

            TezDefinitionLeaf node = null;
            if (!m_Container.tryGetSecondaryNode(id, out node))
            {
                node = this.onCreateSecondaryChild(token);
                m_Container.addSecondaryNode(id, node);
            }

            return node;
        }

        public TezDefinitionLeaf getSecondaryNode(ITezDefinitionToken token)
        {
            return this.getSecondaryNode(token.tokenID);
        }

        public TezDefinitionLeaf getSecondaryNode(int id)
        {
            m_Container.tryGetSecondaryNode(id, out TezDefinitionLeaf node);
            return node;
        }

        /// <summary>
        /// 注册Object
        /// </summary>
        public void registerObject(ITezDefinitionObjectAndHandler handler)
        {
            var definition_path = handler.definition;
            var primary_length = definition_path.primaryLength;
            TezDefinitionPath pre_path_node = null;
            for (int i = PrimaryBegin; i < primary_length; i++)
            {
                this.getOrCreatePrimaryNode(definition_path.getPrimaryPathToken(i), ref pre_path_node).onRegisterObject(handler);
            }

            var secondary_length = definition_path.secondaryLength;
            for (int i = 0; i < secondary_length; i++)
            {
                this.getOrCreateSecondaryNode(definition_path.getSecondaryPathToken(i)).onRegisterObject(handler);
            }
        }

        /// <summary>
        /// 单独注册PrimaryPath
        /// </summary>
        public void registerPrimaryPath(ITezDefinitionObjectAndHandler handler)
        {
            var definition_path = handler.definition;
            var primary_length = definition_path.primaryLength;
            TezDefinitionPath pre_path_node = null;
            for (int i = PrimaryBegin; i < primary_length; i++)
            {
                this.getOrCreatePrimaryNode(definition_path.getPrimaryPathToken(i), ref pre_path_node).onRegisterObject(handler);
            }
        }

        /// <summary>
        /// 单独注册SecondaryPath
        /// </summary>
        public void registerSecondaryPath(ITezDefinitionObjectAndHandler handler)
        {
            var definition_path = handler.definition;
            var secondary_length = definition_path.secondaryLength;
            for (int i = 0; i < secondary_length; i++)
            {
                this.getOrCreateSecondaryNode(definition_path.getSecondaryPathToken(i)).onRegisterObject(handler);
            }
        }

        /// <summary>
        /// 单独注册一个SecondaryToken
        /// </summary>
        /// <param name="handler"></param>
        public void registerSecondaryPath(ITezDefinitionObjectAndHandler handler, ITezDefinitionToken token)
        {
            this.getOrCreateSecondaryNode(token).onRegisterObject(handler);
        }

        /// <summary>
        /// 注销Object
        /// </summary>
        public void unregisterObject(ITezDefinitionObjectAndHandler handler)
        {
            var definition_path = handler.definition;
            var primary_length = definition_path.primaryLength;
            for (int i = PrimaryBegin; i < primary_length; i++)
            {
                this.getPrimaryNode(definition_path.getPrimaryPathToken(i)).onUnregisterObject(handler);
            }

            var secondary_length = definition_path.secondaryLength;
            for (int i = 0; i < secondary_length; i++)
            {
                this.getSecondaryNode(definition_path.getSecondaryPathToken(i)).onUnregisterObject(handler);
            }
        }

        /// <summary>
        /// 单独注销PrimaryPath
        /// </summary>
        public void unregisterPrimaryPath(ITezDefinitionObjectAndHandler handler)
        {
            var definition_path = handler.definition;
            var primary_length = definition_path.primaryLength;
            for (int i = PrimaryBegin; i < primary_length; i++)
            {
                this.getPrimaryNode(definition_path.getPrimaryPathToken(i)).onUnregisterObject(handler);
            }
        }

        /// <summary>
        /// 单独注销SecondaryPath
        /// </summary>
        public void unregisterSecondaryPath(ITezDefinitionObjectAndHandler handler)
        {
            var definition_path = handler.definition;
            var secondary_length = definition_path.secondaryLength;
            for (int i = 0; i < secondary_length; i++)
            {
                this.getSecondaryNode(definition_path.getSecondaryPathToken(i)).onUnregisterObject(handler);
            }
        }

        /// <summary>
        /// 单独注销一个SecondaryToken
        /// </summary>
        /// <param name="handler"></param>
        public void unregisterSecondaryPath(ITezDefinitionObjectAndHandler handler, ITezDefinitionToken token)
        {
            this.getSecondaryNode(token).onUnregisterObject(handler);
        }

        /// <summary>
        /// 添加一个带定义的对象
        /// 一般为各种Modifier
        /// 用于属性加成等系统
        /// </summary>
        public void addDefinitionObject(ITezDefinitionObject def_object)
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
                node.addDefinitionObject(def_object);
            }
            else
            {
                length = definition.secondaryLength;
                for (int i = 0; i < length; i++)
                {
                    var node = this.getOrCreateSecondaryNode(definition.getSecondaryPathToken(i));
                    node.addDefinitionObject(def_object);
                }
            }
        }

        public void removeDefinitionObject(ITezDefinitionObject def_object)
        {
            var defition = def_object.definition;
            int length = defition.primaryLength;
            if (length > 0)
            {
                var node = this.getPrimaryNode(defition.getPrimaryPathToken(length - 1));
                node.removeDefinitionObject(def_object);
            }
            else
            {
                length = defition.secondaryLength;
                for (int i = 0; i < length; i++)
                {
                    var node = this.getSecondaryNode(defition.getSecondaryPathToken(i));
                    node.removeDefinitionObject(def_object);
                }
            }
        }
    }
}