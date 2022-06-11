using System;
using System.Collections.Generic;
using tezcat.Framework.Extension;

namespace tezcat.Framework.TypeTraits
{
    public class TezTypeListManager
    {
        static Dictionary<Type, TezEventExtension.Function<List<TezType>>> Registers = new Dictionary<Type, TezEventExtension.Function<List<TezType>>>();

        public static void add(Type type, TezEventExtension.Function<List<TezType>> function)
        {
            Registers.Add(type, function);
        }

        public static List<TezType> getList(Type type)
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
            TezEventExtension.Action[] m_CallBack = null;

            public Switcher()
            {
                m_CallBack = new TezEventExtension.Action[count];
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

            public Switcher register(T item, TezEventExtension.Action call_back)
            {
                m_CallBack[item.ID] = call_back;
                return this;
            }
        }

        public class Switcher<P1>
        {
            TezEventExtension.Action<P1>[] m_CallBack = null;

            public Switcher()
            {
                m_CallBack = new TezEventExtension.Action<P1>[count];
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

            public Switcher<P1> register(T item, TezEventExtension.Action<P1> call_back)
            {
                m_CallBack[item.ID] = call_back;
                return this;
            }
        }

        public class Switcher<P1, P2>
        {
            TezEventExtension.Action<P1, P2>[] m_CallBack = null;

            public Switcher()
            {
                m_CallBack = new TezEventExtension.Action<P1, P2>[count];
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

            public Switcher<P1, P2> register(T type, TezEventExtension.Action<P1, P2> call_back)
            {
                m_CallBack[type.ID] = call_back;
                return this;
            }
        }

        public class ReturnSwitcher<R>
        {
            TezEventExtension.Function<R>[] m_CallBack = null;

            public ReturnSwitcher()
            {
                m_CallBack = new TezEventExtension.Function<R>[count];
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

            public ReturnSwitcher<R> register(T type, TezEventExtension.Function<R> call_back)
            {
                m_CallBack[type.ID] = call_back;
                return this;
            }
        }

        public class ReturnSwitcher<R, P1>
        {
            TezEventExtension.Function<R, P1>[] m_CallBack = null;

            public ReturnSwitcher()
            {
                m_CallBack = new TezEventExtension.Function<R, P1>[count];
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

            public ReturnSwitcher<R, P1> register(T type, TezEventExtension.Function<R, P1> call_back)
            {
                m_CallBack[type.ID] = call_back;
                return this;
            }
        }

        public class ReturnSwitcher<R, P1, P2>
        {
            TezEventExtension.Function<R, P1, P2>[] m_CallBack = null;

            public ReturnSwitcher()
            {
                m_CallBack = new TezEventExtension.Function<R, P1, P2>[count];
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

            public ReturnSwitcher<R, P1, P2> register(T item, TezEventExtension.Function<R, P1, P2> call_back)
            {
                m_CallBack[item.ID] = call_back;
                return this;
            }
        }
        #endregion
    }

    public abstract class TezType : IEquatable<TezType>
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
            return this.Equals((TezType)obj);
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

        public bool Equals(TezType other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return this.ID == other.ID;
        }

        public static bool operator !=(TezType x, TezType y)
        {
            if (object.ReferenceEquals(x, null))
            {
                return object.ReferenceEquals(y, null);
            }

            return x.Equals(y);
        }

        public static bool operator ==(TezType x, TezType y)
        {
            return !(x == y);
        }
    }
}
