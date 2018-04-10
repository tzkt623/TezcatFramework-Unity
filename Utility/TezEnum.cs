namespace tezcat.Utility
{
    public class TezEnumRegister<T> where T : TezEnum, new()
    {
        public static int count { get; private set; } = 0;

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
                m_CallBack[item.id]();
            }

            public void register(T item, TezEventBus.Action call_back)
            {
                m_CallBack[item.id] = call_back;
            }
        }
        #endregion

        public static T register(string name)
        {
            var v = new T();
            v.init(count++, name);
            return v;
        }
    }

    public abstract class TezEnum
    {
        public int id { get; private set; }
        public string name { get; private set; }

        public void init(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public static bool operator !=(TezEnum v1, TezEnum v2)
        {
            return v1.id != v2.id;
        }

        public static bool operator ==(TezEnum v1, TezEnum v2)
        {
            return v1.id == v2.id;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.id;
        }
    }
}