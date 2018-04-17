using System.Collections.Generic;

namespace tezcat.Utility
{
    public class TezRTTIRegister<T> where T : TezRTTI, new()
    {
        public static List<T> RTTI { get; private set; } = new List<T>();
        static Dictionary<string, int> RTTIIndex = new Dictionary<string, int>();

        public static int count
        {
            get { return RTTI.Count; }
        }

        public static T register(string name)
        {
            var v = new T();
            v.init(RTTI.Count, name);
            RTTIIndex.Add(name, RTTI.Count);
            RTTI.Add(v);
            return v;
        }

        public static T get(int index)
        {
            return RTTI[index];
        }

        public static T get(string name)
        {
            return get(RTTIIndex[name]);
        }

        #region Switcher
        public class Switcher
        {
            TezEventBus.Action[] m_CallBack = null;

            public Switcher()
            {
                m_CallBack = new TezEventBus.Action[count];
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

            public void register(T item, TezEventBus.Action call_back)
            {
                m_CallBack[item.ID] = call_back;
            }
        }

        public class Switcher<P1>
        {
            TezEventBus.Action<P1>[] m_CallBack = null;

            public Switcher()
            {
                m_CallBack = new TezEventBus.Action<P1>[count];
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

            public void register(T item, TezEventBus.Action<P1> call_back)
            {
                m_CallBack[item.ID] = call_back;
            }
        }

        public class Switcher<P1, P2>
        {
            TezEventBus.Action<P1, P2>[] m_CallBack = null;

            public Switcher()
            {
                m_CallBack = new TezEventBus.Action<P1, P2>[count];
                for (int i = 0; i < m_CallBack.Length; i++)
                {
                    m_CallBack[i] = this.defaultCallBack;
                }
            }

            void defaultCallBack(P1 p1, P2 p2)
            {

            }

            public void invoke(T item, P1 p1, P2 p2)
            {
                m_CallBack[item.ID](p1, p2);
            }

            public void register(T item, TezEventBus.Action<P1, P2> call_back)
            {
                m_CallBack[item.ID] = call_back;
            }
        }

        public class ReturnSwitcher<R>
        {
            TezEventBus.Function<R>[] m_CallBack = null;

            public ReturnSwitcher()
            {
                m_CallBack = new TezEventBus.Function<R>[count];
                for (int i = 0; i < m_CallBack.Length; i++)
                {
                    m_CallBack[i] = this.defaultCallBack;
                }
            }

            R defaultCallBack()
            {
                return default(R);
            }

            public R invoke(T item)
            {
                return m_CallBack[item.ID]();
            }

            public void register(T item, TezEventBus.Function<R> call_back)
            {
                m_CallBack[item.ID] = call_back;
            }
        }

        public class ReturnSwitcher<R, P1>
        {
            TezEventBus.Function<R, P1>[] m_CallBack = null;

            public ReturnSwitcher()
            {
                m_CallBack = new TezEventBus.Function<R, P1>[count];
                for (int i = 0; i < m_CallBack.Length; i++)
                {
                    m_CallBack[i] = this.defaultCallBack;
                }
            }

            R defaultCallBack(P1 p1)
            {
                return default(R);
            }

            public R invoke(T item, P1 p1)
            {
                return m_CallBack[item.ID](p1);
            }

            public void register(T item, TezEventBus.Function<R, P1> call_back)
            {
                m_CallBack[item.ID] = call_back;
            }
        }

        public class ReturnSwitcher<R, P1, P2>
        {
            TezEventBus.Function<R, P1, P2>[] m_CallBack = null;

            public ReturnSwitcher()
            {
                m_CallBack = new TezEventBus.Function<R, P1, P2>[count];
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

            public void register(T item, TezEventBus.Function<R, P1, P2> call_back)
            {
                m_CallBack[item.ID] = call_back;
            }
        }

        #endregion
    }

    public abstract class TezRTTI
    {
        public int ID { get; private set; }
        public string name { get; private set; }

        public void init(int id, string name)
        {
            this.ID = id;
            this.name = name;
        }

        public override int GetHashCode()
        {
            return this.ID;
        }

        protected static T initRTTI<T>(T e, string name) where T : TezRTTI, new()
        {
            if (e == null)
            {
                return TezRTTIRegister<T>.register(name);
            }

            return e;
        }

        public static T get<T>(int id) where T : TezRTTI, new()
        {
            return TezRTTIRegister<T>.get(id);
        }

        public static T get<T>(string name) where T : TezRTTI, new()
        {
            return TezRTTIRegister<T>.get(name);
        }
    }
}