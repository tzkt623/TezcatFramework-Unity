using System;
using System.Collections.Generic;

namespace tezcat.Framework.TypeTraits
{
    public interface ITezEnumeration
    {
        Type systemType { get; }
        string toName { get; }
        int toID { get; }
    }

    public interface ITezEnumeration<Self>
        : ITezEnumeration
        , IEquatable<Self>
    {

    }

    public abstract class TezEnumeration<Self, TEnumValue>
        : ITezEnumeration<Self>
        , IComparable<Self>
        where Self : TezEnumeration<Self, TEnumValue>
        where TEnumValue : struct, IComparable
    {
        #region Static
        static readonly string[] EnumNameArray = null;
        protected static readonly ITezEnumeration[] EnumArray = null;
        protected static readonly Dictionary<string, ITezEnumeration> EnumWithName = null;
        public static readonly int EnumCount = -1;

        static TezEnumeration()
        {
            EnumCount = Enum.GetValues(typeof(TEnumValue)).Length;

            EnumWithName = new Dictionary<string, ITezEnumeration>(EnumCount);
            EnumNameArray = Enum.GetNames(typeof(TEnumValue));
            EnumArray = new ITezEnumeration[EnumCount];
        }
        #endregion

        Type m_SystemType = typeof(Self);
        public Type systemType => m_SystemType;

        public string toName
        {
            get { return EnumNameArray[m_ID]; }
        }

        int m_ID = 0;
        public int toID => m_ID;

        TEnumValue m_Value;
        public TEnumValue value => m_Value;

        protected TezEnumeration(TEnumValue value)
        {
            m_Value = value;
            m_ID = Convert.ToInt32(m_Value);
            EnumWithName[this.toName] = (Self)this;
            EnumArray[this.toID] = (Self)this;
        }

        public override int GetHashCode()
        {
            return m_Value.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return this.Equals((Self)other);
        }

        public bool Equals(Self other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return m_SystemType == other.m_SystemType && this.m_ID == other.m_ID;
        }

        public int CompareTo(Self other)
        {
            return m_ID.CompareTo(other.m_ID);
        }

        public static bool operator !=(TezEnumeration<Self, TEnumValue> a, TezEnumeration<Self, TEnumValue> b)
        {
            return !(a == b);
        }

        public static bool operator ==(TezEnumeration<Self, TEnumValue> a, TezEnumeration<Self, TEnumValue> b)
        {
            if (object.ReferenceEquals(a, null))
            {
                return object.ReferenceEquals(b, null);
            }

            return a.Equals(b);
        }
    }
}