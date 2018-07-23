﻿using System.Collections.Generic;
using tezcat.Signal;
using tezcat.String;

namespace tezcat.TypeTraits
{
    public class TezTypeRegisterHelper
    {
        static Dictionary<System.Type, TezEventCenter.Function<List<TezType>>> Registers = new Dictionary<System.Type, TezEventCenter.Function<List<TezType>>>();

        public static void add(System.Type type, TezEventCenter.Function<List<TezType>> function)
        {
            Registers.Add(type, function);
        }

        public static List<TezType> getList(System.Type type)
        {
            return Registers[type]();
        }
    }

    public class TezTypeRegister<T> where T : TezType, new()
    {
        public static List<T> TYPE { get; private set; } = new List<T>();
        static Dictionary<string, int> TYPEIndex = new Dictionary<string, int>();

        public static int count
        {
            get { return TYPE.Count; }
        }

        static TezTypeRegister()
        {
            TezTypeRegisterHelper.add(typeof(T), () => new List<TezType>(TYPE));
        }

        public static T register(string name)
        {
            var v = new T();
            v.init(TYPE.Count, name);
            TYPEIndex.Add(name, TYPE.Count);
            TYPE.Add(v);
            return v;
        }

        public static T get(int index)
        {
            return TYPE[index];
        }

        public static T get(string name)
        {
            return get(TYPEIndex[name]);
        }

        #region Switcher
        public class Switcher
        {
            TezEventCenter.Action[] m_CallBack = null;

            public Switcher()
            {
                m_CallBack = new TezEventCenter.Action[count];
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

            public Switcher register(T item, TezEventCenter.Action call_back)
            {
                m_CallBack[item.ID] = call_back;
                return this;
            }
        }

        public class Switcher<P1>
        {
            TezEventCenter.Action<P1>[] m_CallBack = null;

            public Switcher()
            {
                m_CallBack = new TezEventCenter.Action<P1>[count];
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

            public Switcher<P1> register(T item, TezEventCenter.Action<P1> call_back)
            {
                m_CallBack[item.ID] = call_back;
                return this;
            }
        }

        public class Switcher<P1, P2>
        {
            TezEventCenter.Action<P1, P2>[] m_CallBack = null;

            public Switcher()
            {
                m_CallBack = new TezEventCenter.Action<P1, P2>[count];
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

            public Switcher<P1, P2> register(T type, TezEventCenter.Action<P1, P2> call_back)
            {
                m_CallBack[type.ID] = call_back;
                return this;
            }
        }

        public class ReturnSwitcher<R>
        {
            TezEventCenter.Function<R>[] m_CallBack = null;

            public ReturnSwitcher()
            {
                m_CallBack = new TezEventCenter.Function<R>[count];
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

            public ReturnSwitcher<R> register(T type, TezEventCenter.Function<R> call_back)
            {
                m_CallBack[type.ID] = call_back;
                return this;
            }
        }

        public class ReturnSwitcher<R, P1>
        {
            TezEventCenter.Function<R, P1>[] m_CallBack = null;

            public ReturnSwitcher()
            {
                m_CallBack = new TezEventCenter.Function<R, P1>[count];
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

            public ReturnSwitcher<R, P1> register(T type, TezEventCenter.Function<R, P1> call_back)
            {
                m_CallBack[type.ID] = call_back;
                return this;
            }
        }

        public class ReturnSwitcher<R, P1, P2>
        {
            TezEventCenter.Function<R, P1, P2>[] m_CallBack = null;

            public ReturnSwitcher()
            {
                m_CallBack = new TezEventCenter.Function<R, P1, P2>[count];
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

            public ReturnSwitcher<R, P1, P2> register(T item, TezEventCenter.Function<R, P1, P2> call_back)
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
        public TezStaticString name { get; private set; }

        public TezType()
        {

        }

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
            return TezTypeRegister<T>.register(name);
        }

        protected static T register<T>(T e, string name) where T : TezType, new()
        {
            if (e == null)
            {
                return TezTypeRegister<T>.register(name);
            }

            return e;
        }

        protected static List<T> getList<T>() where T : TezType, new()
        {
            return TezTypeRegister<T>.TYPE;
        }

        public static bool operator !=(TezType x, TezType y)
        {
            /// (!true || !false) && (true || false) || (x)
            /// (!false || !false) && (false || false) || (x.ID != y.ID || x.name != y.name)

            var flagx = object.ReferenceEquals(x, null);
            var flagy = object.ReferenceEquals(y, null);

            return (!flagx || !flagy) && (flagx || flagy) || (x.ID != y.ID || x.name != y.name);
        }

        public static bool operator ==(TezType x, TezType y)
        {
            ///(false && false) || (x.ID == y.ID && x.name == y.name)
            ///(true && true) || (x)
            ///(false && true) || (!false && !true) && (x)

            var flagx = object.ReferenceEquals(x, null);
            var flagy = object.ReferenceEquals(y, null);

            return (flagx && flagy) || (!flagx && !flagy) && (x.ID == y.ID && x.name == y.name);
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
}