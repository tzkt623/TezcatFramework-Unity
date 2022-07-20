using System.Collections.Generic;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public class TezTags : ITezCloseable
    {
        HashSet<int> m_Set = new HashSet<int>();

        public TezTags()
        {
        }

        public bool add(string name)
        {
            return m_Set.Add(TezTagGenerator.get(name).id);
        }

        public bool remove(string name)
        {
            return m_Set.Remove(TezTagGenerator.get(name).id);
        }

        public bool add(int tag)
        {
            return m_Set.Add(TezTagGenerator.get(tag).id);
        }

        public bool remove(int tag)
        {
            return m_Set.Remove(TezTagGenerator.get(tag).id);
        }

        public void close()
        {
            m_Set.Clear();
            m_Set = null;
        }

        /// <summary>
        /// 存在(仅1个)
        /// </summary>
        public bool oneOf(int tag)
        {
            return m_Set.Contains(tag);
        }

        /// <summary>
        /// 不存在(仅1个)
        /// </summary>
        public bool noneOf(int tag)
        {
            return !m_Set.Contains(tag);
        }

        /// <summary>
        /// 全都存在
        /// </summary>
        public bool allOf(int[] tags)
        {
            foreach (var tag in tags)
            {
                if (!m_Set.Contains(tag))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 存在任意一个
        /// </summary>
        public bool anyOf(int[] tags)
        {
            foreach (var tag in tags)
            {
                if (m_Set.Contains(tag))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 全都不存在
        /// </summary>
        public bool noneOf(int[] tags)
        {
            foreach (var tag in tags)
            {
                if (m_Set.Contains(tag))
                {
                    return false;
                }
            }

            return true;
        }

        public void foreachTag(TezEventExtension.Action<int> action)
        {
            foreach (var tag in m_Set)
            {
                action(tag);
            }
        }
    }
}