using System.Collections.Generic;

namespace tezcat.Framework.AI
{
    public class TezAIFactory
    {
        static Dictionary<string, TezAIBehaviour> m_BehaviourDic = new Dictionary<string, TezAIBehaviour>();

        public static T get<T>(string name) where T : TezAIBehaviour
        {
            TezAIBehaviour temp = null;
            m_BehaviourDic.TryGetValue(name, out temp);
            return (T)temp;
        }

        public static T register<T>(string name) where T : TezAIBehaviour, new()
        {
            TezAIBehaviour temp = null;
            if (!m_BehaviourDic.TryGetValue(name, out temp))
            {
                temp = new T();
                m_BehaviourDic.Add(name, temp);
            }

            return (T)temp;
        }
    }
}