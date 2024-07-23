using tezcat.Framework.Core;

namespace tezcat.Framework.BonusSystem
{
    /// <summary>
    /// <para>加成树</para>
    /// <para>用于RPG游戏类的加成系统</para>
    /// <para>常见于装备,技能,属性</para>
    /// 
    /// 加成树的路径应该固定路径才对
    /// 也就是所有相同的系统,共享同一种加成路径
    /// 
    /// </summary>
    /// <typeparam name="Container">分类器内部结构 默认提供List和Dict两种任选</typeparam>
    public abstract class TezBonusTree<Container>
        : ITezBonusTree
        where Container : TezBonusTreeContainer, new()
    {
        const int cPrimaryBegin = 0;
        TezBonusTreeContainer mContainer = new Container();

        void ITezCloseable.closeThis()
        {
            this.onClose();
        }

        /// <summary>
        /// 关闭
        /// </summary>
        protected virtual void onClose()
        {
            mContainer.close();
            mContainer = null;
        }

        protected abstract TezBonusTreeNode onCreateNode(ITezBonusToken token);

        protected TezBonusTreeNode getOrCreateNode(ITezBonusToken token, ref TezBonusTreePathNode pre_path_node)
        {
            var id = token.tokenID;

            TezBonusTreeNode node = null;
            if (!mContainer.tryGetNode(id, out node))
            {
                node = this.onCreateNode(token);
                mContainer.addNode(id, node);
                pre_path_node?.addChild(id);
            }

            if (node.nodeType == TezBonusTreeNodeType.Path)
            {
                pre_path_node = (TezBonusTreePathNode)node;
            }

            return node;
        }

        protected TezBonusTreeNode getOrCreateNode(ITezBonusToken token)
        {
            var id = token.tokenID;

            TezBonusTreeNode node = null;
            if (!mContainer.tryGetNode(id, out node))
            {
                node = this.onCreateNode(token);
                mContainer.addNode(id, node);
            }

            return node;
        }

        public TezBonusTreeNode getNode(ITezBonusToken token)
        {
            return this.getNode(token.tokenID);
        }

        public TezBonusTreeNode getNode(int id)
        {
            mContainer.tryGetNode(id, out TezBonusTreeNode node);
            return node;
        }

        /// <summary>
        /// 注册Agent
        /// </summary>
        public void registerAgent(ITezBonusAgent agent)
        {
            var path = agent.bonusPath;
            var length = path.length;
            TezBonusTreePathNode pre_path_node = null;
            for (int i = cPrimaryBegin; i < length; i++)
            {
                this.getOrCreateNode(path.getToken(i), ref pre_path_node).registerAgent(agent);
            }
        }

        /// <summary>
        /// 单独注册PrimaryPath
        /// </summary>
        public void registerMainPath(ITezBonusAgent agent)
        {
            var definition_path = agent.bonusPath;
            var primary_length = definition_path.length;
            TezBonusTreePathNode pre_path_node = null;
            for (int i = cPrimaryBegin; i < primary_length; i++)
            {
                this.getOrCreateNode(definition_path.getToken(i), ref pre_path_node).registerAgent(agent);
            }
        }

        /// <summary>
        /// 注销Object
        /// </summary>
        public void unregisterAgent(ITezBonusAgent agent)
        {
            var definition_path = agent.bonusPath;
            var primary_length = definition_path.length;
            for (int i = cPrimaryBegin; i < primary_length; i++)
            {
                this.getNode(definition_path.getToken(i)).unregisterAgent(agent);
            }
        }

        /// <summary>
        /// 单独注销PrimaryPath
        /// </summary>
        public void unregisterMainPath(ITezBonusAgent agent)
        {
            var definition_path = agent.bonusPath;
            var primary_length = definition_path.length;
            for (int i = cPrimaryBegin; i < primary_length; i++)
            {
                this.getNode(definition_path.getToken(i)).unregisterAgent(agent);
            }
        }

        /// <summary>
        /// 添加一个带定义的对象
        /// 一般为各种Modifier
        /// 用于属性加成等系统
        /// </summary>
        public void addBonusObject(ITezBonusCarrier obj)
        {
            ///如果没有Object被注册到路径上
            ///那么就算此Modifier加入了也不会造成任何影响
            ///
            ///如果路径已经建立好了
            ///Modifier加入时只需要直接加入他定义路径的最后的位置上即可
            var definition = obj.bonusPath;
            int length = definition.length;
            var node = this.getOrCreateNode(definition.getToken(length - 1));
            node.addBountyObject(obj);
        }

        public void removeBonusObject(ITezBonusCarrier obj)
        {
            var defition = obj.bonusPath;
            int length = defition.length;
            var node = this.getNode(defition.getToken(length - 1));
            node.removeBountyObject(obj);
        }
    }
}