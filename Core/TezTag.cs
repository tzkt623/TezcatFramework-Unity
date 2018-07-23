using System;
using System.Collections.Generic;
using tezcat.Signal;

namespace tezcat.Core
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
            if(m_List.Count > id)
            {
                return m_List[id];
            }

            return null;
        }

        public static TezTag get(string name)
        {
            int id = -1;
            if(m_Dic.TryGetValue(name, out id))
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
            var check = obj as TezTag;
            if (check)
            {
                return check.ID == this.ID;
            }

            return false;
        }

        bool IEquatable<TezTag>.Equals(TezTag other)
        {
            return other == null ? false : this.ID == other.ID;
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

    public class TezTagSet
    {
        HashSet<int> m_Set = new HashSet<int>();

        public void add(string name)
        {
            var tag = TezTag.get(name);
            if(tag)
            {
                m_Set.Add(tag.ID);
            }
        }

        public void remove(string name)
        {
            var tag = TezTag.get(name);
            if(tag)
            {
                m_Set.Remove(tag.ID);
            }
        }

        public void add(int id)
        {
            var tag = TezTag.get(id);
            if(tag)
            {
                m_Set.Add(tag.ID);
            }
        }

        public void remove(int id)
        {
            var tag = TezTag.get(id);
            if(tag)
            {
                m_Set.Remove(tag.ID);
            }
        }

        public void add(TezTag tag)
        {
            m_Set.Add(tag.ID);
        }

        public void remove(TezTag tag)
        {
            m_Set.Remove(tag.ID);
        }

        public void clear()
        {
            m_Set.Clear();
            m_Set = null;
        }

        public bool check(TezTag tag)
        {
            return m_Set.Contains(tag.ID);
        }

        public bool check(params TezTag[] tags)
        {
            bool result = true;
            foreach (var tag in tags)
            {
                result &= m_Set.Contains(tag.ID);
            }

            return result;
        }

        public void foreachTag(TezEventCenter.Action<TezTag> action)
        {
            foreach (var tag in m_Set)
            {
                action(TezTag.get(tag));
            }
        }
    }
}