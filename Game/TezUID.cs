using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat
{
    public class TezUID
        : IEquatable<TezUID>
        , IComparable<TezUID>
        , IEqualityComparer<TezUID>
        , IComparer<TezUID>
    {
        static Stack<uint> m_FreeIDList = new Stack<uint>();
        static uint m_Giver = 0;

        uint m_UID = 0;
        public uint UID
        {
            get { return m_UID; }
        }

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

        public bool Equals(TezUID x, TezUID y)
        {
            return x.m_UID == y.m_UID;
        }

        public int GetHashCode(TezUID obj)
        {
            return obj.GetHashCode();
        }

        public int Compare(TezUID x, TezUID y)
        {
            return x.m_UID.CompareTo(y.m_UID);
        }
    }
}