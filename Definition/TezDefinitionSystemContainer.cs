using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace tezcat.Framework.Definition
{
	public abstract class TezDefinitionSystemContainer
	{
	    public abstract void addPrimaryNode(int id, TezDefinitionNode node);
	    public abstract void addSecondaryNode(int id, TezDefinitionLeaf node);
	
	    public abstract bool tryGetPrimaryNode(int id, out TezDefinitionNode node);
	    public abstract bool tryGetSecondaryNode(int id, out TezDefinitionLeaf node);
	    public abstract void close();
	}
	
	public class TezDefinitionSystemListContainer : TezDefinitionSystemContainer
	{
	    List<TezDefinitionNode> m_PrimaryNodes = new List<TezDefinitionNode>();
	    List<TezDefinitionLeaf> m_SecondaryNodes = new List<TezDefinitionLeaf>();
	
	    public override void addPrimaryNode(int id, TezDefinitionNode node)
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
	            m_PrimaryNodes.AddRange(new TezDefinitionNode[remain_count]);
	            m_PrimaryNodes[id] = node;
	        }
	    }
	
	    public override void addSecondaryNode(int id, TezDefinitionLeaf node)
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
	            m_SecondaryNodes.AddRange(new TezDefinitionLeaf[remain_count]);
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
	
	    public override bool tryGetPrimaryNode(int id, out TezDefinitionNode node)
	    {
	        if (id < m_PrimaryNodes.Count)
	        {
	            node = m_PrimaryNodes[id];
	            return true;
	        }
	
	        node = null;
	        return false;
	    }
	
	    public override bool tryGetSecondaryNode(int id, out TezDefinitionLeaf node)
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
	
	public class TezDefinitionSystemHashContainer : TezDefinitionSystemContainer
	{
	    Dictionary<int, TezDefinitionNode> m_PrimaryNodes = new Dictionary<int, TezDefinitionNode>();
	    Dictionary<int, TezDefinitionLeaf> m_SecondaryNodes = new Dictionary<int, TezDefinitionLeaf>();
	
	    public override void addPrimaryNode(int id, TezDefinitionNode node)
	    {
	        m_PrimaryNodes.Add(id, node);
	    }
	
	    public override void addSecondaryNode(int id, TezDefinitionLeaf node)
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
	
	    public override bool tryGetPrimaryNode(int id, out TezDefinitionNode node)
	    {
	        return m_PrimaryNodes.TryGetValue(id, out node);
	    }
	
	    public override bool tryGetSecondaryNode(int id, out TezDefinitionLeaf node)
	    {
	        return m_SecondaryNodes.TryGetValue(id, out node);
	    }
	}
}