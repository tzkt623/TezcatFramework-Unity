using System;
using System.Collections.Generic;
using tezcat.Framework.DataBase;

namespace tezcat.Framework.Core
{
    public class TezRID
        : IEquatable<TezRID>
        , IComparable<TezRID>
    {
        #region IDManager
        class IDManager
        {
            class SubGroup
            {
                private ulong m_IDGiver = 1;
                private Queue<ulong> m_Free = new Queue<ulong>();

                public ulong giveID()
                {
                    if (m_Free.Count > 0)
                    {
                        return m_Free.Dequeue();
                    }

                    return m_IDGiver++;
                }

                public void recycleID(ref ulong id)
                {
                    m_Free.Enqueue(id);
                }
            }

            class Group
            {
                List<SubGroup> m_SubGroups = new List<SubGroup>();

                public ulong giveID(ref int sub_gid)
                {
                    while (sub_gid >= m_SubGroups.Count)
                    {
                        m_SubGroups.Add(new SubGroup());
                    }

                    return m_SubGroups[sub_gid].giveID();
                }

                public void recycleID(ref int sub_gid, ref ulong id)
                {
                    m_SubGroups[sub_gid].recycleID(ref id);
                }
            }

            List<Group> m_Groups = new List<Group>();

            public ulong giveID(int gid, int sub_gid)
            {
                while (gid >= m_Groups.Count)
                {
                    m_Groups.Add(new Group());
                }

                return (ulong)gid << 48 | (ulong)sub_gid << 32 | m_Groups[gid].giveID(ref sub_gid);
            }

            public void recycleID(int gid, int sub_gid, ulong id)
            {
                m_Groups[gid].recycleID(ref sub_gid, ref id);
            }
        }
        static readonly IDManager Manager = new IDManager();
        #endregion

        class Ref
        {
            public const ulong EmptyID = 0;

            public ITezGroup group = null;
            public ITezDetailedGroup subGroup = null;
            public ulong itemID = EmptyID;
            public int dbID = -1;

            public int refrence { get; private set; } = 0;

            public Ref(ITezGroup group, ITezDetailedGroup sub_group)
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
                this.itemID = EmptyID;
                this.dbID = -1;
                this.refrence = -1;
            }

            public bool sameAs(Ref other)
            {
                return this.itemID == other.itemID;
#if Not_Use_BitID
                return this.itemID == other.itemID
                    && this.subGroup == other.subGroup
                    && this.group == other.group;
#endif
            }

            public bool differentWith(Ref other)
            {
                return this.itemID != other.itemID;
#if Not_Use_BitID
                return this.itemID != other.itemID
                    || this.subGroup != other.subGroup
                    || this.group != other.group;
#endif
            }
        }

        Ref m_Ref = null;
        public ITezGroup group { get { return m_Ref.group; } }
        public ITezDetailedGroup subgroup { get { return m_Ref.subGroup; } }
        public ulong itemID
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
        public TezRID(ITezGroup group, ITezDetailedGroup sub_group)
        {
            if (m_Ref != null)
            {
                throw new ArgumentException("m_Ref must null");
            }

            m_Ref = new Ref(group, sub_group);
            m_Ref.itemID = Manager.giveID(group.toID, subgroup.toID);
            m_Ref.retain();
        }

        /// <summary>
        /// 生成一个新的ID并且包含数据库ID
        /// 引用计数自动加一
        /// </summary>
        public TezRID(ITezGroup group, ITezDetailedGroup sub_group, int db_id)
        {
            if (m_Ref != null)
            {
                throw new ArgumentException("m_Ref must null");
            }

            m_Ref = new Ref(group, sub_group) { dbID = db_id };
            m_Ref.itemID = Manager.giveID(group.toID, subgroup.toID);
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

            if (other == null || other.m_Ref == null)
            {
                throw new ArgumentException("other or other.m_Ref must not null");
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
            m_Ref.itemID = Manager.giveID(group.toID, subgroup.toID);
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
            return m_Ref.itemID.GetHashCode();

#if Not_Use_BitID
            return (m_Ref.group.toID & m_Ref.subGroup.toID & m_Ref.itemID).GetHashCode();
#endif
        }

        public override string ToString()
        {
            return string.Format("{0}/{1}", m_Ref.group.toName, m_Ref.subGroup.toName);
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
}