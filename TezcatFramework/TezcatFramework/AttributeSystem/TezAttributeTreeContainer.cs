using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace tezcat.Framework.Attribute
{
	/// <summary>
	/// 属性系统的container
	/// 
	/// 可以自己切存储结构的方式
	/// 或者自己实现
	/// </summary>

	public abstract class TezAttributeTreeContainer
	{
	    public abstract void addPrimaryNode(int id, TezAttributeNode node);
	    public abstract void addSecondaryNode(int id, TezAttributeLeaf node);
	
	    public abstract bool tryGetPrimaryNode(int id, out TezAttributeNode node);
	    public abstract bool tryGetSecondaryNode(int id, out TezAttributeLeaf node);
	    public abstract void close();
	}
	
	public class TezAttributeTreeListContainer : TezAttributeTreeContainer
	{
	    List<TezAttributeNode> m_PrimaryNodes = new List<TezAttributeNode>();
	    List<TezAttributeLeaf> m_SecondaryNodes = new List<TezAttributeLeaf>();
	
	    public override void addPrimaryNode(int id, TezAttributeNode node)
	    {
	        ///id = 7 实际为第8个
	        ///Count = 5
	        ///rc = 2
	        ///
	        var remain_count = m_PrimaryNodes.Count - id;
	        if (remain_count > 0)
	        {
	            m_PrimaryNodes[id] = node;
	        }
	        else if (remain_count == 0)
	        {
	            m_PrimaryNodes.Add(node);
	        }
	        else
	        {
	            remain_count = -remain_count + 1;
	            m_PrimaryNodes.AddRange(new TezAttributeNode[remain_count]);
	            m_PrimaryNodes[id] = node;
	        }
	    }
	
	    public override void addSecondaryNode(int id, TezAttributeLeaf node)
	    {
	        var remain_count = m_SecondaryNodes.Count - id;
	        if (remain_count > 0)
	        {
	            m_SecondaryNodes[id] = node;
	        }
	        else if (remain_count == 0)
	        {
	            m_SecondaryNodes.Add(node);
	        }
	        else
	        {
	            remain_count = -remain_count + 1;
	            m_SecondaryNodes.AddRange(new TezAttributeLeaf[remain_count]);
	            m_SecondaryNodes[id] = node;
	        }
	    }
	
	    public override void close()
	    {
	        foreach (var item in m_PrimaryNodes)
	        {
	            item.close();
	        }
	
	        foreach (var item in m_SecondaryNodes)
	        {
	            item.close();
	        }
	
	        m_PrimaryNodes.Clear();
	        m_SecondaryNodes.Clear();
	
	        m_PrimaryNodes = null;
	        m_SecondaryNodes = null;
	    }
	
	    public override bool tryGetPrimaryNode(int id, out TezAttributeNode node)
	    {
	        if (id < m_PrimaryNodes.Count)
	        {
	            node = m_PrimaryNodes[id];
	            return true;
	        }
	
	        node = null;
	        return false;
	    }
	
	    public override bool tryGetSecondaryNode(int id, out TezAttributeLeaf node)
	    {
	        if (id < m_SecondaryNodes.Count)
	        {
	            node = m_SecondaryNodes[id];
	            return true;
	        }
	
	        node = null;
	        return false;
	    }
	}
	
	public class TezAttributeTreeDictContainer : TezAttributeTreeContainer
	{
	    Dictionary<int, TezAttributeNode> m_PrimaryNodes = new Dictionary<int, TezAttributeNode>();
	    Dictionary<int, TezAttributeLeaf> m_SecondaryNodes = new Dictionary<int, TezAttributeLeaf>();
	
	    public override void addPrimaryNode(int id, TezAttributeNode node)
	    {
	        m_PrimaryNodes.Add(id, node);
	    }
	
	    public override void addSecondaryNode(int id, TezAttributeLeaf node)
	    {
	        m_SecondaryNodes.Add(id, node);
	    }
	
	    public override void close()
	    {
	        foreach (var pair in m_PrimaryNodes)
	        {
	            pair.Value.close();
	        }
	
	        foreach (var pair in m_SecondaryNodes)
	        {
	            pair.Value.close();
	        }
	
	        m_PrimaryNodes.Clear();
	        m_SecondaryNodes.Clear();
	
	        m_PrimaryNodes = null;
	        m_SecondaryNodes = null;
	    }
	
	    public override bool tryGetPrimaryNode(int id, out TezAttributeNode node)
	    {
	        return m_PrimaryNodes.TryGetValue(id, out node);
	    }
	
	    public override bool tryGetSecondaryNode(int id, out TezAttributeLeaf node)
	    {
	        return m_SecondaryNodes.TryGetValue(id, out node);
	    }
	}
}