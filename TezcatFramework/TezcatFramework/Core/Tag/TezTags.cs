using System.Collections.Generic;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public class TezTags : ITezCloseable
    {
        public IEnumerable<int> tags => mSet;

        HashSet<int> mSet = new HashSet<int>();

        public TezTags()
        {
        }

        public bool add(string name)
        {
            return mSet.Add(TezTagManager.get(name).id);
        }

        public bool remove(string name)
        {
            return mSet.Remove(TezTagManager.get(name).id);
        }

        public bool add(int tag)
        {
            return mSet.Add(TezTagManager.get(tag).id);
        }

        public bool remove(int tag)
        {
            return mSet.Remove(TezTagManager.get(tag).id);
        }

        public void close()
        {
            mSet.Clear();
            mSet = null;
        }

        /// <summary>
        /// 存在(仅1个)
        /// </summary>
        public bool oneOf(int tag)
        {
            return mSet.Contains(tag);
        }

        /// <summary>
        /// 不存在(仅1个)
        /// </summary>
        public bool noneOf(int tag)
        {
            return !mSet.Contains(tag);
        }

        /// <summary>
        /// 完全等于
        /// </summary>
        public bool equalOf(IEnumerable<int> tags)
        {
            return mSet.SetEquals(tags);
        }

        /// <summary>
        /// 全都存在
        /// </summary>
        public bool allOf(IEnumerable<int> tags)
        {
            return mSet.IsSupersetOf(tags);
        }

        /// <summary>
        /// 存在任意一个
        /// </summary>
        public bool anyOf(IEnumerable<int> tags)
        {
            return mSet.Overlaps(tags);
        }

        /// <summary>
        /// 全都不存在
        /// </summary>
        public bool noneOf(IEnumerable<int> tags)
        {
            return !mSet.Overlaps(tags);
        }

        public void foreachTag(TezEventExtension.Action<int> action)
        {
            foreach (var tag in mSet)
            {
                action(tag);
            }
        }
    }
}