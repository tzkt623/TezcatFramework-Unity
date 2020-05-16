using System;
using System.Collections.Generic;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public class TezTag
        : IEquatable<TezTag>
        , IComparable<TezTag>
    {
        #region Manager
        static Dictionary<string, int> m_Dic = new Dictionary<string, int>();
        static List<TezTag> m_List = new List<TezTag>();

        public static TezTag register(string name)
        {
            int id = -1;
            if (m_Dic.TryGetValue(name, out id))
            {
                return m_List[id];
            }
            else
            {
                id = m_List.Count;
                var tag = new TezTag()
                {
                    name = name,
                    ID = id
                };
                m_List.Add(tag);
                m_Dic.Add(name, id);

                return tag;
            }
        }

        public static TezTag get(int id)
        {
            if (m_List.Count > id)
            {
                return m_List[id];
            }

            return null;
        }

        public static TezTag get(string name)
        {
            int id = -1;
            if (m_Dic.TryGetValue(name, out id))
            {
                return m_List[id];
            }

            return null;
        }
        #endregion

        public string name { get; private set; } = null;
        public int ID { get; private set; } = -1;

        private TezTag()
        {

        }

        public override int GetHashCode()
        {
            return this.ID;
        }

        public override bool Equals(object obj)
        {
            var check = (TezTag)obj;
            return check.ID == this.ID;
        }

        bool IEquatable<TezTag>.Equals(TezTag other)
        {
            return this.Equals(other);
        }

        int IComparable<TezTag>.CompareTo(TezTag other)
        {
            return this.ID.CompareTo(other.ID);
        }

        public static bool operator ==(TezTag a, TezTag b)
        {
            return a.ID == b.ID;
        }

        public static bool operator !=(TezTag a, TezTag b)
        {
            return a.ID != b.ID;
        }

        public static bool operator true(TezTag obj)
        {
            return !object.ReferenceEquals(obj, null);
        }

        public static bool operator false(TezTag obj)
        {
            return object.ReferenceEquals(obj, null);
        }

        public static bool operator !(TezTag obj)
        {
            return object.ReferenceEquals(obj, null);
        }
    }

    public class TezTagSet : ITezCloseable
    {
        HashSet<int> m_Set = new HashSet<int>();

        public void add(string name)
        {
            this.add(TezTag.get(name));
        }

        public void remove(string name)
        {
            this.remove(TezTag.get(name));
        }

        public void add(int id)
        {
            this.add(TezTag.get(id));
        }

        public void remove(int id)
        {
            this.remove(TezTag.get(id));
        }

        public void add(TezTag tag)
        {
            m_Set.Add(tag.ID);
        }

        public void remove(TezTag tag)
        {
            m_Set.Remove(tag.ID);
        }

        public void close(bool self_close = true)
        {
            m_Set.Clear();
            m_Set = null;
        }

        public bool oneOf(TezTag tag)
        {
            return m_Set.Contains(tag.ID);
        }

        public bool allOf(params TezTag[] tags)
        {
            foreach (var tag in tags)
            {
                if (!m_Set.Contains(tag.ID))
                {
                    return false;
                }
            }

            return true; ;
        }

        public bool anyOf(params TezTag[] tags)
        {
            foreach (var tag in tags)
            {
                if (m_Set.Contains(tag.ID))
                {
                    return true;
                }
            }

            return false;
        }

        public bool noneOf(params TezTag[] tags)
        {
            foreach (var tag in tags)
            {
                if (m_Set.Contains(tag.ID))
                {
                    return false;
                }
            }

            return true;
        }

        public void foreachTag(TezEventExtension.Action<TezTag> action)
        {
            foreach (var tag in m_Set)
            {
                action(TezTag.get(tag));
            }
        }
    }
}