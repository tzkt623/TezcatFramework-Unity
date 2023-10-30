using System.Collections.Generic;

namespace tezcat.Framework.BonusSystem
{
    /// <summary>
    /// 加成系统的container
    /// 
    /// <para>
    /// 可以切换存储结构的方式
    /// 或者继承TezAttributeTreeContainer自己实现
    /// </para>
    /// </summary>
    public abstract class TezBonusTreeContainer
    {
        public abstract void addNode(int id, TezBonusTreeNode node);
        public abstract bool tryGetNode(int id, out TezBonusTreeNode node);
        public abstract void close();
    }

    public class TezBonusTreeListContainer : TezBonusTreeContainer
    {
        List<TezBonusTreeNode> mNodeList = new List<TezBonusTreeNode>();

        public override void addNode(int id, TezBonusTreeNode node)
        {
            ///id = 7 实际为第8个
            ///Count = 5
            ///rc = 2
            var remain_count = mNodeList.Count - id;
            if (remain_count > 0)
            {
                mNodeList[id] = node;
            }
            else if (remain_count == 0)
            {
                mNodeList.Add(node);
            }
            else
            {
                remain_count = -remain_count + 1;
                mNodeList.AddRange(new TezBonusTreeNode[remain_count]);
                mNodeList[id] = node;
            }
        }

        public override void close()
        {
            foreach (var item in mNodeList)
            {
                item?.close();
            }

            mNodeList.Clear();
            mNodeList = null;
        }

        public override bool tryGetNode(int id, out TezBonusTreeNode node)
        {
            node = null;
            if (id < mNodeList.Count)
            {
                node = mNodeList[id];
            }

            return node != null;
        }
    }

    public class TezBonusTreeDictContainer : TezBonusTreeContainer
    {
        Dictionary<int, TezBonusTreeNode> mNodeDict = new Dictionary<int, TezBonusTreeNode>();

        public override void addNode(int id, TezBonusTreeNode node)
        {
            mNodeDict.Add(id, node);
        }

        public override void close()
        {
            foreach (var pair in mNodeDict)
            {
                pair.Value.close();
            }

            mNodeDict.Clear();
            mNodeDict = null;
        }

        public override bool tryGetNode(int id, out TezBonusTreeNode node)
        {
            return mNodeDict.TryGetValue(id, out node);
        }
    }
}