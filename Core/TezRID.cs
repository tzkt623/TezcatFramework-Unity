using System;
using System.Collections.Generic;
using tezcat.Framework.Database;

namespace tezcat.Framework.Core
{
    public class TezRID
        : IEquatable<TezRID>
        , IComparable<TezRID>
    {
        #region IDManager
        class IDManager
        {
            class RSubclass
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

                public void recycleID(ulong id)
                {
                    m_Free.Enqueue(id & 0xFFFFFFFF);
                }
            }

            class RClass
            {
                List<RSubclass> m_SubclassList = new List<RSubclass>();

                public ulong giveID(int sub_gid)
                {
                    while (sub_gid >= m_SubclassList.Count)
                    {
                        m_SubclassList.Add(new RSubclass());
                    }

                    return m_SubclassList[sub_gid].giveID();
                }

                public void recycleID(int sub_gid, ulong id)
                {
                    m_SubclassList[sub_gid].recycleID(id);
                }
            }

            List<RClass> m_Groups = new List<RClass>();

            public ulong giveID(int class_id, int subclass_id)
            {
                while (class_id >= m_Groups.Count)
                {
                    m_Groups.Add(new RClass());
                }

                return (ulong)class_id << 48 | (ulong)subclass_id << 32 | m_Groups[class_id].giveID(subclass_id);
            }

            public void recycleID(int class_id, int subclass_id, ulong id)
            {
                m_Groups[class_id].recycleID(subclass_id, id);
            }
        }
        static readonly IDManager Manager = new IDManager();
        #endregion

        class Ref
        {
            public const ulong EmptyID = 0;

            public ITezGroup group = null;
            public ITezSubgroup subGroup = null;

            public int classID;
            public int subclassID;
            public ulong itemID = EmptyID;
            public int dbID = -1;

            public int refrence { get; private set; } = 0;

            public Ref(ITezGroup group, ITezSubgroup sub_group)
            {
                this.group = group;
                this.subGroup = sub_group;
            }

            public Ref(int class_id, int subclass_id)
            {
                this.classID = class_id;
                this.subclassID = subclass_id;
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
        public ITezGroup group
        {
            get { return m_Ref.group; }
        }

        public ITezSubgroup subgroup
        {
            get { return m_Ref.subGroup; }
        }

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
        /// 生成一个新的ID
        /// 不包含数据库ID
        /// 引用计数自动加一
        /// </summary>
        public TezRID(ITezGroup group, ITezSubgroup sub_group)
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
        public TezRID(ITezGroup group, ITezSubgroup sub_group, int db_id)
        {
            if (m_Ref != null)
            {
                throw new ArgumentException("m_Ref must null");
            }

            m_Ref = new Ref(group, sub_group)
            {
                dbID = db_id
            };

            m_Ref.itemID = Manager.giveID(group.toID, subgroup.toID);
            m_Ref.retain();
        }

        /// <summary>
        /// 生成一个新的ID并且包含数据库ID
        /// 引用计数自动加一
        /// </summary>
        public TezRID(int class_id, int subclass_id, int db_id)
        {
            if (m_Ref != null)
            {
                throw new ArgumentException("m_Ref must null");
            }

            m_Ref = new Ref(class_id, subclass_id)
            {
                dbID = db_id
            };

            m_Ref.itemID = Manager.giveID(class_id, subclass_id);
            m_Ref.retain();
        }

        /// <summary>
        /// 构造一个相同ID资源的副本
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
            var temp = new Ref(m_Ref.classID, m_Ref.subclassID);

            if (m_Ref.release())
            {
                Manager.recycleID(m_Ref.classID, m_Ref.subclassID, m_Ref.itemID);
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