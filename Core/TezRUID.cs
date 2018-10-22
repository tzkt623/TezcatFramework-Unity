using System;
using System.Collections.Generic;
using tezcat.DataBase;

namespace tezcat.Core
{
    public class TezRID
        : IEquatable<TezRID>
        , IComparable<TezRID>
    {
        class IDManager
        {
            class SubGroup
            {
                int m_IDGiver = 0;
                Queue<int> m_Free = new Queue<int>();

                public int giveID()
                {
                    if (m_Free.Count > 0)
                    {
                        return m_Free.Dequeue();
                    }

                    return m_IDGiver++;
                }

                public void recycleID(ref int id)
                {
                    m_Free.Enqueue(id);
                }
            }

            class Group
            {
                List<SubGroup> m_SubGroups = new List<SubGroup>();

                public int giveID(ref int sub_gid)
                {
                    while (sub_gid >= m_SubGroups.Count)
                    {
                        m_SubGroups.Add(new SubGroup());
                    }

                    return m_SubGroups[sub_gid].giveID();
                }

                public void recycleID(ref int sub_gid, ref int id)
                {
                    m_SubGroups[sub_gid].recycleID(ref id);
                }
            }

            List<Group> m_Groups = new List<Group>();

            public int giveID(int gid, int sub_gid)
            {
                while (gid >= m_Groups.Count)
                {
                    m_Groups.Add(new Group());
                }

                return m_Groups[gid].giveID(ref sub_gid);
            }

            public void recycleID(int gid, int sub_gid, int id)
            {
                m_Groups[gid].recycleID(ref sub_gid, ref id);
            }
        }

        static IDManager Manager = new IDManager();

        class Ref
        {
            public ITezGroup group { get; private set; } = null;
            public ITezSubGroup subGroup { get; private set; } = null;
            public int itemID { get; set; } = -1;
            public int dbID { get; set; } = -1;

            public int refrence { get; private set; } = 0;

            public Ref(ITezGroup group, ITezSubGroup sub_group)
            {
                this.group = group;
                this.subGroup = sub_group;
            }

            public void retain()
            {
                this.refrence += 1;
            }

            public bool release()
            {
                this.refrence -= 1;
                return this.refrence == 0;
            }

            public void close()
            {
                this.group = null;
                this.subGroup = null;
                this.itemID = -1;
                this.dbID = -1;
                this.refrence = -1;
            }

            public bool sameAs(Ref other)
            {
                return this.itemID == other.itemID
                    && this.subGroup == other.subGroup
                    && this.group == other.group
                    && this.dbID == other.dbID;
            }

            public bool differentWith(Ref other)
            {
                return this.itemID != other.itemID
                    || this.dbID != other.dbID
                    || this.subGroup != other.subGroup
                    || this.group != other.group;
            }
        }
        Ref m_Ref = null;

        public ITezGroup group { get { return m_Ref.group; } }
        public ITezSubGroup subGroup { get { return m_Ref.subGroup; } }
        public int itemID
        {
            get { return m_Ref.itemID; }
        }

        public int dbID
        {
            get { return m_Ref.dbID; }
        }

        public bool isRunTimeItem
        {
            get { return m_Ref.dbID == -1; }
        }

        public int refrence
        {
            get { return m_Ref.refrence; }
        }

        /// <summary>
        /// 生成一个新的ID不包含数据库ID
        /// 引用计数自动加一
        /// </summary>
        public TezRID(ITezGroup group, ITezSubGroup sub_group)
        {
            if (m_Ref != null)
            {
                throw new ArgumentException("m_Ref must null");
            }

            m_Ref = new Ref(group, sub_group);
            m_Ref.itemID = Manager.giveID(group.toID, subGroup.toID);
            m_Ref.retain();
        }

        /// <summary>
        /// 生成一个新的ID并且包含数据库ID
        /// 引用计数自动加一
        /// </summary>
        public TezRID(ITezGroup group, ITezSubGroup sub_group, int db_id)
        {
            if (m_Ref != null)
            {
                throw new ArgumentException("m_Ref must null");
            }

            m_Ref = new Ref(group, sub_group) { dbID = db_id };
            m_Ref.itemID = Manager.giveID(group.toID, subGroup.toID);
            m_Ref.retain();
        }

        /// <summary>
        /// 构造一个相同的ID副本
        /// 引用计数自动加一
        /// </summary>
        public TezRID(TezRID other)
        {
            if (m_Ref != null)
            {
                throw new ArgumentException("m_Ref must null");
            }

            if (other.m_Ref == null)
            {
                throw new ArgumentException("other.m_Ref must not null");
            }

            m_Ref = other.m_Ref;
            m_Ref.retain();
        }

        /// <summary>
        /// 在当前ID的类型基础之上
        /// 生成一个新的未使用的ID
        /// 引用计数自动加一
        /// </summary>
        public void updateID()
        {
            var temp = new Ref(m_Ref.group, m_Ref.subGroup);

            if (m_Ref.release())
            {
                Manager.recycleID(m_Ref.group.toID, m_Ref.subGroup.toID, m_Ref.itemID);
                m_Ref.close();
            }

            m_Ref = temp;
            m_Ref.itemID = Manager.giveID(group.toID, subGroup.toID);
            m_Ref.retain();
        }

        /// <summary>
        /// 清理此ID
        /// </summary>
        public void close()
        {
            if (m_Ref.release())
            {
                Manager.recycleID(m_Ref.group.toID, m_Ref.subGroup.toID, m_Ref.itemID);
                m_Ref.close();
                m_Ref = null;
            }
        }

        public override int GetHashCode()
        {
            return (m_Ref.group.toID & m_Ref.subGroup.toID & m_Ref.itemID).GetHashCode();
        }

        public bool sameAs(TezRID other)
        {
            return m_Ref.sameAs(other.m_Ref);
        }

        public bool differentWith(TezRID other)
        {
            return m_Ref.differentWith(other.m_Ref);
        }

        int IComparable<TezRID>.CompareTo(TezRID other)
        {
            return m_Ref.group.toID.CompareTo(other.group.toID);
        }

        bool IEquatable<TezRID>.Equals(TezRID other)
        {
            return m_Ref == other.m_Ref;
        }
    }

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
                while (iid >= m_Cache.Count)
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
            else if (m_Ref < 0)
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