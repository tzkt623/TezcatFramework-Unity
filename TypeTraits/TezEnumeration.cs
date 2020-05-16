using System;
using System.Collections.Generic;

namespace tezcat.Framework.TypeTraits
{
    public interface ITezEnumeration
    {
        Type systemType { get; }
        string toName { get; }
        int toID { get; }
        bool sameAs(ITezEnumeration enumeration);
    }

    public abstract class TezEnumeration<TEnumeration, TEnumValue>
        : ITezEnumeration
        , IComparable<TEnumeration>
        , IEquatable<TEnumeration>
        where TEnumeration : TezEnumeration<TEnumeration, TEnumValue>
        where TEnumValue : struct, IComparable
    {
        #region Static
        static readonly string[] EnumNameArray = null;
        protected static readonly ITezEnumeration[] EnumArray = null;
        protected static readonly Dictionary<string, ITezEnumeration> EnumWithName = null;
        public static readonly int EnumCount = -1;

        static TezEnumeration()
        {
            var temp = (TEnumValue[])Enum.GetValues(typeof(TEnumValue));
            EnumCount = temp.Length;

            EnumWithName = new Dictionary<string, ITezEnumeration>(EnumCount);
            EnumNameArray = Enum.GetNames(typeof(TEnumValue));
            EnumArray = new ITezEnumeration[EnumCount];
        }
        #endregion

        public Type systemType { get; } = typeof(TEnumeration);

        public string toName
        {
            get { return EnumNameArray[toID]; }
        }

        public abstract int toID { get; }

        public TEnumValue value { get; private set; }

        protected TezEnumeration(TEnumValue value)
        {
            this.value = value;
            EnumWithName[this.toName] = (TEnumeration)this;
            EnumArray[this.toID] = (TEnumeration)this;
        }

        public bool sameAs(ITezEnumeration enumeration)
        {
            return this.systemType == enumeration.systemType && this.toID == enumeration.toID;
        }

        public int CompareTo(TEnumeration other)
        {
            return value.CompareTo(other.value);
        }

        public bool Equals(TEnumeration other)
        {
            return other != null && value.Equals(other.value);
        }

        #region 重载操作
        public static implicit operator TEnumValue(TezEnumeration<TEnumeration, TEnumValue> enumeration)
        {
            return enumeration.value;
        }

        public override bool Equals(object obj)
        {
            return this.Equals((TEnumeration)obj);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public static bool operator !=(TezEnumeration<TEnumeration, TEnumValue> x, TezEnumeration<TEnumeration, TEnumValue> y)
        {
            /// (!true || !false) && (true || false) || (x)
            /// (!false || !false) && (false || false) || (x.ID != y.ID || x.name != y.name)

            var flagx = object.ReferenceEquals(x, null);
            var flagy = object.ReferenceEquals(y, null);

            return (!flagx || !flagy) && (flagx || flagy || x.value.CompareTo(y.value) != 0);
        }

        public static bool operator ==(TezEnumeration<TEnumeration, TEnumValue> x, TezEnumeration<TEnumeration, TEnumValue> y)
        {
            ///(false && false) || (x.ID == y.ID && x.name == y.name)
            ///(true && true) || (x)
            ///(false && true) || (!false && !true) && (x)

            var flagx = object.ReferenceEquals(x, null);
            var flagy = object.ReferenceEquals(y, null);

            return (flagx && flagy) || (!flagx && !flagy && x.value.CompareTo(y.value) == 0);
        }

        public static bool operator true(TezEnumeration<TEnumeration, TEnumValue> obj)
        {
            return !object.ReferenceEquals(obj, null);
        }

        public static bool operator false(TezEnumeration<TEnumeration, TEnumValue> obj)
        {
            return object.ReferenceEquals(obj, null);
        }

        public static bool operator !(TezEnumeration<TEnumeration, TEnumValue> obj)
        {
            return object.ReferenceEquals(obj, null);
        }
        #endregion
    }
}