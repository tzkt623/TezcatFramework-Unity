using System.Collections.Generic;
using tezcat.Core;
using tezcat.Signal;

namespace tezcat.Utility
{
    public interface ITezPropertyOwner
    {
        System.Type GetType();
        List<TezValueWrapper> properties { get; }
    }

    public class TezPropertyManager
    {
        Dictionary<ITezValueName, TezValueWrapper> m_ValueDic = new Dictionary<ITezValueName, TezValueWrapper>();

        public void register(TezValueWrapper wrapper)
        {
            m_ValueDic.Add(wrapper.valueName, wrapper);
        }

        public void unregister(TezValueWrapper wrapper)
        {
            m_ValueDic.Remove(wrapper.valueName);
        }
    }
}
