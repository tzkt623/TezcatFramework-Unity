using System;
using System.Collections.Generic;

namespace tezcat.Core
{
    public class TezUID
        : TezObject
        , IEquatable<TezUID>
        , IComparable<TezUID>
    {
        #region 分组
        class Group
        {
            static Stack<uint> m_FreeID = new Stack<uint>();
            static uint m_Giver = 0;

            int m_GID;
            public Group(int gid)
            {
                this.m_GID = gid;
            }

            public bool hasID()
            {
                return m_FreeID.Count > 0 || m_Giver < TezcatFramework.UIDMax;
            }

            public bool tryGiveID(ref int gid, ref uint id)
            {
                gid = m_GID;
                if (m_FreeID.Count > 0)
                {
                    id = m_FreeID.Pop();
                    return true;
                }

                if (m_Giver < TezcatFramework.UIDMax)
                {
                    id = m_Giver++;
                    return true;
                }

                gid = -1;
                id = 0;
                return false;
            }

            public void recycle(uint id)
            {
                m_FreeID.Push(id);
            }
        }

        static List<Group> m_Group = null;
        static Group m_Current = null;

        static void giveID(ref int gid, ref uint id)
        {
            if(!m_Current.tryGiveID(ref gid, ref id))
            {
                foreach (var group in m_Group)
                {
                    if(group.hasID())
                    {
                        m_Current = group;
                        m_Current.tryGiveID(ref gid, ref id);
                        return;
                    }
                }

                m_Current = new Group(m_Group.Count);
                m_Group.Add(m_Current);
                m_Current.tryGiveID(ref gid, ref id);
            }
        }

        static void recycle(int gid, uint id)
        {
            m_Group[gid].recycle(id);
        }
        #endregion

        int m_GID = -1;
        uint m_UID = 0;

        static TezUID()
        {
            m_Group = new List<Group>();
            m_Current = new Group(m_Group.Count);
            m_Group.Add(m_Current);
        }

        public TezUID()
        {
            TezUID.giveID(ref m_GID, ref m_UID);
        }

        public override string ToString()
        {
            return string.Format("{0}{1}", m_GID, m_UID);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as TezUID);
        }

        public override int GetHashCode()
        {
            return m_UID.GetHashCode();
        }

        public bool Equals(TezUID other)
        {
            return other == null ? false : m_GID == other.m_GID && m_UID == other.m_UID;
        }

        public int CompareTo(TezUID other)
        {
            return m_UID.CompareTo(other.m_UID);
        }

        public override void close()
        {
            TezUID.recycle(m_GID, m_UID);
            m_GID = -1;
            m_UID = 0;
        }

        public static bool operator ==(TezUID a, TezUID b)
        {
            return a.m_GID == b.m_GID && a.m_UID == b.m_UID;
        }

        public static bool operator !=(TezUID a, TezUID b)
        {
            return a.m_GID != b.m_GID || a.m_UID != b.m_UID;
        }
    }
}