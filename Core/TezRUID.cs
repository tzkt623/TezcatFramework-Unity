using System;
using System.Collections.Generic;

namespace tezcat.Core
{
    /// <summary>
    /// 资源ID
    /// </summary>
    public class TezRUID
        : ITezCloseable
        , IEquatable<TezRUID>
        , IComparable<TezRUID>
    {
        #region 分组
        static List<Group> m_Group = new List<Group>();
        class Group
        {
            List<TezRUID> m_Cache = new List<TezRUID>();
            Stack<int> m_FreeID = new Stack<int>();
            int m_Giver = -1;

            public int GID { get; }
            public Group(int gid)
            {
                this.GID = gid;
            }

            public bool hasFreeID()
            {
                return m_FreeID.Count > 0;
            }

            public void recycle(int id)
            {
                m_FreeID.Push(id);
            }

            public TezRUID give()
            {
                int free = -1;
                if (m_FreeID.Count > 0)
                {
                    free = m_FreeID.Pop();
                    return m_Cache[free].clone();
                }
                else
                {
                    var RUID = new TezRUID();
                    RUID.GID = GID;
                    RUID.IID = m_Cache.Count;
                    m_Cache.Add(RUID);
                    return RUID.clone();
                }
            }

            public TezRUID create(int iid)
            {
                while(iid >= m_Cache.Count)
                {
                    var RUID = new TezRUID();
                    RUID.GID = GID;
                    RUID.IID = m_Cache.Count;
                    m_Cache.Add(RUID);
                }

                return m_Cache[iid].clone();
            }
        }

        public static TezRUID create(int gid, int iid)
        {
            while (m_Group.Count <= gid)
            {
                m_Group.Add(new Group(m_Group.Count));
            }

            return m_Group[gid].create(iid);
        }

        public static TezRUID update(int gid)
        {
            return m_Group[gid].give();
        }
        #endregion

        public int GID { get; private set; } = -1;
        public int IID { get; private set; } = -1;
        int m_Ref = 0;

        private TezRUID()
        {
            m_Ref = 0;
        }

        public TezRUID clone()
        {
            m_Ref += 1;
            return this;
        }

        public void close()
        {
            m_Ref -= 1;
            if (m_Ref == 0)
            {
                m_Group[GID].recycle(IID);
            }
            else if(m_Ref < 0)
            {
                throw new ArgumentOutOfRangeException("Ref >> Where Call [=] To Clone RUID??");
            }
        }

        public override string ToString()
        {
            return string.Format("{0}{1}-R:{2}", GID, IID, m_Ref);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as TezRUID);
        }

        public override int GetHashCode()
        {
            return (GID & IID).GetHashCode();
        }

        public bool Equals(TezRUID other)
        {
            return GID == other.GID && IID == other.IID;
        }

        public int CompareTo(TezRUID other)
        {
            return IID.CompareTo(other.IID);
        }

        public static bool operator ==(TezRUID a, TezRUID b)
        {
            return a.GID == b.GID && a.IID == b.IID;
        }

        public static bool operator !=(TezRUID a, TezRUID b)
        {
            return a.GID != b.GID || a.IID != b.IID;
        }


        #region 重载操作
        public static bool operator true(TezRUID obj)
        {
            return !object.ReferenceEquals(obj, null);
        }

        public static bool operator false(TezRUID obj)
        {
            return object.ReferenceEquals(obj, null);
        }

        public static bool operator !(TezRUID obj)
        {
            return object.ReferenceEquals(obj, null);
        }
        #endregion
    }
}