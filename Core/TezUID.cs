using System;
using System.Collections.Generic;

namespace tezcat.Core
{
    public class TezUID
        : IEquatable<TezUID>
        , IComparable<TezUID>
    {
        static Stack<uint> m_FreeIDList = new Stack<uint>();
        static uint m_Giver = 0;

        uint m_UID = 0;
        public TezUID()
        {
            if (m_FreeIDList.Count > 0)
            {
                m_UID = m_FreeIDList.Pop();
            }
            else
            {
                m_UID = m_Giver++;
            }
        }

        ~TezUID()
        {
            m_FreeIDList.Push(m_UID);
        }

        public override string ToString()
        {
            return string.Format("[UID-{0}]", m_UID);
        }

        public override bool Equals(object obj)
        {
            var other = obj as TezUID;
            if (other == null)
            {
                return false;
            }

            return m_UID == other.m_UID;
        }

        public override int GetHashCode()
        {
            return m_UID.GetHashCode();
        }

        public bool Equals(TezUID other)
        {
            return m_UID == other.m_UID;
        }

        public int CompareTo(TezUID other)
        {
            return m_UID.CompareTo(other.m_UID);
        }

        public static bool operator == (TezUID a, TezUID b)
        {
            return a.m_UID == b.m_UID;
        }

        public static bool operator !=(TezUID a, TezUID b)
        {
            return a.m_UID != b.m_UID;
        }

        public static implicit operator uint(TezUID uid)
        {
            return uid.m_UID;
        }

        public static bool operator !(TezUID uid)
        {
            return object.ReferenceEquals(uid, null);
        }
    }
}