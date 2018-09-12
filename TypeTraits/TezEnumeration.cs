using System;
using System.Collections.Generic;
using tezcat.Signal;

namespace tezcat.TypeTraits
{
    public class TezTypeListManager
    {
        static Dictionary<System.Type, TezEventDispatcher.Function<List<TezType>>> Registers = new Dictionary<System.Type, TezEventDispatcher.Function<List<TezType>>>();

        public static void add(System.Type type, TezEventDispatcher.Function<List<TezType>> function)
        {
            Registers.Add(type, function);
        }

        public static List<TezType> getList(System.Type type)
        {
            return Registers[type]();
        }
    }

    public class TezTypeList<T> where T : TezType, new()
    {
        public static List<T> List { get; private set; } = new List<T>();
        static Dictionary<string, int> Convertor = new Dictionary<string, int>();

        public static int count
        {
            get { return List.Count; }
        }

        static TezTypeList()
        {
            TezTypeListManager.add(typeof(T), () => new List<TezType>(List));
        }

        public static T register(string name)
        {
            var v = new T();
            v.init(List.Count, name);
            Convertor.Add(name, List.Count);
            List.Add(v);
            return v;
        }

        public static T get(int index)
        {
            return List[index];
        }

        public static T get(string name)
        {
            return get(Convertor[name]);
        }

        #region Switcher
        public class Switcher
        {
            TezEventDispatcher.Action[] m_CallBack = null;

            public Switcher()
            {
                m_CallBack = new TezEventDispatcher.Action[count];
                for (int i = 0; i < m_CallBack.Length; i++)
                {
                    m_CallBack[i] = this.defaultCallBack;
                }
            }

            void defaultCallBack()
            {

            }

            public void invoke(T item)
            {
                m_CallBack[item.ID]();
            }

            public Switcher register(T item, TezEventDispatcher.Action call_back)
            {
                m_CallBack[item.ID] = call_back;
                return this;
            }
        }

        public class Switcher<P1>
        {
            TezEventDispatcher.Action<P1>[] m_CallBack = null;

            public Switcher()
            {
                m_CallBack = new TezEventDispatcher.Action<P1>[count];
                for (int i = 0; i < m_CallBack.Length; i++)
                {
                    m_CallBack[i] = this.defaultCallBack;
                }
            }

            void defaultCallBack(P1 p1)
            {

            }

            public void invoke(T item, P1 p1)
            {
                m_CallBack[item.ID](p1);
            }

            public Switcher<P1> register(T item, TezEventDispatcher.Action<P1> call_back)
            {
                m_CallBack[item.ID] = call_back;
                return this;
            }
        }

        public class Switcher<P1, P2>
        {
            TezEventDispatcher.Action<P1, P2>[] m_CallBack = null;

            public Switcher()
            {
                m_CallBack = new TezEventDispatcher.Action<P1, P2>[count];
                for (int i = 0; i < m_CallBack.Length; i++)
                {
                    m_CallBack[i] = this.defaultCallBack;
                }
            }

            void defaultCallBack(P1 p1, P2 p2)
            {

            }

            public void invoke(T type, P1 p1, P2 p2)
            {
                m_CallBack[type.ID](p1, p2);
            }

            public Switcher<P1, P2> register(T type, TezEventDispatcher.Action<P1, P2> call_back)
            {
                m_CallBack[type.ID] = call_back;
                return this;
            }
        }

        public class ReturnSwitcher<R>
        {
            TezEventDispatcher.Function<R>[] m_CallBack = null;

            public ReturnSwitcher()
            {
                m_CallBack = new TezEventDispatcher.Function<R>[count];
                for (int i = 0; i < m_CallBack.Length; i++)
                {
                    m_CallBack[i] = this.defaultCallBack;
                }
            }

            R defaultCallBack()
            {
                return default(R);
            }

            public R invoke(T type)
            {
                return m_CallBack[type.ID]();
            }

            public ReturnSwitcher<R> register(T type, TezEventDispatcher.Function<R> call_back)
            {
                m_CallBack[type.ID] = call_back;
                return this;
            }
        }

        public class ReturnSwitcher<R, P1>
        {
            TezEventDispatcher.Function<R, P1>[] m_CallBack = null;

            public ReturnSwitcher()
            {
                m_CallBack = new TezEventDispatcher.Function<R, P1>[count];
                for (int i = 0; i < m_CallBack.Length; i++)
                {
                    m_CallBack[i] = this.defaultCallBack;
                }
            }

            R defaultCallBack(P1 p1)
            {
                return default(R);
            }

            public R invoke(T type, P1 p1)
            {
                return m_CallBack[type.ID](p1);
            }

            public ReturnSwitcher<R, P1> register(T type, TezEventDispatcher.Function<R, P1> call_back)
            {
                m_CallBack[type.ID] = call_back;
                return this;
            }
        }

        public class ReturnSwitcher<R, P1, P2>
        {
            TezEventDispatcher.Function<R, P1, P2>[] m_CallBack = null;

            public ReturnSwitcher()
            {
                m_CallBack = new TezEventDispatcher.Function<R, P1, P2>[count];
                for (int i = 0; i < m_CallBack.Length; i++)
                {
                    m_CallBack[i] = this.defaultCallBack;
                }
            }

            R defaultCallBack(P1 p1, P2 p2)
            {
                return default(R);
            }

            public R invoke(T item, P1 p1, P2 p2)
            {
                return m_CallBack[item.ID](p1, p2);
            }

            public ReturnSwitcher<R, P1, P2> register(T item, TezEventDispatcher.Function<R, P1, P2> call_back)
            {
                m_CallBack[item.ID] = call_back;
                return this;
            }
        }
        #endregion
    }

    public abstract class TezType
    {
        public int ID { get; protected set; }
        public string name { get; private set; }

        public void init(int id, string name)
        {
            this.ID = id;
            this.name = name;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.ID;
        }

        protected static T register<T>(string name) where T : TezType, new()
        {
            return TezTypeList<T>.register(name);
        }

        protected static T register<T>(T e, string name) where T : TezType, new()
        {
            if (e == null)
            {
                return TezTypeList<T>.register(name);
            }

            return e;
        }

        protected static List<T> getList<T>() where T : TezType, new()
        {
            return TezTypeList<T>.List;
        }

        public static bool operator !=(TezType x, TezType y)
        {
            /// (!true || !false) && (true || false) || (x)
            /// (!false || !false) && (false || false) || (x.ID != y.ID || x.name != y.name)

            var flagx = object.ReferenceEquals(x, null);
            var flagy = object.ReferenceEquals(y, null);

            return (!flagx || !flagy) && (flagx || flagy) || (x.ID != y.ID);
        }

        public static bool operator ==(TezType x, TezType y)
        {
            ///(false && false) || (x.ID == y.ID && x.name == y.name)
            ///(true && true) || (x)
            ///(false && true) || (!false && !true) && (x)

            var flagx = object.ReferenceEquals(x, null);
            var flagy = object.ReferenceEquals(y, null);

            return (flagx && flagy) || (!flagx && !flagy) && (x.ID == y.ID);
        }

        #region 重载操作
        public static bool operator true(TezType obj)
        {
            return !object.ReferenceEquals(obj, null);
        }

        public static bool operator false(TezType obj)
        {
            return object.ReferenceEquals(obj, null);
        }

        public static bool operator !(TezType obj)
        {
            return object.ReferenceEquals(obj, null);
        }
        #endregion
    }

    public abstract class TezEnumeration<TEnumeration, TValue>
        : IComparable<TEnumeration>
        , IEquatable<TEnumeration>
        where TEnumeration : TezEnumeration<TEnumeration, TValue>
        where TValue : IComparable
    {
        static Dictionary<string, TEnumeration> m_Dic = new Dictionary<string, TEnumeration>();
        public static TEnumeration get(string name)
        {
            return m_Dic[name];
        }

        public string name { get; }
        public TValue value { get; }
        public Type type
        {
            get { return typeof(TEnumeration); }
        }

        protected TezEnumeration(TValue value, string name)
        {
            this.value = value;
            this.name = name;

            m_Dic[this.name] = (TEnumeration)this;
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
        public override bool Equals(object obj)
        {
            return this.Equals((TEnumeration)obj);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public static bool operator !=(TezEnumeration<TEnumeration, TValue> x, TezEnumeration<TEnumeration, TValue> y)
        {
            /// (!true || !false) && (true || false) || (x)
            /// (!false || !false) && (false || false) || (x.ID != y.ID || x.name != y.name)

            var flagx = object.ReferenceEquals(x, null);
            var flagy = object.ReferenceEquals(y, null);

            return (!flagx || !flagy) && (flagx || flagy) || (x.value.CompareTo(y.value) != 0);
        }

        public static bool operator ==(TezEnumeration<TEnumeration, TValue> x, TezEnumeration<TEnumeration, TValue> y)
        {
            ///(false && false) || (x.ID == y.ID && x.name == y.name)
            ///(true && true) || (x)
            ///(false && true) || (!false && !true) && (x)

            var flagx = object.ReferenceEquals(x, null);
            var flagy = object.ReferenceEquals(y, null);

            return (flagx && flagy) || (!flagx && !flagy) && (x.value.CompareTo(y.value) == 0);
        }

        public static bool operator true(TezEnumeration<TEnumeration, TValue> obj)
        {
            return !object.ReferenceEquals(obj, null);
        }

        public static bool operator false(TezEnumeration<TEnumeration, TValue> obj)
        {
            return object.ReferenceEquals(obj, null);
        }

        public static bool operator !(TezEnumeration<TEnumeration, TValue> obj)
        {
            return object.ReferenceEquals(obj, null);
        }
        #endregion
    }
}