using System.Collections.Generic;

namespace tezcat.Framework.Utility
{
    public interface ITezValiderHandler
    {
        TezValider valider { get; }
    }


    public class TezValider
	{
	    #region Pool
	    static Stack<TezValider> Pool = new Stack<TezValider>();
	    public static TezValider create()
	    {
	        if (Pool.Count > 0)
	        {
	            var valider = Pool.Pop();
	            valider.reset();
	            return valider;
	        }
	
	        return new TezValider();
	    }
	    #endregion
	
	    int m_Ref = 0;
	    bool m_Valid = false;
	
	    public bool valid
	    {
	        get { return m_Valid; }
	    }
	
	    private TezValider() { }
	
	    private void reset()
	    {
	        m_Ref = 0;
	        m_Valid = true;
	    }
	
	    public void setInvalid()
	    {
	        m_Valid = false;
	    }
	
	    public void retain()
	    {
	        m_Ref += 1;
	    }
	
	    public void release()
	    {
	        m_Ref -= 1;
	        if (m_Ref == 0)
	        {
	            m_Ref = -1;
	            Pool.Push(this);
	        }
	    }
	}
}
