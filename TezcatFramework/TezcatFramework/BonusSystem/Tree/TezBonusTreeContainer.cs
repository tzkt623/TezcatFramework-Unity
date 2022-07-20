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
	    List<TezBonusTreeNode> m_NodeList = new List<TezBonusTreeNode>();
	
	    public override void addNode(int id, TezBonusTreeNode node)
	    {
	        ///id = 7 实际为第8个
	        ///Count = 5
	        ///rc = 2
	        var remain_count = m_NodeList.Count - id;
	        if (remain_count > 0)
	        {
	            m_NodeList[id] = node;
	        }
	        else if (remain_count == 0)
	        {
	            m_NodeList.Add(node);
	        }
	        else
	        {
	            remain_count = -remain_count + 1;
	            m_NodeList.AddRange(new TezBonusTreeNode[remain_count]);
	            m_NodeList[id] = node;
	        }
	    }
	
	    public override void close()
	    {
	        foreach (var item in m_NodeList)
	        {
	            item.close();
	        }
	
	        m_NodeList.Clear();
	        m_NodeList = null;
	    }
	
	    public override bool tryGetNode(int id, out TezBonusTreeNode node)
	    {
	        if (id < m_NodeList.Count)
	        {
	            node = m_NodeList[id];
	            return true;
	        }
	
	        node = null;
	        return false;
	    }
	}
	
	public class TezBonusTreeDictContainer : TezBonusTreeContainer
	{
	    Dictionary<int, TezBonusTreeNode> m_NodeDict = new Dictionary<int, TezBonusTreeNode>();
	
	    public override void addNode(int id, TezBonusTreeNode node)
	    {
	        m_NodeDict.Add(id, node);
	    }
	
	    public override void close()
	    {
	        foreach (var pair in m_NodeDict)
	        {
	            pair.Value.close();
	        }
	
	        m_NodeDict.Clear();
	        m_NodeDict = null;
	    }
	
	    public override bool tryGetNode(int id, out TezBonusTreeNode node)
	    {
	        return m_NodeDict.TryGetValue(id, out node);
	    }
	}
}