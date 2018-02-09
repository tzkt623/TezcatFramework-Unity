
namespace tezcat
{
    public class TezSingleton<T> where T : class, new()
    {
        private static T m_Instance = default(T);

        public static T instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new T();
                }
                return m_Instance;
            }
        }

        public static void init()
        {
            m_Instance = new T();
        }

        public static T getInstance
        {
            get { return m_Instance; }
        }
    }
}